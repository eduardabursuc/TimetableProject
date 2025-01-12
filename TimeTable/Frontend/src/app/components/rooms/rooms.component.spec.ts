import { TestBed, ComponentFixture, waitForAsync } from '@angular/core/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RoomService } from '../../services/room.service';
import { GlobalsService } from '../../services/globals.service';
import { CookieService } from 'ngx-cookie-service';
import { RoomsComponent } from './rooms.component';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { LoadingComponent } from '../loading/loading.component';
import { Room } from '../../models/room.model';

describe('RoomsComponent', () => {
  let component: RoomsComponent;
  let fixture: ComponentFixture<RoomsComponent>;
  let roomService: RoomService;
  let router: Router;
  let globals: GlobalsService;
  let cookieService: CookieService;

  const mockRouter = { 
    events: of(null), 
    navigate: jasmine.createSpy('navigate') 
  };

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        FormsModule,
        CommonModule,
        SidebarMenuComponent,
        GenericModalComponent,
        LoadingComponent,
        RoomsComponent
      ],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: CookieService, useValue: { get: jasmine.createSpy('get').and.returnValue('fakeToken') } },
        { provide: RoomService, useValue: { getAll: () => of([]), create: () => of({ id: '123' }), update: () => of(null), delete: () => of([]) } },
        { provide: GlobalsService, useValue: { checkToken: jasmine.createSpy('checkToken') } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoomsComponent);
    component = fixture.componentInstance;
    roomService = TestBed.inject(RoomService);
    router = TestBed.inject(Router);
    globals = TestBed.inject(GlobalsService);
    cookieService = TestBed.inject(CookieService);

    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to login if token is empty', () => {
    (cookieService.get as jasmine.Spy).and.returnValue('');
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should fetch rooms on init', () => {
    spyOn(component, 'fetchRooms');
    component.ngOnInit();
    expect(component.fetchRooms).toHaveBeenCalled();
  });

  it('should set isLoading to false after fetching rooms', () => {
    spyOn(roomService, 'getAll').and.returnValue(of([]));
    component.fetchRooms();
    expect(component.isLoading).toBeFalse();
  });
/*
  it('should display an error message if room is invalid', () => {
    component.newRoom.name = '';
    component.newRoom.capacity = 0;
    const isValid = component.isValidRoom();
    expect(isValid).toBeFalse();
    expect(component.isModalVisible).toBeTrue();
    expect(component.modalMessage).toBe('Please fill in both fields.');
  });
*/
  it('should add a new room', () => {
    spyOn(roomService, 'create').and.returnValue(of({ id: '123' }));
    component.newRoom = { id: '', name: 'Test Room', capacity: 10 };
    component.addRoom();
    expect(roomService.create).toHaveBeenCalled();
    expect(component.rooms.length).toBe(1);
  });

  describe('isValidRoom', () => {
    it('should return false and show modal if room name is empty', () => {
      component.newRoom = { id: '1', name: '', capacity: 10 };

      const result = component.isValidRoom();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('Please fill in both fields.');
      expect(component.modalTitle).toBe('Invalid room');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
    });

    it('should return false and show modal if room capacity is less than or equal to 0', () => {
      component.newRoom = { id: '1', name: 'Room 1', capacity: 0 };

      const result = component.isValidRoom();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('Please fill in both fields.');
      expect(component.modalTitle).toBe('Invalid room');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
    });

    it('should return false and show modal if a room with the same name already exists', () => {
      component.newRoom = { id: '1', name: 'Room 1', capacity: 10 };
      component.rooms = [{ id: '2', name: 'Room 1', capacity: 15 }];

      const result = component.isValidRoom();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('A room with the same name already exists.');
      expect(component.modalTitle).toBe('Invalid room');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
    });

    it('should return true if room is valid and no room with the same name exists', () => {
      component.newRoom = { id: '1', name: 'Room 1', capacity: 10 };
      component.rooms = [{ id: '2', name: 'Room 2', capacity: 15 }];

      const result = component.isValidRoom();

      expect(result).toBeTrue();
    });
  });

  describe('updateRoom', () => {
    it('should update the room and update the rooms list', () => {
      component.newRoom = { id: '1', name: 'Updated Room', capacity: 20 };
      component.rooms = [{ id: '1', name: 'Room 1', capacity: 10 }, { id: '2', name: 'Room 2', capacity: 15 }];
      spyOn(component, 'isValidRoom').and.returnValue(true);
      spyOn(roomService, 'update').and.returnValue(of());

      component.updateRoom();

      expect(roomService.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Room', capacity: 20 });
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
      expect(component.isAddCase).toBeTrue();
    });

    it('should log an error if updating the room fails', () => {
      const mockError = { message: 'Request failed' };
      component.newRoom = { id: '1', name: 'Updated Room', capacity: 20 };
      spyOn(component, 'isValidRoom').and.returnValue(true);
      spyOn(roomService, 'update').and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.updateRoom();

      expect(roomService.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Room', capacity: 20 });
      expect(console.error).toHaveBeenCalledWith('Error updating room:', mockError);
    });

  });


  describe('showDeleteModal', () => {
    it('should set modal properties and make it visible', () => {
      const mockRoom: Room = { id: '1', name: 'Room 1', capacity: 5 };

      component.showDeleteModal(mockRoom);

      expect(component.roomToDelete).toBe(mockRoom);
      expect(component.modalTitle).toBe('Delete room');
      expect(component.cancelOption).toBeTrue();
      expect(component.modalMessage).toBe('Are you sure you want to delete Room 1 ?');
      expect(component.modalType).toBe('delete');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('deleteRoom', () => {
    it('should delete the room and update the rooms list', () => {
      component.roomToDelete = { id: '1', name: 'Room 1', capacity: 5 };
      component.rooms = [{ id: '1', name: 'Room 1', capacity: 5 }, { id: '2', name: 'Room 2', capacity: 10 }];
      spyOn(roomService, 'delete').and.returnValue(of());

      component.deleteRoom();

      expect(roomService.delete).toHaveBeenCalledWith('1');
    });

    it('should log an error if deleting the room fails', () => {
      const mockError = { message: 'Request failed' };
      component.roomToDelete = { id: '1', name: 'Room 1', capacity: 5 };
      spyOn(roomService, 'delete').and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.deleteRoom();

      expect(roomService.delete).toHaveBeenCalledWith('1');
      expect(console.error).toHaveBeenCalledWith('Error deleting room:', mockError);
    });
  });

  describe('handleModalConfirm', () => {
    it('should call deleteRoom if modalType is delete', () => {
      component.modalType = 'delete';
      spyOn(component, 'deleteRoom');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteRoom).toHaveBeenCalled();
    });

    it('should not call deleteRoom if modalType is not delete', () => {
      component.modalType = null;
      spyOn(component, 'deleteRoom');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteRoom).not.toHaveBeenCalled();
    });
  });

  describe('onBack', () => {
    it('should navigate back in history', () => {
      spyOn(window.history, 'back');

      component.onBack();

      expect(window.history.back).toHaveBeenCalled();
    });
  });
});

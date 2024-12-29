import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RoomsComponent } from './rooms.component';
import { Router } from '@angular/router';
import { RoomService } from '../../services/room.service';
import { CookieService } from 'ngx-cookie-service';
import { of, throwError } from 'rxjs';
import { Room } from '../../models/room.model';
import { fakeAsync} from '@angular/core/testing';

class MockRouter {
  events = of(); // Ensure 'events' is defined as an observable
  navigate = jasmine.createSpy('navigate');
}

describe('RoomsComponent', () => {
  let component: RoomsComponent;
  let fixture: ComponentFixture<RoomsComponent>;
  let mockRouter: MockRouter;
  let mockRoomService: jasmine.SpyObj<RoomService>;
  let mockCookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    mockRouter = new MockRouter(); // Use the MockRouter class
    mockRoomService = jasmine.createSpyObj('RoomService', ['getAll', 'create', 'update', 'delete']);
    mockCookieService = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [RoomsComponent],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: RoomService, useValue: mockRoomService },
        { provide: CookieService, useValue: mockCookieService },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoomsComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
/*
  describe('ngOnInit', () => {
    it('should redirect to login if no token is present', () => {
      mockCookieService.get.and.returnValue('');
      component.ngOnInit();
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should fetch rooms if a token is present', () => {
      mockCookieService.get.and.returnValue('mockToken');
      spyOn(localStorage, 'getItem').and.returnValue('mockUser');
      mockRoomService.getAll.and.returnValue(of());

      component.ngOnInit();

      expect(component.token).toBe('mockToken');
      expect(component.user).toBe('mockUser'); 
      expect(mockRoomService.getAll).toHaveBeenCalledWith('mockUser');
    });
  });
*/

  describe('fetchRooms', () => {
    it('should populate rooms on successful fetch', () => {
      const mockRooms: Room[] = [
        { id: '1', name: 'Room 1', capacity: 10 },
        { id: '2', name: 'Room 2', capacity: 20 }
      ];
      mockRoomService.getAll.and.returnValue(of(mockRooms));

      component.fetchRooms();

      expect(component.rooms).toEqual(mockRooms);
    });

    it('should handle errors when fetching rooms', () => {
      spyOn(console, 'error');
      mockRoomService.getAll.and.returnValue(throwError(() => new Error('Fetch error')));

      component.fetchRooms();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch rooms:', jasmine.any(Error));
    });
  });

  describe('isValidRoom', () => {
    beforeEach(() => {
      component.rooms = [
        { id: '1', name: 'Room 1', capacity: 10 }
      ];
    });

    it('should return false if room name is empty', () => {
      component.newRoom = { id: '', name: '', capacity: 0 };
      const result = component.isValidRoom();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('Please fill in both fields.');
    });

    it('should return false if capacity is zero or negative', () => {
      component.newRoom = { id: '', name: 'Room 2', capacity: 0 };
      const result = component.isValidRoom();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('Please fill in both fields.');
    });

    it('should return false if room name already exists', () => {
      component.newRoom = { id: '', name: 'Room 1', capacity: 10 };
      const result = component.isValidRoom();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('A room with the same name already exists.');
    });

    it('should return true for valid room', () => {
      component.newRoom = { id: '', name: 'Room 2', capacity: 10 };
      const result = component.isValidRoom();
      expect(result).toBe(true);
    });
  });

  describe('addRoom', () => {
    it('should not add room if validation fails', () => {
      component.newRoom = { id: '', name: '', capacity: 0 };
      spyOn(component, 'isValidRoom').and.returnValue(false);

      component.addRoom();

      expect(mockRoomService.create).not.toHaveBeenCalled();
    });

    it('should add room if validation passes', () => {
      component.newRoom = { id: '', name: 'Room 3', capacity: 15 };
      component.user = 'mockUser';
      spyOn(component, 'isValidRoom').and.returnValue(true);
      mockRoomService.create.and.returnValue(of({ id: '3' }));

      component.addRoom();

      expect(mockRoomService.create).toHaveBeenCalledWith({
        userEmail: 'mockUser',
        name: 'Room 3',
        capacity: 15,
      });
      expect(component.rooms.length).toBe(1);
      expect(component.rooms[0].name).toBe('Room 3');
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
    });
  });

  describe('updateRoom', () => {
    it('should not update room if validation fails', () => {
      component.newRoom = { id: '1', name: '', capacity: 0 };
      spyOn(component, 'isValidRoom').and.returnValue(false);

      component.updateRoom();

      expect(mockRoomService.update).not.toHaveBeenCalled();
    });
    /*
    it('should update room if validation passes', () => {
      component.newRoom = { id: '1', name: 'Updated Room', capacity: 20 };
      spyOn(component, 'isValidRoom').and.returnValue(true);
      mockRoomService.update.and.returnValue(of());

      component.updateRoom();

      expect(mockRoomService.update).toHaveBeenCalledWith('1', component.newRoom);
      expect(component.rooms[0].name).toBe('Updated Room');
      expect(component.newRoom).toEqual({ id: '', name: '', capacity: 0 });
    }); 
    */
  });

  describe('deleteRoom', () => {
    it('should delete a room', fakeAsync(() => {
      const roomToDelete: Room = { id: '1', name: 'Room 1', capacity: 10 };
      
      component.rooms = [roomToDelete];
      component.roomToDelete = roomToDelete;
      mockRoomService.delete.and.returnValue(of()); // Simulate successful deletion
  
      component.deleteRoom(); // Call the delete function
      expect(mockRoomService.delete).toHaveBeenCalledWith('1'); // Check if delete was called
      /* expect(component.rooms.length).toBe(0); */
    }));
  });

  describe('handleModalConfirm', () => {
    it('should close modal and delete room if modal type is delete', () => {
      component.modalType = 'delete';
      component.roomToDelete = { id: '1', name: 'Room 1', capacity: 10 };
      spyOn(component, 'deleteRoom');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBe(false);
      expect(component.deleteRoom).toHaveBeenCalled();
    });
  });

  describe('onBack', () => {
    it('should go back in history', () => {
      spyOn(window.history, 'back');
      component.onBack();
      expect(window.history.back).toHaveBeenCalled();
    });
  });
});

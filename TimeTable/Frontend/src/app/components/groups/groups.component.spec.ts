import { TestBed, ComponentFixture, waitForAsync } from '@angular/core/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GroupService } from '../../services/group.service';
import { GlobalsService } from '../../services/globals.service';
import { CookieService } from 'ngx-cookie-service';
import { GroupsComponent } from './groups.component';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { LoadingComponent } from '../loading/loading.component';
import { Group } from '../../models/group.model';

describe('GroupsComponent', () => {
  let component: GroupsComponent;
  let fixture: ComponentFixture<GroupsComponent>;
  let groupService: GroupService;
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
        GroupsComponent
      ],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: CookieService, useValue: { get: jasmine.createSpy('get').and.returnValue('fakeToken') } },
        { provide: GroupService, useValue: { getAll: () => of([]), create: () => of({ id: '123' }), update: () => of(null), delete: () => of(null) } },
        { provide: GlobalsService, useValue: { checkToken: jasmine.createSpy('checkToken') } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GroupsComponent);
    component = fixture.componentInstance;
    groupService = TestBed.inject(GroupService);
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

  it('should fetch groups on init', () => {
    spyOn(component, 'fetchGroups');
    component.ngOnInit();
    expect(component.fetchGroups).toHaveBeenCalled();
  });

  it('should set isLoading to false after fetching groups', () => {
    spyOn(groupService, 'getAll').and.returnValue(of([]));
    component.fetchGroups();
    expect(component.isLoading).toBeFalse();
  });

  it('should display an error message if group name is invalid', () => {
    component.newGroup.name = '';
    const isValid = component.isValidGroup();
    expect(isValid).toBeFalse();
    expect(component.isModalVisible).toBeTrue();
    expect(component.modalMessage).toBe("Please fill in group's name.");
  });

  it('should add a new group', () => {
    spyOn(groupService, 'create').and.returnValue(of({ id: '123' }));
    component.newGroup.name = 'Test Group';
    component.addGroup();
    expect(groupService.create).toHaveBeenCalled();
    expect(component.groups.length).toBe(1);
  });

  describe('isValidGroup', () => {
    it('should return false and show modal if group name is empty', () => {
      component.newGroup = { id: '1', name: '' };

      const result = component.isValidGroup();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe("Please fill in group's name.");
      expect(component.modalTitle).toBe('Invalid group');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newGroup).toEqual({ id: '', name: '' });
    });

    it('should return false and show modal if a group with the same name already exists', () => {
      component.newGroup = { id: '1', name: 'Group 1' };
      component.groups = [{ id: '2', name: 'Group 1' }];

      const result = component.isValidGroup();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('A group with the same name already exists.');
      expect(component.modalTitle).toBe('Invalid group');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newGroup).toEqual({ id: '', name: '' });
    });

    it('should return true if group is valid and no group with the same name exists', () => {
      component.newGroup = { id: '1', name: 'Group 1' };
      component.groups = [{ id: '2', name: 'Group 2' }];

      const result = component.isValidGroup();

      expect(result).toBeTrue();
    });
  });

  describe('editGroup', () => {
    it('should set newGroup and isAddCase correctly', () => {
      const mockGroup: Group = { id: '1', name: 'Group 1' };

      component.editGroup(mockGroup);

      expect(component.newGroup).toBe(mockGroup);
      expect(component.isAddCase).toBeFalse();
    });
  });

  describe('updateGroup', () => {
    it('should update the group and update the groups list', () => {
      component.newGroup = { id: '1', name: 'Updated Group' };
      component.groups = [{ id: '1', name: 'Group 1' }, { id: '2', name: 'Group 2' }];
      spyOn(component, 'isValidGroup').and.returnValue(true);
      spyOn(groupService, 'update').and.returnValue(of());

      component.updateGroup();

      expect(groupService.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Group' });
      expect(component.newGroup).toEqual({ id: '', name: '' });
      expect(component.isAddCase).toBeTrue();
    });

    it('should log an error if updating the group fails', () => {
      const mockError = { message: 'Request failed' };
      component.newGroup = { id: '1', name: 'Updated Group' };
      spyOn(component, 'isValidGroup').and.returnValue(true);
      spyOn(groupService, 'update').and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.updateGroup();

      expect(groupService.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Group' });
      expect(console.error).toHaveBeenCalledWith('Error updating group:', mockError);
    });
  });

  describe('showDeleteModal', () => {
    it('should set modal properties and make it visible', () => {
      const mockGroup: Group = { id: '1', name: 'Group 1' };

      component.showDeleteModal(mockGroup);

      expect(component.groupToDelete).toBe(mockGroup);
      expect(component.modalTitle).toBe('Delete group');
      expect(component.cancelOption).toBeTrue();
      expect(component.modalMessage).toBe('Are you sure you want to delete Group 1 ?');
      expect(component.modalType).toBe('delete');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('deleteGroup', () => {
    it('should delete the group and update the groups list', () => {
      component.groupToDelete = { id: '1', name: 'Group 1' };
      component.groups = [{ id: '1', name: 'Group 1' }, { id: '2', name: 'Group 2' }];
      spyOn(groupService, 'delete').and.returnValue(of());

      component.deleteGroup();

      expect(groupService.delete).toHaveBeenCalledWith('1');
    });

    it('should log an error if deleting the group fails', () => {
      const mockError = { message: 'Request failed' };
      component.groupToDelete = { id: '1', name: 'Group 1' };
      spyOn(groupService, 'delete').and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.deleteGroup();

      expect(groupService.delete).toHaveBeenCalledWith('1');
      expect(console.error).toHaveBeenCalledWith('Error deleting group:', mockError);
    });
  });

  describe('handleModalConfirm', () => {
    it('should call deleteGroup if modalType is delete', () => {
      component.modalType = 'delete';
      spyOn(component, 'deleteGroup');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteGroup).toHaveBeenCalled();
    });

    it('should not call deleteGroup if modalType is not delete', () => {
      component.modalType = null;
      spyOn(component, 'deleteGroup');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteGroup).not.toHaveBeenCalled();
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

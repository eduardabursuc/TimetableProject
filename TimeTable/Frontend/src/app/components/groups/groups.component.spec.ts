import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GroupsComponent } from './groups.component';
import { Router } from '@angular/router';
import { GroupService } from '../../services/group.service';
import { CookieService } from 'ngx-cookie-service';
import { of, throwError } from 'rxjs';
import { Group } from '../../models/group.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';

class MockRouter {
  events = of(); // Ensure 'events' is defined as an observable
  navigate = jasmine.createSpy('navigate');
}

describe('GroupsComponent', () => {
  let component: GroupsComponent;
  let fixture: ComponentFixture<GroupsComponent>;
  let mockRouter: MockRouter;
  let mockGroupService: jasmine.SpyObj<GroupService>;
  let mockCookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    mockRouter = new MockRouter(); // Use the MockRouter class
    mockGroupService = jasmine.createSpyObj('GroupService', ['getAll', 'create', 'update', 'delete']);
    mockCookieService = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        SidebarMenuComponent,
        CommonModule,
        GenericModalComponent,
        GroupsComponent
      ],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: GroupService, useValue: mockGroupService },
        { provide: CookieService, useValue: mockCookieService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GroupsComponent);
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

    it('should fetch groups if a token is present', () => {
      mockCookieService.get.and.returnValue('mockToken');
      spyOn(localStorage, 'getItem').and.returnValue('mockUser');
      mockGroupService.getAll.and.returnValue(of());

      component.ngOnInit();

      expect(component.token).toBe('mockToken');
      expect(component.user).toBe('mockUser'); 
      expect(mockGroupService.getAll).toHaveBeenCalledWith('mockUser');
    });
  });
*/
  describe('fetchGroups', () => {
    it('should populate groups on successful fetch', () => {
      const mockGroups: Group[] = [
        { id: '1', name: 'Group 1' },
        { id: '2', name: 'Group 2' }
      ];
      mockGroupService.getAll.and.returnValue(of(mockGroups));

      component.fetchGroups();

      expect(component.groups).toEqual(mockGroups);
    });

    it('should handle errors when fetching groups', () => {
      spyOn(console, 'error');
      mockGroupService.getAll.and.returnValue(throwError(() => new Error('Fetch error')));

      component.fetchGroups();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch groups:', jasmine.any(Error));
    });
  });

  describe('isValidGroup', () => {
    beforeEach(() => {
      component.groups = [
        { id: '1', name: 'Group 1' }
      ];
    });

    it('should return false if group name is empty', () => {
      component.newGroup = { id: '', name: '' };
      const result = component.isValidGroup();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('Please fill in group\'s name.');
    });

    it('should return false if group name already exists', () => {
      component.newGroup = { id: '', name: 'Group 1' };
      const result = component.isValidGroup();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('A group with the same name already exists.');
    });

    it('should return true for valid group', () => {
      component.newGroup = { id: '', name: 'Group 2' };
      const result = component.isValidGroup();
      expect(result).toBe(true);
    });
  });

  describe('addGroup', () => {
    it('should not add group if validation fails', () => {
      component.newGroup = { id: '', name: '' };
      spyOn(component, 'isValidGroup').and.returnValue(false);

      component.addGroup();

      expect(mockGroupService.create).not.toHaveBeenCalled();
    });

    it('should add group if validation passes', () => {
      component.newGroup = { id: '', name: 'Group 3' };
      component.user = 'mockUser';
      spyOn(component, 'isValidGroup').and.returnValue(true);
      mockGroupService.create.and.returnValue(of({ id: '3' }));

      component.addGroup();

      expect(mockGroupService.create).toHaveBeenCalledWith({
        userEmail: 'mockUser',
        name: 'Group 3'
      });
      expect(component.groups.length).toBe(1);
      expect(component.groups[0].name).toBe('Group 3');
      expect(component.newGroup).toEqual({ id: '', name: '' });
    });
  });

  describe('updateGroup', () => {
    it('should not update group if validation fails', () => {
      component.newGroup = { id: '1', name: '' };
      spyOn(component, 'isValidGroup').and.returnValue(false);

      component.updateGroup();

      expect(mockGroupService.update).not.toHaveBeenCalled();
    });
    /*
    it('should update group if validation passes', () => {
      component.newGroup = { id: '1', name: 'Updated Group' };
      spyOn(component, 'isValidGroup').and.returnValue(true);
      mockGroupService.update.and.returnValue(of());

      component.updateGroup();

      expect(mockGroupService.update).toHaveBeenCalledWith('1', component.newGroup);
      const index = component.groups.findIndex(group => group.id === '1');
      expect(component.groups[index].name).toBe('Updated Group');
      expect(component.newGroup).toEqual({ id: '', name: '' });
      expect(component.isAddCase).toBe(true);
    });
    */
  });
/*
  describe('deleteGroup', () => {
    it('should delete a group', () => {
      const groupToDelete: Group = { id: '1', name: 'Group 1' };
      
      component.groups = [groupToDelete];
      component.groupToDelete = groupToDelete;
      mockGroupService.delete.and.returnValue(of());

      component.deleteGroup();

      expect(mockGroupService.delete).toHaveBeenCalledWith('1');
      expect(component.groups.length).toBe(0);
    });
  });
*/
  describe('handleModalConfirm', () => {
    it('should close modal and delete group if modal type is delete', () => {
      component.modalType = 'delete';
      component.groupToDelete = { id: '1', name: 'Group 1' };
      spyOn(component, 'deleteGroup');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBe(false);
      expect(component.deleteGroup).toHaveBeenCalled();
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

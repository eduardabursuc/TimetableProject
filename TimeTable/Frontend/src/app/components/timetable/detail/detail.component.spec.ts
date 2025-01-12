import { TestBed, ComponentFixture } from '@angular/core/testing';
import { DetailComponent } from './detail.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { TimetableService } from '../../../services/timetable.service';
import { CourseService } from '../../../services/course.service';
import { ProfessorService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { GroupService } from '../../../services/group.service';
import { Course } from '../../../models/course.model';
import { Professor } from '../../../models/professor.model';
import { Room } from '../../../models/room.model';
import { Group } from '../../../models/group.model';
import { of, throwError } from 'rxjs';
import { Timetable } from '../../../models/timetable.model';
import { Constraint, ConstraintType } from '../../../models/constraint.model';
import { ConstraintService } from '../../../services/constraint.service';


describe('DetailComponent', () => {
  let component: DetailComponent;
  let fixture: ComponentFixture<DetailComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;
  let courseService: jasmine.SpyObj<CourseService>;
  let constraintService: jasmine.SpyObj<ConstraintService>;
  let professorService: jasmine.SpyObj<ProfessorService>;
  let roomService: jasmine.SpyObj<RoomService>;
  let groupService: jasmine.SpyObj<GroupService>;
  let cookieService: jasmine.SpyObj<CookieService>;
  let globals: any = { checkToken: jasmine.createSpy('checkToken') };
  let router: any = { navigate: jasmine.createSpy('navigate') };
  let activatedRoute: any = { params: of({ id: '123' }) };

  const mockConstraint = { 
    id: '1',
    type: ConstraintType.HARD_NO_OVERLAP, 
    professorId: '1', 
    courseName: 'Math', 
    roomName: 'C2', 
    wantedRoomName: 'C1', 
    groupName: 'G1', 
    day: 'Monday',
    wantedDay: 'Tuesday',
    time: '08:00-10:00',
    wantedTime: '10:00-12:00',
    event: 'laboratory' 
  };

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getById', 'delete', 'update']);
    const courseServiceSpy = jasmine.createSpyObj('CourseService', ['getAll', 'getById']);
    const constraintServiceSpy = jasmine.createSpyObj('ConstraintService', ['create', 'delete', 'getAllForProfessor']);
    const professorServiceSpy = jasmine.createSpyObj('ProfessorService', ['getAll', 'getById']);
    const roomServiceSpy = jasmine.createSpyObj('RoomService', ['getAll', 'getById']);
    const groupServiceSpy = jasmine.createSpyObj('GroupService', ['getAll', 'getById']);
    const cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule, FormsModule, DetailComponent],
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy },
        { provide: CourseService, useValue: courseServiceSpy },
        { provide: ConstraintService, useValue: constraintServiceSpy },
        { provide: ProfessorService, useValue: professorServiceSpy },
        { provide: RoomService, useValue: roomServiceSpy },
        { provide: GroupService, useValue: groupServiceSpy },
        { provide: CookieService, useValue: cookieServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DetailComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
    courseService = TestBed.inject(CourseService) as jasmine.SpyObj<CourseService>;
    constraintService = TestBed.inject(ConstraintService) as jasmine.SpyObj<ConstraintService>;
    professorService = TestBed.inject(ProfessorService) as jasmine.SpyObj<ProfessorService>;
    roomService = TestBed.inject(RoomService) as jasmine.SpyObj<RoomService>;
    groupService = TestBed.inject(GroupService) as jasmine.SpyObj<GroupService>;
    cookieService = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });


  describe('ngOnInit', () => {

    it('should initialize component and load data', () => {
      cookieService.get.and.returnValue('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RAZ21haWwuY29tIiwicm9sZSI6ImFkbWluIiwibmJmIjoxNzM2NTk4MzM0LCJleHAiOjE3MzY2ODQ3MzQsImlhdCI6MTczNjU5ODMzNH0.SRp2duRGuEJU3sCqe1SSeNpsFA3HDRLhH0bUxZ4o3Ds');
      spyOn(localStorage, 'getItem').and.callFake((key: string) => {
        if (key === 'user') return 'testUser';
        if (key === 'role') return 'testRole';
        return null;
      });

      component.ngOnInit();

      expect(cookieService.get).toHaveBeenCalledWith('authToken');
      expect(component.token).toBe('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RAZ21haWwuY29tIiwicm9sZSI6ImFkbWluIiwibmJmIjoxNzM2NTk4MzM0LCJleHAiOjE3MzY2ODQ3MzQsImlhdCI6MTczNjU5ODMzNH0.SRp2duRGuEJU3sCqe1SSeNpsFA3HDRLhH0bUxZ4o3Ds');
      expect(component.user).toBe('testUser');
      expect(component.role).toBe('testRole');
      expect(component.id).toBe('');
    });

  });


  it('should fetch timetable data by ID on initialization', () => {
    const timetableMock = { id: '1', events: [] } as any;
    timetableService.getById.and.returnValue(of(timetableMock));
  
    component.getTimetableById('1'); // Call method
  
    expect(timetableService.getById).toHaveBeenCalledWith('1');
    expect(component.timetable).toEqual(timetableMock);
    expect(component.errorMessage).toBeNull();
  });

  it('should handle error when fetching timetable fails', () => {
    timetableService.getById.and.returnValue(throwError('Error fetching timetable'));
    component.getTimetableById('1');
    expect(component.errorMessage).toContain('Failed to fetch details for ID: 1.');
  });

  it('should toggle edit mode', () => {
    component.isEditMode = false;
    component.toggleEditMode();
    expect(component.isEditMode).toBeTrue();
  });  

  it('should fetch related data during initialization', () => {
    const mockCourse: Course[] = [{ id: '1', courseName: 'Sample', package: 'compulsory', credits: 5, level: 'license', semester: 4 }];
    const mockProfessor: Professor[] = [{ id: '1', name: 'Sample', email: "some@gmail.com" }];
    const mockGroup: Group[] = [{ id: '1', name: 'Sample' }];
    const mockRoom: Room[] = [{ id: '1', name: 'Sample', capacity: 10 }];

    courseService.getAll.and.returnValue(of(mockCourse));
    professorService.getAll.and.returnValue(of(mockProfessor));
    groupService.getAll.and.returnValue(of(mockGroup));
    roomService.getAll.and.returnValue(of(mockRoom));

    component.fetchData();

    expect(component.courses).toEqual(mockCourse);
    expect(component.professors).toEqual(mockProfessor);
    expect(component.groups).toEqual(mockGroup);
    expect(component.rooms).toEqual(mockRoom);
  });

  it('should toggle privacy to public', () => {
    component.timetable = { isPublic: false } as any;

    component.togglePrivacy();

    if( component.timetable )
    expect(component.timetable.isPublic).toBeTrue();
    expect(component.privacy).toBe('public');
  });

  it('should toggle privacy to private', () => {
    component.timetable = { isPublic: true } as any;

    component.togglePrivacy();

    if( component.timetable )
    expect(component.timetable.isPublic).toBeFalse();
    expect(component.privacy).toBe('private');
  });

  it('should show delete modal', () => {
    const timetable = { id: '1', name: 'Test Timetable' } as any;

    component.showDeleteModal(timetable);

    expect(component.modalType).toBe('delete');
    expect(component.timetableToDelete).toBe(timetable);
  });

  it('should format time correctly', () => {
    expect(component.formatTime('9:5')).toBe('09:05');
    expect(component.formatTime('12:00')).toBe('12:00');
  });

  describe('handleModalConfirm', () => {
    it('should delete timetable if confirmed and modalType is delete', () => {
      spyOn(component, 'deleteTimetable');
      component.modalType = 'delete';
      component.timetableToDelete = { id: '1' } as any;

      component.handleModalConfirm({ confirmed: true });

      expect(component.deleteTimetable).toHaveBeenCalledWith('1');
    });

    it('should handle update if confirmed and modalType is edit', () => {
      spyOn(component, 'handleUpdate');
      component.modalType = 'edit';

      component.handleModalConfirm({ confirmed: true });

      expect(component.handleUpdate).toHaveBeenCalled();
    });

    it('should handle add constraint if confirmed and modalType is addConstraint', () => {
      spyOn(component, 'handleAddConstraint');
      component.modalType = 'addConstraint';

      component.handleModalConfirm({ confirmed: true, inputValue: 'Constraint' });

      expect(component.handleAddConstraint).toHaveBeenCalled();
    });

    it('should handle delete constraint if confirmed and modalType is deleteConstraint', () => {
      spyOn(component, 'handleDeleteConstraint');
      component.modalType = 'deleteConstraint';

      component.handleModalConfirm({ confirmed: true });

      expect(component.handleDeleteConstraint).toHaveBeenCalled();
    });

    it('should reset modal state after handling confirmation', () => {
      component.handleModalConfirm({ confirmed: true });

      expect(component.isInputRequired).toBeFalse();
      expect(component.inputValue).toBe('');
      expect(component.isEditMode).toBeFalse();
      expect(component.isModalVisible).toBeFalse();
      expect(component.modalType).toBeNull();
      expect(component.modalMessage).toBe('');
    });
  });

  describe('handleUpdate', () => {
    it('should return if timetable is not defined', () => {
      component.timetable = null;

      component.handleUpdate();

      expect(component.timetable).toBeNull();
    });

    it('should update timetable events with correct data and call timetableService.update', () => {
      component.timetable = {
        id: '1',
        events: [
          {
            courseId: '1',
            roomId: '1',
            professorId: '1',
            groupId: '1',
            weekEvenness: true,
            courseName: 'Old Course',
            roomName: 'Old Room',
            professorName: 'Old Professor',
            group: 'Old Group',
            timeslot: { day: 'Monday', startTime: '08:00', endTime: '10:00', time: '08:00-10:00' },
            isEven: true,
          }
        ]
      } as Timetable;

      timetableService.update.and.returnValue(of());

      component.handleUpdate();

      expect(timetableService.update).toHaveBeenCalledWith('1', component.timetable);
      expect(component.isEditMode).toBeFalse();
    });


    it('should handle update error', () => {
      component.timetable = {
        id: '1',
        name: 'Test Timetable',
        events: [],
        isPublic: false,
        userEmail: '',
        createdAt: new Date(),
      } as Timetable;

      timetableService.update.and.returnValue(throwError('Error updating timetable'));

      component.handleUpdate();

      expect(component.errorMessage).toBe('Failed to update timetable. Please try again.');
    });
  });

  describe('showDeleteModalConstraint', () => {
    it('should set constraintToDelete and show modal with correct message', () => {
      component.showDeleteModalConstraint(mockConstraint);

      expect(component.constraintToDelete).toBe(mockConstraint);
      expect(component.modalTitle).toBe('Confirm Deletion');
      expect(component.modalType).toBe('deleteConstraint');
      expect(component.isModalVisible).toBeTrue();
    });

    describe('showSaveModal', () => {
      it('should set modal properties and show modal with correct message', () => {
        const timetable: Timetable = { id: '1', name: 'Test Timetable' } as Timetable;
  
        component.showSaveModal(timetable);
  
        expect(component.modalTitle).toBe('Confirm Saving Changes');
        expect(component.modalMessage).toBe('Confirm changes for timetable "Test Timetable?"');
        expect(component.modalType).toBe('edit');
        expect(component.isModalVisible).toBeTrue();
      });
    });

    describe('addConstraint', () => {
      it('should set modal properties and show modal for adding constraint', () => {
        const timetable: Timetable = { id: '1', name: 'Test Timetable' } as Timetable;
  
        component.addConstraint(timetable);
  
        expect(component.isInputRequired).toBeTrue();
        expect(component.modalTitle).toBe('Add constraint');
        expect(component.inputPlaceholder).toBe('Constraint description');
        expect(component.modalType).toBe('addConstraint');
        expect(component.isModalVisible).toBeTrue();
      });
    });
  });

  describe('getGroupNameById', () => {
    it('should return group name by id', () => {
      component.groups = [{ id: '1', name: 'Group 1' } as Group];
      const groupName = component.getGroupNameById('1');
      expect(groupName).toBe('Group 1');
    });

    it('should return undefined if group id is not found', () => {
      component.groups = [{ id: '1', name: 'Group 1' } as Group];
      const groupName = component.getGroupNameById('2');
      expect(groupName).toBeUndefined();
    });
  });

  describe('getRoomNameById', () => {
    it('should return room name by id', () => {
      component.rooms = [{ id: '1', name: 'Room 1' } as Room];
      const roomName = component.getRoomNameById('1');
      expect(roomName).toBe('Room 1');
    });

    it('should return undefined if room id is not found', () => {
      component.rooms = [{ id: '1', name: 'Room 1' } as Room];
      const roomName = component.getRoomNameById('2');
      expect(roomName).toBeUndefined();
    });
  });

  describe('getCourseNameById', () => {
    it('should return course name by id', () => {
      component.courses = [{ id: '1', courseName: 'Course 1' } as Course];
      const courseName = component.getCourseNameById('1');
      expect(courseName).toBe('Course 1');
    });

    it('should return undefined if course id is not found', () => {
      component.courses = [{ id: '1', courseName: 'Course 1' } as Course];
      const courseName = component.getCourseNameById('2');
      expect(courseName).toBeUndefined();
    });
  });

  describe('onBack', () => {
    it('should call window.history.back', () => {
      spyOn(window.history, 'back');
      component.onBack();
      expect(window.history.back).toHaveBeenCalled();
    });
  });

  describe('groupEventsByDay', () => {
    /*it('should group events by day and sort days for display', () => {
      component.filteredEvents = [
        { timeslot: { time: '08:00-10:00', day: 'Monday', isAvailable: false } },
        { timeslot: { day: 'Wednesday' } },
        { timeslot: { day: 'Monday' } },
        { timeslot: { day: 'Friday' } },
      ] as any[];

      component.groupEventsByDay();

      expect(component.groupedEvents).toEqual({
        Monday: [
          { timeslot: { time: '08:00-10:00', day: 'Monday' }  },
          { timeslot: { day: 'Monday' } },
        ],
        Wednesday: [{ timeslot: { day: 'Wednesday' } }],
        Friday: [{ timeslot: { day: 'Friday' } }],
      });

      expect(component.sortedDays).toEqual(['Monday', 'Wednesday', 'Friday']);
    });*/

    it('should handle empty filteredEvents', () => {
      component.filteredEvents = [];

      component.groupEventsByDay();

      expect(component.groupedEvents).toEqual({});
      expect(component.sortedDays).toEqual([]);
    });
  });

  describe('handleAddConstraint', () => {

    it('should set isLoading to true and make a request when inputValue is valid', () => {
      const mockResponse = { id: '1', input: 'mockInput' };
      constraintService.create.and.returnValue(of(mockResponse));
      spyOn(component, 'loadConstraints');

      component.inputValue = 'mockInput';
      component.user = 'mockUser';
      component.id = 'mockId';

      component.handleAddConstraint();

      //expect(component.isLoading).toBeTrue();
      expect(constraintService.create).toHaveBeenCalledWith({
        professorEmail: 'mockUser',
        timetableId: 'mockId',
        input: 'mockInput',
      });
      expect(component.loadConstraints).toHaveBeenCalled();
      expect(component.isLoading).toBeFalse();
    });
  
    it('should handle errors and display an error message if the request fails', () => {
      const mockError = { message: 'Request failed' };
      constraintService.create.and.returnValue(throwError(mockError));
      spyOn(console, 'error');
  
      component.inputValue = 'mockInput';
      component.user = 'mockUser';
      component.id = 'mockId';
  
      component.handleAddConstraint();
  
      expect(component.isLoading).toBeFalse();
      expect(console.error).toHaveBeenCalledWith('Failed to add a constraint:', mockError);
      expect(component.errorMessage).toBe('Failed to add a constraint. Please try again.');
    });
  
    it('should not make a request if inputValue is empty', () => {
      component.inputValue = '';
      component.user = 'mockUser';
      component.id = 'mockId';
  
      component.handleAddConstraint();
  
      expect(constraintService.create).not.toHaveBeenCalled();
    });

  });
/*
  describe('populateEventDetails', () => {

    it('should populate event details correctly', () => {
      component.filteredEvents = [
        {
          courseId: '1',
          professorId: '1',
          roomId: '1',
          groupId: '1',
          isEven: true,
          timeslot: { time: '08:00-10:00' },
        },
      ] as any[];

      const mockCourse = { id: '1' ,courseName: 'Course 1', package: 'Package 1', credits: 5, level: 'license', semester: 4 };
      const mockProfessor = { id: '1', name: 'Professor 1', email: 'some' };
      const mockRoom = { id: '1', name: 'Room 1', capacity: 10 };
      const mockGroup = { id: '1', name: 'Group 1' };

      courseService.getById.and.returnValue(of(mockCourse));
      professorService.getById.and.returnValue(of(mockProfessor));
      roomService.getById.and.returnValue(of(mockRoom));
      groupService.getById.and.returnValue(of(mockGroup));

      component.populateEventDetails();

      expect(component.filteredEvents[0].weekEvenness).toBeTrue();
      expect(component.filteredEvents[0].timeslot.startTime).toBe('08:00');
      expect(component.filteredEvents[0].timeslot.endTime).toBe('10:00');
      expect(component.filteredEvents[0].courseName).toBe('Course 1');
      expect(component.filteredEvents[0].coursePackage).toBe('Package 1');
      expect(component.filteredEvents[0].professorName).toBe('Professor 1');
      expect(component.filteredEvents[0].roomName).toBe('Room 1');
      expect(component.filteredEvents[0].group).toBe('Group 1');
    });

    it('should handle errors when fetching course details', () => {
      component.filteredEvents = [
        {
          courseId: '1',
          professorId: '1',
          roomId: '1',
          groupId: '1',
          isEven: true,
          timeslot: { time: '08:00-10:00' },
        },
      ] as any[];

      const mockError = { message: 'Request failed' };
      courseService.getById.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.populateEventDetails();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch course:', mockError);
    });

    it('should handle errors when fetching professor details', () => {
      component.filteredEvents = [
        {
          courseId: '1',
          professorId: '1',
          roomId: '1',
          groupId: '1',
          isEven: true,
          timeslot: { time: '08:00-10:00' },
        },
      ] as any[];

      const mockError = { message: 'Request failed' };
      professorService.getById.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.populateEventDetails();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch professor:', mockError);
    });

    it('should handle errors when fetching room details', () => {
      component.filteredEvents = [
        {
          courseId: '1',
          professorId: '1',
          roomId: '1',
          groupId: '1',
          isEven: true,
          timeslot: { time: '08:00-10:00' },
        },
      ] as any[];

      const mockError = { message: 'Request failed' };
      roomService.getById.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.populateEventDetails();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch room:', mockError);
    });

    it('should handle errors when fetching group details', () => {
      component.filteredEvents = [
        {
          courseId: '1',
          professorId: '1',
          roomId: '1',
          groupId: '1',
          isEven: true,
          timeslot: { time: '08:00-10:00' },
        },
      ] as any[];

      const mockError = { message: 'Request failed' };
      //groupService.getById.and.returnValue(throwError(mockError));
      //spyOn(console, 'error');

      component.populateEventDetails();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch group:', mockError);
    });
  });*/

  describe('sortEvents', () => {
    it('should sort events by day and start time', () => {
      component.filteredEvents = [
        { timeslot: { day: 'Tuesday', startTime: '10:00' } },
        { timeslot: { day: 'Monday', startTime: '09:00' } },
        { timeslot: { day: 'Monday', startTime: '08:00' } },
        { timeslot: { day: 'Wednesday', startTime: '11:00' } },
      ] as any[];

      component.sortEvents();

      expect(component.filteredEvents).toEqual([
        { timeslot: { day: 'Monday', startTime: '08:00' } },
        { timeslot: { day: 'Monday', startTime: '09:00' } },
        { timeslot: { day: 'Tuesday', startTime: '10:00' } },
        { timeslot: { day: 'Wednesday', startTime: '11:00' } },
      ]);
    });
  });

  describe('deleteTimetable', () => {
    it('should delete timetable and navigate to /timetable on success', () => {
      timetableService.delete.and.returnValue(of());
      spyOn(console, 'log');

      component.deleteTimetable('1');

      expect(timetableService.delete).toHaveBeenCalledWith('1');
      expect(component.timetable).toBeNull();
      expect(component.filteredEvents).toEqual([]);
      expect(component.errorMessage).toBeNull();
    });

    it('should handle error when deleting timetable fails', () => {
      const mockError = { message: 'Request failed' };
      timetableService.delete.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.deleteTimetable('1');

      expect(timetableService.delete).toHaveBeenCalledWith('1');
      expect(component.errorMessage).toBe('Failed to delete timetable. Please try again.');
      expect(console.error).toHaveBeenCalledWith('Server error:', mockError);
    });

    it('should not delete timetable if id is not provided', () => {
      component.deleteTimetable('');

      expect(timetableService.delete).not.toHaveBeenCalled();
    });
  });

  describe('loadConstraints', () => {
    it('should load constraints successfully', () => {
      const mockConstraints = [{ 
        id: '1',
        type: ConstraintType.HARD_NO_OVERLAP, 
        professorId: '1', 
        courseName: 'Math', 
        roomName: 'C2', 
        wantedRoomName: 'C1', 
        groupName: 'G1', 
        day: 'Monday',
        wantedDay: 'Tuesday',
        time: '08:00-10:00',
        wantedTime: '10:00-12:00',
        event: 'laboratory' 
      }, { 
        id: '2',
        type: ConstraintType.HARD_NO_OVERLAP, 
        professorId: '1', 
        courseName: 'Math', 
        roomName: 'C2', 
        wantedRoomName: 'C1', 
        groupName: 'G1', 
        day: 'Monday',
        wantedDay: 'Tuesday',
        time: '08:00-10:00',
        wantedTime: '10:00-12:00',
        event: 'laboratory' 
      }];
      constraintService.getAllForProfessor.and.returnValue(of(mockConstraints));

      component.loadConstraints();

      expect(constraintService.getAllForProfessor).toHaveBeenCalledWith('', ''); 
    });

    it('should handle errors when loading constraints', () => {
      const mockError = { message: 'Request failed' };
      constraintService.getAllForProfessor.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.loadConstraints();

      expect(console.error).toHaveBeenCalledWith('Error loading constraints: ', mockError);
    });
  });
  

  describe('handleDeleteConstraint', () => {

    it('should ensure delete call is made', () => { 
      component.constraints = [mockConstraint]; 
      component.constraintToDelete = mockConstraint; 

      constraintService.delete.and.returnValue(of()); 

      component.handleDeleteConstraint(); 

      expect(constraintService.delete).toHaveBeenCalledWith(mockConstraint.id); 
    });

    it('should handle errors when deleting constraint', () => {
      const mockError = { message: 'Request failed' };

      component.constraints = [mockConstraint];
      component.constraintToDelete = mockConstraint;
      constraintService.delete.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.handleDeleteConstraint();

      expect(console.error).toHaveBeenCalledWith('Failed to delete constraint:', mockError);
      expect(component.errorMessage).toBe('Failed to delete constraint. Please try again.');
    });
  });

  
});
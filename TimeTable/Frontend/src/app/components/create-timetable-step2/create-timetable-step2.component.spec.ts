import { TestBed, ComponentFixture } from '@angular/core/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of, throwError } from 'rxjs';
import { CreateTimetableStep2Component } from './create-timetable-step2.component';
import { TimetableService } from '../../services/timetable.service';
import { CourseService } from '../../services/course.service';
import { ProfessorService } from '../../services/professor.service';
import { GroupService } from '../../services/group.service';
import { CookieService } from 'ngx-cookie-service';

class MockRouter {
  events = of();
  navigate = jasmine.createSpy('navigate');
}

describe('CreateTimetableStep2Component', () => {
  let component: CreateTimetableStep2Component;
  let fixture: ComponentFixture<CreateTimetableStep2Component>;
  let mockRouter: MockRouter;
  let mockCookieService: jasmine.SpyObj<CookieService>;
  let mockCourseService: jasmine.SpyObj<CourseService>;
  let mockProfessorService: jasmine.SpyObj<ProfessorService>;
  let mockGroupService: jasmine.SpyObj<GroupService>;
  let mockTimetableService: jasmine.SpyObj<TimetableService>;

  beforeEach(async () => {
    mockRouter = new MockRouter();
    mockCookieService = jasmine.createSpyObj('CookieService', ['get']);
    mockCourseService = jasmine.createSpyObj('CourseService', ['getAll']);
    mockProfessorService = jasmine.createSpyObj('ProfessorService', ['getAll']);
    mockGroupService = jasmine.createSpyObj('GroupService', ['getAll']);
    mockTimetableService = jasmine.createSpyObj('TimetableService', ['create']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, CommonModule, CreateTimetableStep2Component],
      declarations: [],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: CookieService, useValue: mockCookieService },
        { provide: CourseService, useValue: mockCourseService },
        { provide: ProfessorService, useValue: mockProfessorService },
        { provide: GroupService, useValue: mockGroupService },
        { provide: TimetableService, useValue: mockTimetableService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTimetableStep2Component);
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

    it('should fetch data when a token is present', () => {
      mockCookieService.get.and.returnValue('authToken');
      mockCourseService.getAll.and.returnValue(of([]));
      mockProfessorService.getAll.and.returnValue(of([]));
      mockGroupService.getAll.and.returnValue(of([]));

      component.ngOnInit();

      expect(mockCourseService.getAll).toHaveBeenCalled();
      expect(mockProfessorService.getAll).toHaveBeenCalled();
      expect(mockGroupService.getAll).toHaveBeenCalled();
    });
  });
*/
  describe('fetchData', () => {
    it('should populate courses, professors, and groups on success', () => {
      const mockCourses = [{ userEmail: "some@gmail.com", id: '1', courseName: 'Course 1', credits: 2, package: 'compulsory', semester: 3, level: 'license'}];
      const mockProfessors = [{ userEmail: "some@gmail.com", id: '1', name: 'Professor 1', email: "prof@gmail.com" }];
      const mockGroups = [{ id: '1', name: 'Group 1' }];

      mockCourseService.getAll.and.returnValue(of(mockCourses));
      mockProfessorService.getAll.and.returnValue(of(mockProfessors));
      mockGroupService.getAll.and.returnValue(of(mockGroups));

      component.fetchData();

      expect(component.courses).toEqual(mockCourses);
      expect(component.professors).toEqual(mockProfessors);
      expect(component.groups).toEqual(mockGroups);
    });

    it('should handle errors gracefully', () => {
      mockCourseService.getAll.and.returnValue(throwError(() => new Error('Error')));
      mockProfessorService.getAll.and.returnValue(of([]));
      mockGroupService.getAll.and.returnValue(of([]));

      component.fetchData();

      expect(component.courses).toEqual([]);
      expect(component.professors).toEqual([]);
      expect(component.groups).toEqual([]);
    });
  });

  describe('addEvent', () => {
    it('should show a modal if fields are missing', () => {
      component.addEvent();
      expect(component.modalTitle).toBe('Missing Fields');
    });

    it('should add a new event if all fields are valid', () => {
      const course = { userEmail: "some@gmail.com", id: '1', courseName: 'Course 1', credits: 2, package: 'compulsory', semester: 3, level: 'license'};
      const professor = { userEmail: "some@gmail.com", id: '1', name: 'Professor 1', email: "prof@gmail.com" };
      const group = { id: '1', name: 'Group 1' };

      component.selectedCourse = course;
      component.selectedProfessor = professor;
      component.selectedGroup = group;

      component.addEvent();

      expect(component.addedEvents.length).toBe(1);
      expect(component.addedEvents[0]).toEqual({
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      });
    });

    it('should show error modal if the event already exists', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };

      component.selectedCourse =  { id: '1', courseName: 'Course 1', credits: 2, package: 'compulsory', semester: 3, level: 'license'};
      component.selectedProfessor = { id: '1', name: 'Professor 1', email: "" };
      component.selectedGroup = { id: '1', name: 'Group 1' };
      component.eventDuration = 1;
      component.eventType = 'course';

      component.addedEvents = [mockEvent];

      component.addEvent();

      expect(component.modalTitle).toBe('Event Already Exists');
      expect(component.modalMessage).toBe('This event is already added. Please modify it or remove the existing one.');
      expect(component.modalType).toBe('add');
      expect(component.isModalVisible).toBeTrue();
    });

    it('should update the event if eventToEdit is not null', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };
      const updatedEvent = {
        course: 'Course 2',
        courseId: '2',
        professor: 'Professor 2',
        professorId: '2',
        group: 'Group 2',
        groupId: '2',
        duration: 2,
        type: 'lab',
      };
      component.addedEvents = [mockEvent];
      component.selectedCourse = { id: '2', courseName: 'Course 2', credits: 3, package: 'compulsory', semester: 2, level: 'master' };
      component.selectedProfessor = { id: '2', name: 'Professor 2', email: "" };
      component.selectedGroup = { id: '2', name: 'Group 2' };
      component.eventDuration = 2;
      component.eventType = 'lab';
      component.eventToEdit = mockEvent;

      component.addEvent();

      expect(component.addedEvents).toEqual([updatedEvent]);
      expect(component.eventToEdit).toBeNull();
    });
  });

  describe('generateTimetable', () => {

    it('should log an error and return if inputValue is empty or only contains whitespace', () => {
      spyOn(console, 'error');

      component.inputValue = '   ';
      component.generateTimetable();

      expect(console.error).toHaveBeenCalledWith('Timetable name is required.');
      expect(mockTimetableService.create).not.toHaveBeenCalled();
    });

    it('should call the timetable service with correct data', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };
      component.addedEvents = [mockEvent];
      component.validatedIntervals = [
        { day: 'Monday', startTime: '08:00', endTime: '10:00', selected: false, valid: true },
      ];
      component.user = 'test-user';
      component.inputValue = 'Test Timetable';

      mockTimetableService.create.and.returnValue(of({ id: '123' }));

      component.generateTimetable();

      expect(mockTimetableService.create).toHaveBeenCalledWith({
        userEmail: 'test-user',
        name: 'Test Timetable',
        events: [
          {
            EventName: 'course',
            CourseId: '1',
            ProfessorId: '1',
            GroupId: '1',
            Duration: 1,
          },
        ],
        timeslots: [{ day: 'Monday', time: '08:00 - 10:00' }],
      });
    });

    it('should handle error when timetable creation fails', () => {
      const mockError = { message: 'Request failed' };
      mockTimetableService.create.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.inputValue = 'Test Timetable';
      component.generateTimetable();

      expect(mockTimetableService.create).toHaveBeenCalled();
      expect(component.isModalVisible).toBeTrue();
      expect(component.modalTitle).toBe('Creating error');
      expect(component.modalMessage).toBe('No valid timetable can be generated.');
      expect(component.modalType).toBe('error');
      expect(console.error).toHaveBeenCalledWith('Error generating timetable:', mockError);
      expect(component.isLoading).toBeFalse();
    });
  });

  describe('onGenerate', () => {
    it('should show modal with message when no events are added', () => {
      component.addedEvents = [];
      component.onGenerate();

      expect(component.modalTitle).toBe('No Events Added');
      expect(component.modalMessage).toBe('No events have been added. Please add events before generating the timetable.');
      expect(component.modalType).toBe('generate');
      expect(component.isModalVisible).toBeTrue();
      expect(component.isInputRequired).toBeFalse();
      expect(component.showCancelButton).toBeFalse();
    });

    it('should show modal with input when events are added', () => {
      component.addedEvents = [{
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      }];
      component.onGenerate();

      expect(component.modalTitle).toBe('Generate Timetable');
      expect(component.modalMessage).toBe('Please enter a name for the timetable:');
      expect(component.modalType).toBe('generate');
      expect(component.isModalVisible).toBeTrue();
      expect(component.isInputRequired).toBeTrue();
      expect(component.inputPlaceholder).toBe('Timetable Name');
      expect(component.showCancelButton).toBeTrue();
    });
  });

  describe('saveEventsToLocalStorage', () => {
    it('should save events to local storage if available', () => {
      spyOn(localStorage, 'setItem');
      component.addedEvents = [{
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      }];

      component.saveEventsToLocalStorage();

      expect(localStorage.setItem).toHaveBeenCalledWith('addedEvents', JSON.stringify(component.addedEvents));
    });
  });

  describe('loadEventForEdit', () => {
    it('should load event for edit and show modal', () => {
      component.courses = [{ courseName: 'Course 1', id: '1', credits: 3, package: 'compulsory', semester: 1, level: 'license' }];
      component.professors = [{ name: 'Professor 1', id: '1', email: 'professor1@example.com' }];
      component.groups = [{ name: 'Group 1', id: '1' }];
      component.eventToEdit = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };

      component.loadEventForEdit();

      expect(component.selectedCourse).toEqual(component.courses[0]);
      expect(component.selectedProfessor).toEqual(component.professors[0]);
      expect(component.selectedGroup).toEqual(component.groups[0]);
      expect(component.eventDuration).toBe(1);
      expect(component.eventType).toBe('course');
      expect(component.isModalVisible).toBeTrue();
    });

  });

  describe('deleteEvent', () => {
    it('should set eventToDelete and show modal with correct message', () => {
      const event = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };

      component.deleteEvent(event);

      expect(component.eventToDelete).toEqual(event);
      expect(component.modalTitle).toBe('Confirm Deletion');
      expect(component.modalMessage).toBe(`Are you sure you want to delete the event for ${event.course}?`);
      expect(component.modalType).toBe('delete');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('editEvent', () => {
    it('should set eventToEdit and show modal with correct message', () => {
      const event = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };

      component.editEvent(event);

      expect(component.eventToEdit).toEqual(event);
      expect(component.modalTitle).toBe('Confirm Edit');
      expect(component.modalMessage).toBe(`Are you sure you want to edit the event for ${event.course}?`);
      expect(component.modalType).toBe('edit');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('handleGenerateConfirmation', () => {
    it('should close modal if no events are added', () => {
      spyOn(component, 'closeModal');
      component.addedEvents = [];

      component.handleGenerateConfirmation();

      expect(component.closeModal).toHaveBeenCalled();
    });

    it('should set inputValue to empty string if inputValue is not provided', () => {
      component.addedEvents = [{ /* mock event data */ }];
      component.inputValue = 'Existing Value';

      component.handleGenerateConfirmation();

      expect(component.inputValue).toBe('');
    });
    /*
    it('should set inputValue to provided value', () => {
      component.addedEvents = [{ /* mock event data * / }];
      const inputValue = 'New Timetable';

      component.handleGenerateConfirmation(inputValue);

      expect(component.inputValue).toBe(inputValue);
    });*/

    it('should set isLoading to true and call generateTimetable if inputValue is provided', () => {
      spyOn(component, 'generateTimetable');
      component.addedEvents = [{ /* mock event data */ }];
      const inputValue = 'New Timetable';

      component.handleGenerateConfirmation(inputValue);

      expect(component.isLoading).toBeTrue();
      expect(component.generateTimetable).toHaveBeenCalled();
    });

    it('should not set isLoading to true or call generateTimetable if inputValue is not provided', () => {
      spyOn(component, 'generateTimetable');
      component.addedEvents = [{ /* mock event data */ }];
      component.inputValue = '';

      component.handleGenerateConfirmation();

      //expect(component.isLoading).toBeFalse();
      expect(component.generateTimetable).not.toHaveBeenCalled();
    });
  });

  describe('handleModalConfirm', () => {
    it('should close modal if event is not confirmed', () => {
      spyOn(component, 'closeModal');
      const event = { confirmed: false };

      component.handleModalConfirm(event);

      expect(component.closeModal).toHaveBeenCalled();
    });

    it('should handle delete confirmation and close modal', () => {
      spyOn(component, 'handleDeleteConfirmation');
      spyOn(component, 'closeModal');
      component.modalType = 'delete';
      const event = { confirmed: true };

      component.handleModalConfirm(event);

      expect(component.handleDeleteConfirmation).toHaveBeenCalled();
      expect(component.closeModal).toHaveBeenCalled();
    });

    it('should handle edit confirmation and close modal', () => {
      spyOn(component, 'handleEditConfirmation');
      spyOn(component, 'closeModal');
      component.modalType = 'edit';
      const event = { confirmed: true };

      component.handleModalConfirm(event);

      expect(component.handleEditConfirmation).toHaveBeenCalled();
      expect(component.closeModal).toHaveBeenCalled();
    });

    it('should handle generate confirmation with input value and close modal', () => {
      spyOn(component, 'handleGenerateConfirmation');
      spyOn(component, 'closeModal');
      component.modalType = 'generate';
      const event = { confirmed: true, inputValue: 'Timetable Name' };

      component.handleModalConfirm(event);

      expect(component.handleGenerateConfirmation).toHaveBeenCalledWith('Timetable Name');
      expect(component.closeModal).toHaveBeenCalled();
    });

    it('should close modal after handling confirmation', () => {
      spyOn(component, 'closeModal');
      const event = { confirmed: true };

      component.handleModalConfirm(event);

      expect(component.closeModal).toHaveBeenCalled();
    });
  });

  describe('closeModal', () => {
    it('should set isModalVisible to false and modalType to null', () => {
      component.isModalVisible = true;
      component.modalType = 'edit';

      component.closeModal();

      expect(component.isModalVisible).toBeFalse();
      expect(component.modalType).toBeNull();
    });
  });

  describe('handleDeleteConfirmation', () => {
    it('should delete the event and save events to local storage', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };
      component.addedEvents = [mockEvent];
      component.eventToDelete = mockEvent;
      spyOn(component, 'saveEventsToLocalStorage');

      component.handleDeleteConfirmation();

      expect(component.addedEvents).toEqual([]);
      expect(component.eventToDelete).toBeNull();
      expect(component.saveEventsToLocalStorage).toHaveBeenCalled();
    });

    it('should not delete any event if eventToDelete is null', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };
      component.addedEvents = [mockEvent];
      component.eventToDelete = null;
      spyOn(component, 'saveEventsToLocalStorage');

      component.handleDeleteConfirmation();

      expect(component.addedEvents).toEqual([mockEvent]);
      expect(component.saveEventsToLocalStorage).not.toHaveBeenCalled();
    });
  });

  describe('handleEditConfirmation', () => {
    it('should load the event for edit if eventToEdit is not null', () => {
      const mockEvent = {
        course: 'Course 1',
        courseId: '1',
        professor: 'Professor 1',
        professorId: '1',
        group: 'Group 1',
        groupId: '1',
        duration: 1,
        type: 'course',
      };
      component.eventToEdit = mockEvent;
      spyOn(component, 'loadEventForEdit');

      component.handleEditConfirmation();

      expect(component.loadEventForEdit).toHaveBeenCalled();
    });

    it('should not load any event for edit if eventToEdit is null', () => {
      component.eventToEdit = null;
      spyOn(component, 'loadEventForEdit');

      component.handleEditConfirmation();

      expect(component.loadEventForEdit).not.toHaveBeenCalled();
    });
  });

});
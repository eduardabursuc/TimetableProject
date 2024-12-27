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
  });

  describe('generateTimetable', () => {
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
  });
});
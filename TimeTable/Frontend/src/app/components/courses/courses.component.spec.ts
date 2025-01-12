import { TestBed, ComponentFixture } from '@angular/core/testing';
import { CoursesComponent } from './courses.component';
import { CourseService } from '../../services/course.service';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Course } from '../../models/course.model';

describe('CoursesComponent', () => {
  let component: CoursesComponent;
  let fixture: ComponentFixture<CoursesComponent>;
  let courseServiceSpy: jasmine.SpyObj<CourseService>;
  let cookieServiceSpy: jasmine.SpyObj<CookieService>;
  let globalsServiceSpy: jasmine.SpyObj<GlobalsService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockRouter = { 
    events: of(null), 
    navigate: jasmine.createSpy('navigate') 
  };

  beforeEach(async () => {
    const courseServiceMock = jasmine.createSpyObj('CourseService', ['getAll', 'create', 'update', 'delete']);
    const cookieServiceMock = jasmine.createSpyObj('CookieService', ['get']);
    const globalsServiceMock = jasmine.createSpyObj('GlobalsService', ['checkToken']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, CommonModule, CoursesComponent],
      providers: [
        { provide: CourseService, useValue: courseServiceMock },
        { provide: CookieService, useValue: cookieServiceMock },
        { provide: GlobalsService, useValue: globalsServiceMock },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CoursesComponent);
    component = fixture.componentInstance;
    courseServiceSpy = TestBed.inject(CourseService) as jasmine.SpyObj<CourseService>;
    cookieServiceSpy = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
    globalsServiceSpy = TestBed.inject(GlobalsService) as jasmine.SpyObj<GlobalsService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should check token and fetch courses', () => {
      cookieServiceSpy.get.and.returnValue('dummyToken');
      courseServiceSpy.getAll.and.returnValue(of([]));

      component.ngOnInit();

      expect(cookieServiceSpy.get).toHaveBeenCalledWith('authToken');
      expect(globalsServiceSpy.checkToken).toHaveBeenCalledWith('dummyToken');
      expect(component.courses).toEqual([]);
    });
    /*
    it('should redirect to login if no token is found', () => {
      cookieServiceSpy.get.and.returnValue('');
      component.ngOnInit();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    });
    */
  });

  describe('fetchCourses', () => {
    it('should fetch courses and set the courses list', () => {
      const mockCourses = [{ id: '1', courseName: 'Math', credits: 5, package: 'compulsory', semester: 1, level: 'license' }];
      courseServiceSpy.getAll.and.returnValue(of(mockCourses));

      component.fetchCourses();

      expect(component.courses).toEqual(mockCourses);
      expect(component.isLoading).toBeFalse();
    });

    it('should handle fetch error', () => {
      courseServiceSpy.getAll.and.returnValue(throwError('Error'));

      component.fetchCourses();

      expect(component.isLoading).toBeFalse();
    });
  });

  describe('isValidCourse', () => {
    it('should return true for valid course', () => {
      component.newCourse = {
        id: '1',
        courseName: 'Math',
        credits: 5,
        package: 'compulsory',
        semester: 1,
        level: 'license',
      };

      expect(component.isValidCourse()).toBeTrue();
    });

    it('should return false and show modal if course name is empty', () => {
      component.newCourse = { id: '1', courseName: '', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };

      const result = component.isValidCourse();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('Please provide valid details.');
      expect(component.modalTitle).toBe('Invalid course');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
    });

    it('should return false and show modal if course credits are 0', () => {
      component.newCourse = { id: '1', courseName: 'Course 1', credits: 0, package: 'compulsory', semester: 1, level: 'undergraduate' };

      const result = component.isValidCourse();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('Please provide valid details.');
      expect(component.modalTitle).toBe('Invalid course');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
    });

    it('should return false and show modal if course semester is 0', () => {
      component.newCourse = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 0, level: 'undergraduate' };

      const result = component.isValidCourse();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('Please provide valid details.');
      expect(component.modalTitle).toBe('Invalid course');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
    });

    it('should return false and show modal if a course with the same name already exists', () => {
      component.newCourse = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };
      component.courses = [{ id: '2', courseName: 'Course 1', credits: 4, package: 'optional', semester: 2, level: 'graduate' }];

      const result = component.isValidCourse();

      expect(result).toBeFalse();
      expect(component.modalMessage).toBe('A course with the same name already exists.');
      expect(component.modalTitle).toBe('Invalid course');
      expect(component.modalType).toBe('error');
      expect(component.isModalVisible).toBeTrue();
      expect(component.cancelOption).toBeFalse();
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
    });

    it('should return true if course is valid and no course with the same name exists', () => {
      component.newCourse = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };
      component.courses = [{ id: '2', courseName: 'Course 2', credits: 4, package: 'optional', semester: 2, level: 'graduate' }];

      const result = component.isValidCourse();

      expect(result).toBeTrue();
    });
  });

  describe('addCourse', () => {
    it('should add a valid course', () => {
      component.newCourse = { id: '', courseName: 'Math', credits: 5, package: 'compulsory', semester: 1, level: 'license' };
      courseServiceSpy.create.and.returnValue(of({ id: '1' }));

      component.addCourse();

      expect(courseServiceSpy.create).toHaveBeenCalled();
      expect(component.courses.length).toBe(1);
    });

    it('should not add an invalid course', () => {
      spyOn(component, 'isValidCourse').and.returnValue(false);

      component.addCourse();

      expect(courseServiceSpy.create).not.toHaveBeenCalled();
    });
  });

  describe('editCourse', () => {
    it('should set newCourse and isAddCase correctly', () => {
      const mockCourse: Course = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };

      component.editCourse(mockCourse);

      expect(component.newCourse).toBe(mockCourse);
      expect(component.isAddCase).toBeFalse();
    });
  });

  describe('updateCourse', () => {
    it('should update the course and update the courses list', () => {
      component.newCourse = { id: '1', courseName: 'Updated Course', credits: 4, package: 'optional', semester: 2, level: 'graduate' };
      component.courses = [{ id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' }, { id: '2', courseName: 'Course 2', credits: 4, package: 'optional', semester: 2, level: 'graduate' }];
      spyOn(component, 'isValidCourse').and.returnValue(true);
      courseServiceSpy.update.and.returnValue(of());

      component.updateCourse();

      expect(courseServiceSpy.update).toHaveBeenCalledWith('1', { id: '1', courseName: 'Updated Course', credits: 4, package: 'optional', semester: 2, level: 'graduate' });
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
      expect(component.isAddCase).toBeTrue();
    });

    it('should log an error if updating the course fails', () => {
      const mockError = { message: 'Request failed' };
      component.newCourse = { id: '1', courseName: 'Updated Course', credits: 4, package: 'optional', semester: 2, level: 'graduate' };
      spyOn(component, 'isValidCourse').and.returnValue(true);
      courseServiceSpy.update.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.updateCourse();

      expect(courseServiceSpy.update).toHaveBeenCalledWith('1', { id: '1', courseName: 'Updated Course', credits: 4, package: 'optional', semester: 2, level: 'graduate' });
      expect(console.error).toHaveBeenCalledWith('Error updating course:', mockError);
    });

    it('should not update the course if it is not valid', () => {
      component.newCourse = { id: '1', courseName: 'Updated Course', credits: 4, package: 'optional', semester: 2, level: 'graduate' };
      spyOn(component, 'isValidCourse').and.returnValue(false);

      component.updateCourse();

      expect(courseServiceSpy.update).not.toHaveBeenCalled();
    });

  });

  describe('showDeleteModal', () => {
    it('should set modal properties and make it visible', () => {
      const mockCourse: Course = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };

      component.showDeleteModal(mockCourse);

      expect(component.courseToDelete).toBe(mockCourse);
      expect(component.modalTitle).toBe('Delete course');
      expect(component.cancelOption).toBeTrue();
      expect(component.modalMessage).toBe('Are you sure you want to delete Course 1 ?');
      expect(component.modalType).toBe('delete');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('deleteCourse', () => {
    it('should delete the course and update the courses list', () => {
      component.courseToDelete = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };
      component.courses = [{ id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' }, { id: '2', courseName: 'Course 2', credits: 4, package: 'optional', semester: 2, level: 'graduate' }];
      courseServiceSpy.delete.and.returnValue(of());

      component.deleteCourse();

      expect(courseServiceSpy.delete).toHaveBeenCalledWith('1');
    });

    it('should log an error if deleting the course fails', () => {
      const mockError = { message: 'Request failed' };
      component.courseToDelete = { id: '1', courseName: 'Course 1', credits: 3, package: 'compulsory', semester: 1, level: 'undergraduate' };
      courseServiceSpy.delete.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.deleteCourse();

      expect(courseServiceSpy.delete).toHaveBeenCalledWith('1');
      expect(console.error).toHaveBeenCalledWith('Error deleting course:', mockError);
    });
  });

  describe('handleModalConfirm', () => {
    it('should call deleteCourse if modalType is delete', () => {
      component.modalType = 'delete';
      spyOn(component, 'deleteCourse');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteCourse).toHaveBeenCalled();
    });

    it('should not call deleteCourse if modalType is not delete', () => {
      component.modalType = null;
      spyOn(component, 'deleteCourse');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBeFalse();
      expect(component.deleteCourse).not.toHaveBeenCalled();
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

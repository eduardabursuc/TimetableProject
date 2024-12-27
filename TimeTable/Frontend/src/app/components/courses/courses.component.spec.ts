import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CoursesComponent } from './courses.component';
import { Router } from '@angular/router';
import { CourseService } from '../../services/course.service';
import { CookieService } from 'ngx-cookie-service';
import { of, throwError } from 'rxjs';

class MockRouter {
  events = of();
  navigate = jasmine.createSpy('navigate');
}

describe('CoursesComponent', () => {
  let component: CoursesComponent;
  let fixture: ComponentFixture<CoursesComponent>;
  let mockRouter: MockRouter;
  let mockCourseService: jasmine.SpyObj<CourseService>;
  let mockCookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    mockRouter = new MockRouter();
    mockCourseService = jasmine.createSpyObj('CourseService', ['getAll', 'create', 'update', 'delete']);
    mockCookieService = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [CoursesComponent],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: CourseService, useValue: mockCourseService },
        { provide: CookieService, useValue: mockCookieService },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CoursesComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('fetchCourses', () => {
    it('should populate courses on successful fetch', () => {
      const mockCourses = [{ id: '1', courseName: 'Course A', credits: 3, package: 'compulsory', semester: 0, level: 'license'  }];
      mockCourseService.getAll.and.returnValue(of(mockCourses));

      component.fetchCourses();

      expect(component.courses).toEqual(mockCourses);
    });

    it('should handle errors when fetching courses', () => {
      spyOn(console, 'error');
      mockCourseService.getAll.and.returnValue(throwError(() => new Error('Fetch error')));

      component.fetchCourses();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch courses:', jasmine.any(Error));
    });
  });

  describe('isValidCourse', () => {
    beforeEach(() => {
      component.courses = [{ id: '1', courseName: 'Course A', credits: 3, package: 'compulsory', semester: 0, level: 'license'  }];
    });

    it('should return false if course name is empty', () => {
      component.newCourse = { id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' };
      const result = component.isValidCourse();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe("Please provide valid details.");
    });
/*
    it('should return false if course name already exists', () => {
      component.newCourse = { id: '', courseName: 'Course A', credits: 3, package: 'compulsory', semester: 0, level: 'license'  };
      const result = component.isValidCourse();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('A course with the same name already exists.');
    });

    it('should return true for valid course', () => {
      component.newCourse = { id: '', courseName: 'Course B', credits: 4, package: 'compulsory', semester: 0, level: 'license'  };
      const result = component.isValidCourse();
      expect(result).toBe(true);
    });
*/    
  });

  describe('addCourse', () => {
    it('should not add course if validation fails', () => {
      component.newCourse = { id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' };
      spyOn(component, 'isValidCourse').and.returnValue(false);

      component.addCourse();

      expect(mockCourseService.create).not.toHaveBeenCalled();
    });

    it('should add course if validation passes', () => {
      component.newCourse = { id: '', courseName: 'Course C', credits: 3, package: 'compulsory', semester: 0, level: 'license' };
      component.user = 'mockUser';
      spyOn(component, 'isValidCourse').and.returnValue(true);
      mockCourseService.create.and.returnValue(of({ id: '2' }));

      component.addCourse();

      expect(mockCourseService.create).toHaveBeenCalledWith({
        userEmail: 'mockUser',
        courseName: 'Course C',
        credits: 3,
        package: 'compulsory',
        semester: 0,
        level: 'license'
      });
      expect(component.courses.length).toBe(1);
      expect(component.courses[0].courseName).toBe('Course C');
      expect(component.newCourse).toEqual({ id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' });
    });
  });

/*
  describe('updateCourse', () => {
    it('should not update course if validation fails', () => {
      component.newCourse = { id: '1', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license' };
      spyOn(component, 'isValidCourse').and.returnValue(false);

      component.updateCourse();

      expect(mockCourseService.update).not.toHaveBeenCalled();
    });
  });
  describe('deleteCourse', () => {
    it('should delete a course', () => {
      const courseToDelete = { id: '1', courseName: 'Course A', credits: 3, package: 'compulsory', semester: 0, level: 'license' };
      component.courses = [courseToDelete];
      component.courseToDelete = courseToDelete;
      mockCourseService.delete.and.returnValue(of());

      component.deleteCourse();

      expect(mockCourseService.delete).toHaveBeenCalledWith('1');
      expect(component.courses.length).toBe(0);
    });
  });
*/

  describe('handleModalConfirm', () => {
    it('should close modal and delete course if modal type is delete', () => {
      component.modalType = 'delete';
      component.courseToDelete = { id: '1', courseName: 'Course A', credits: 3, package: 'compulsory', semester: 0, level: 'license' };
      spyOn(component, 'deleteCourse');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBe(false);
      expect(component.deleteCourse).toHaveBeenCalled();
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

import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CourseService } from './course.service';
import { GlobalsService } from './globals.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { of } from 'rxjs';
import { Course } from '../models/course.model';

describe('CourseService', () => {
  let service: CourseService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'https://api.mock.com/v1/courses';
  const mockCourse: Course = {
    id: '1',
    courseName: 'Math 101',
    credits: 3,
    package: 'Math Package',
    semester: 1,
    level: 'Beginner'
  };

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders']);
    mockGlobalsService.apiUrl = 'https://api.mock.com';
    mockGlobalsService.getAuthHeaders.and.returnValue(new HttpHeaders({ 'Authorization': 'Bearer mock-token' }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CourseService,
        { provide: GlobalsService, useValue: mockGlobalsService }
      ]
    });

    service = TestBed.inject(CourseService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('create', () => {
    it('should create a course and return its id', () => {
      const courseData = {
        userEmail: 'user@example.com',
        courseName: 'Math 101',
        credits: 3,
        package: 'Math Package',
        semester: 1,
        level: 'Beginner'
      };

      service.create(courseData).subscribe(response => {
        expect(response.id).toBe('1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(courseData);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({ id: '1' });
    });
  });

  describe('getAll', () => {
    it('should fetch all courses for a user', () => {
      const userEmail = 'user@example.com';
      const courses: Course[] = [mockCourse];

      service.getAll(userEmail).subscribe(response => {
        expect(response).toEqual(courses);
      });

      const req = httpMock.expectOne(`${mockApiUrl}?userEmail=${userEmail}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('userEmail')).toBe(userEmail);
      req.flush(courses);
    });
  });

  describe('getById', () => {
    it('should fetch a course by id', () => {
      const courseId = '1';

      service.getById(courseId).subscribe(response => {
        expect(response).toEqual(mockCourse);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockCourse);
    });
  });

  describe('update', () => {
    it('should update a course', () => {
      const courseId = '1';
      const updatedCourse = { ...mockCourse, courseName: 'Updated Math 101' };

      service.update(courseId, updatedCourse).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedCourse);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });

  describe('delete', () => {
    it('should delete a course', () => {
      const courseId = '1';

      service.delete(courseId).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });
});

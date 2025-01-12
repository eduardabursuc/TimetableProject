import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TimetableService } from './timetable.service';
import { GlobalsService } from './globals.service';
import { Timetable } from '../models/timetable.model';
import { HttpHeaders } from '@angular/common/http';
import { create } from 'domain';

describe('TimetableService', () => {
  let service: TimetableService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'http://localhost:3000/v1/timetables';
  const mockTimetable: Timetable = {
    userEmail: 'test@example.com',
    id: '1',
    name: 'Test Timetable',
    createdAt: new Date(),
    isPublic: true,
    events: []
  };

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders']);
    mockGlobalsService.apiUrl = 'http://localhost:3000';
    mockGlobalsService.getAuthHeaders.and.returnValue(new HttpHeaders({ 'Authorization': 'Bearer mock-token' }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TimetableService,
        { provide: GlobalsService, useValue: mockGlobalsService }
      ]
    });

    service = TestBed.inject(TimetableService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('getForProfessor', () => {
    it('should get timetables for a professor', (done) => {
      const mockResponse: Timetable[] = [mockTimetable];
      const professorEmail = 'professor@example.com';

      service.getForProfessor(professorEmail).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne(`${mockApiUrl}/forProfessor?professorEmail=${professorEmail}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('create', () => {
    it('should create a timetable and return its id', () => {
      const timetableData = {
        userEmail: 'user@example.com',
        name: 'New Timetable',
        isPublic: true,
        events: [],
        createdAt: new Date(),
        timeslots: []
      };

      service.create(timetableData).subscribe(response => {
        expect(response.id).toBe('1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(timetableData);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({ id: '1' });
    });
  });

  describe('getAll', () => {
    it('should fetch all timetables for a user', () => {
      const userEmail = 'user@example.com';
      const timetables: Timetable[] = [mockTimetable];

      service.getAll(userEmail).subscribe(response => {
        expect(response).toEqual(timetables);
      });

      const req = httpMock.expectOne(`${mockApiUrl}?userEmail=${userEmail}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('userEmail')).toBe(userEmail);
      req.flush(timetables);
    });
  });

  describe('getById', () => {
    it('should fetch a timetable by id', () => {
      const timetableId = '1';

      service.getById(timetableId).subscribe(response => {
        expect(response).toEqual(mockTimetable);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockTimetable);
    });
  });

  describe('update', () => {
    it('should update a timetable', () => {
      const timetableId = '1';
      const updatedTimetable = { ...mockTimetable, name: 'Updated Timetable' };

      service.update(timetableId, updatedTimetable).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedTimetable);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });

  describe('delete', () => {
    it('should delete a timetable', () => {
      const timetableId = '1';

      service.delete(timetableId).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });
});
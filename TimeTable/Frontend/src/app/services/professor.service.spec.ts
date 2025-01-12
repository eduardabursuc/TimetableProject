import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProfessorService } from './professor.service';
import { GlobalsService } from './globals.service';
import { Professor } from '../models/professor.model';
import { HttpHeaders, HttpParams } from '@angular/common/http';

describe('ProfessorService', () => {
  let service: ProfessorService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'http://localhost:3000/v1/professors';
  const mockProfessor: Professor = {
    id: '1',
    name: 'John Doe',
    email: 'john.doe@example.com'
  };

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders']);
    mockGlobalsService.apiUrl = 'http://localhost:3000';
    mockGlobalsService.getAuthHeaders.and.returnValue(new HttpHeaders({ 'Authorization': 'Bearer mock-token' }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ProfessorService,
        { provide: GlobalsService, useValue: mockGlobalsService }
      ]
    });

    service = TestBed.inject(ProfessorService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('create', () => {
    it('should create a professor and return its id', () => {
      const professorData = {
        userEmail: 'user@example.com',
        name: 'John Doe',
        email: 'john.doe@example.com'
      };

      service.create(professorData).subscribe(response => {
        expect(response.id).toBe('1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(professorData);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({ id: '1' });
    });
  });

  describe('getAll', () => {
    it('should fetch all professors for a user', () => {
      const userEmail = 'user@example.com';
      const professors: Professor[] = [mockProfessor];

      service.getAll(userEmail).subscribe(response => {
        expect(response).toEqual(professors);
      });

      const req = httpMock.expectOne(`${mockApiUrl}?userEmail=${userEmail}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('userEmail')).toBe(userEmail);
      req.flush(professors);
    });
  });

  describe('getById', () => {
    it('should fetch a professor by id', () => {
      const professorId = '1';

      service.getById(professorId).subscribe(response => {
        expect(response).toEqual(mockProfessor);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProfessor);
    });
  });

  describe('update', () => {
    it('should update a professor', () => {
      const professorId = '1';
      const updatedProfessor = { ...mockProfessor, name: 'Updated John Doe' };

      service.update(professorId, updatedProfessor).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedProfessor);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });

  describe('delete', () => {
    it('should delete a professor', () => {
      const professorId = '1';

      service.delete(professorId).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });
});
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GroupService } from './group.service';
import { GlobalsService } from './globals.service';
import { Group } from '../models/group.model';
import { HttpHeaders, HttpParams } from '@angular/common/http';

describe('GroupService', () => {
  let service: GroupService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'http://localhost:3000/v1/groups';
  const mockGroup: Group = {
    id: '1',
    name: 'Test Group'
  };

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders']);
    mockGlobalsService.apiUrl = 'http://localhost:3000';
    mockGlobalsService.getAuthHeaders.and.returnValue(new HttpHeaders({ 'Authorization': 'Bearer mock-token' }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        GroupService,
        { provide: GlobalsService, useValue: mockGlobalsService }
      ]
    });

    service = TestBed.inject(GroupService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('create', () => {
    it('should create a group and return its id', () => {
      const groupData = {
        userEmail: 'user@example.com',
        name: 'New Group'
      };

      service.create(groupData).subscribe(response => {
        expect(response.id).toBe('1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(groupData);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({ id: '1' });
    });
  });

  describe('getAll', () => {
    it('should fetch all groups for a user', () => {
      const userEmail = 'user@example.com';
      const groups: Group[] = [mockGroup];

      service.getAll(userEmail).subscribe(response => {
        expect(response).toEqual(groups);
      });

      const req = httpMock.expectOne(`${mockApiUrl}?userEmail=${userEmail}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('userEmail')).toBe(userEmail);
      req.flush(groups);
    });
  });

  describe('getById', () => {
    it('should fetch a group by id', () => {
      const groupId = '1';

      service.getById(groupId).subscribe(response => {
        expect(response).toEqual(mockGroup);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockGroup);
    });
  });

  describe('update', () => {
    it('should update a group', () => {
      const groupId = '1';
      const updatedGroup = { ...mockGroup, name: 'Updated Group' };

      service.update(groupId, updatedGroup).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedGroup);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });

  describe('delete', () => {
    it('should delete a group', () => {
      const groupId = '1';

      service.delete(groupId).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });
});
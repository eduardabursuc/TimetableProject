import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RoomService } from './room.service';
import { GlobalsService } from './globals.service';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { Room } from '../models/room.model';

describe('RoomService', () => {
  let service: RoomService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'https://api.mock.com/v1/rooms';
  const mockRoom: Room = {
    id: '1',
    name: 'Conference Room',
    capacity: 20
  };

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders']);
    mockGlobalsService.apiUrl = 'https://api.mock.com';
    mockGlobalsService.getAuthHeaders.and.returnValue(new HttpHeaders({ 'Authorization': 'Bearer mock-token' }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        RoomService,
        { provide: GlobalsService, useValue: mockGlobalsService }
      ]
    });

    service = TestBed.inject(RoomService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('create', () => {
    it('should create a room and return its id', () => {
      const roomData = {
        userEmail: 'user@example.com',
        name: 'Conference Room',
        capacity: 20
      };

      service.create(roomData).subscribe(response => {
        expect(response.id).toBe('1');
      });

      const req = httpMock.expectOne(`${mockApiUrl}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(roomData);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({ id: '1' });
    });
  });

  describe('getAll', () => {
    it('should fetch all rooms for a user', () => {
      const userEmail = 'user@example.com';
      const rooms: Room[] = [mockRoom];

      service.getAll(userEmail).subscribe(response => {
        expect(response).toEqual(rooms);
      });

      const req = httpMock.expectOne(`${mockApiUrl}?userEmail=${userEmail}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.get('userEmail')).toBe(userEmail);
      req.flush(rooms);
    });
  });

  describe('getById', () => {
    it('should fetch a room by id', () => {
      const roomId = '1';

      service.getById(roomId).subscribe(response => {
        expect(response).toEqual(mockRoom);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockRoom);
    });
  });

  describe('update', () => {
    it('should update a room', () => {
      const roomId = '1';
      const updatedRoom = { ...mockRoom, name: 'Updated Conference Room' };

      service.update(roomId, updatedRoom).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedRoom);
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });

  describe('delete', () => {
    it('should delete a room', () => {
      const roomId = '1';

      service.delete(roomId).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/1`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBeTrue();
      req.flush({});
    });
  });
});

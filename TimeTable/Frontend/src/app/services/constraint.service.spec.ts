import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ConstraintService } from './constraint.service';
import { GlobalsService } from './globals.service';
import { Constraint, ConstraintType } from '../models/constraint.model';
import { HttpHeaders } from '@angular/common/http';

describe('ConstraintService', () => {
    let service: ConstraintService;
    let httpMock: HttpTestingController;
    let globalsServiceMock: jasmine.SpyObj<GlobalsService>;

    beforeEach(() => {
        const globalsSpy = jasmine.createSpyObj('GlobalsService', ['getAuthHeaders'], { apiUrl: 'http://example.com/api' });

        TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        providers: [
            ConstraintService,
            { provide: GlobalsService, useValue: globalsSpy },
        ],
        });

        service = TestBed.inject(ConstraintService);
        httpMock = TestBed.inject(HttpTestingController);
        globalsServiceMock = TestBed.inject(GlobalsService) as jasmine.SpyObj<GlobalsService>;
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
/*
    describe('create', () => {
        it('should send a POST request with the correct data and headers', () => {
        const mockHeaders = new HttpHeaders({ Authorization: 'Bearer token' });
        globalsServiceMock.getAuthHeaders.and.returnValue(mockHeaders);

        const data = { professorEmail: 'prof@example.com', timetableId: '123', input: 'constraint data' };
        const mockResponse = { id: '456' };

        service.create(data).subscribe((response) => {
            expect(response).toEqual(mockResponse);
        });

        const req = httpMock.expectOne('http://example.com/api/v1/constraints');
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(data);
        expect(req.request.headers.get('Authorization')).toBe('Bearer token');
        req.flush(mockResponse);
        });
    });
*/
    describe('getAllForProfessor', () => {
        it('should send a GET request with the correct parameters and return the expected constraints', () => {
        const professorEmail = 'prof@example.com';
        const timetableId = '123';
    
        const mockResponse: Constraint[] = [
            {
            id: '1',
            type: ConstraintType.SOFT_ROOM_CHANGE,
            professorId: '1',
            courseName: 'Course A',
            roomName: 'Room 101',
            wantedRoomName: 'Room 102',
            groupName: 'Group A',
            day: 'Monday',
            wantedDay: 'Tuesday',
            time: '10:00 - 12:00',
            wantedTime: '11:00 - 13:00',
            event: 'Lecture',
            },
            {
            id: '2',
            type: ConstraintType.SOFT_ROOM_PREFERENCE,
            professorId: '2',
            courseName: 'Course B',
            roomName: 'Room 201',
            wantedRoomName: 'Room 202',
            groupName: 'Group B',
            day: 'Wednesday',
            wantedDay: 'Thursday',
            time: '13:00 - 15:00',
            wantedTime: '14:00 - 16:00',
            event: 'Lab',
            },
        ];
    
        service.getAllForProfessor(professorEmail, timetableId).subscribe((response) => {
            expect(response).toEqual(mockResponse);
        });
    
        const req = httpMock.expectOne((request) =>
            request.url === 'http://example.com/api/v1/constraints/forProfessor' &&
            request.params.has('professorEmail') &&
            request.params.get('professorEmail') === professorEmail &&
            request.params.has('timetableId') &&
            request.params.get('timetableId') === timetableId
        );
    
        expect(req.request.method).toBe('GET');
        req.flush(mockResponse);
        });
    });
    
    describe('delete', () => {
        it('should send a DELETE request with the correct headers', () => {
        const mockHeaders = new HttpHeaders({ Authorization: 'Bearer token' });
        globalsServiceMock.getAuthHeaders.and.returnValue(mockHeaders);
    
        const id = '123';
    
        service.delete(id).subscribe((response) => {
            // Expect the response to be null as it matches Angular's DELETE behavior
            expect(response).toBeNull();
        });
    
        const req = httpMock.expectOne(`http://example.com/api/v1/constraints/${id}`);
        expect(req.request.method).toBe('DELETE');
        expect(req.request.headers.get('Authorization')).toBe('Bearer token');
        req.flush(null); // Mock DELETE requests usually return null
        });
    });
});

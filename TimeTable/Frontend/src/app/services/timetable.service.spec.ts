import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TimetableService } from './timetable.service';

describe('TimetableService', () => {
  let service: TimetableService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(TimetableService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // it('should create a new timetable', () => {
  //   const mockTimetable = { Events: [{ id: '1', timeslots: [] }] };
  //   const mockResponse = { id: '1' };

  //   service.create(mockTimetable).subscribe((response) => {
  //     expect(response).toEqual(mockResponse);
  //   });

  //   const req = httpMock.expectOne('/api/timetables');
  //   expect(req.request.method).toBe('POST');
  //   req.flush(mockResponse);
  // });
});
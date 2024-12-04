import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DetailComponent } from './detail.component';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TimetableService } from '../../../services/timetable.service';

describe('DetailComponent', () => {
  let component: DetailComponent;
  let fixture: ComponentFixture<DetailComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getById']);

    await TestBed.configureTestingModule({
      imports: [
        DetailComponent
      ],
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: { params: of({ id: '123' }) }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DetailComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch timetable by ID', () => {
    const mockTimetable = { id: '123', timeslots: [] };
    timetableService.getById.and.returnValue(of(mockTimetable));

    component.getTimetableById('123');

    expect(timetableService.getById).toHaveBeenCalledWith('123');
    expect(component.timetable).toEqual(mockTimetable);
  });

  it('should handle error when fetching timetable by ID', () => {
    const error = new Error('Failed to fetch timetable');
    timetableService.getById.and.returnValue(throwError(() => error));

    component.getTimetableById('123');

    expect(timetableService.getById).toHaveBeenCalledWith('123');
    expect(component.errorMessage).toBe('Failed to fetch details for ID: 123.');
  });
});

// import { ComponentFixture, TestBed } from '@angular/core/testing';
// import { TimetableComponent } from '../../../app/components/timetable/timetable.component';
// import { TimetableService } from '../../services/timetable.service';
// import { of, throwError } from 'rxjs';
// import { HttpClientTestingModule } from '@angular/common/http/testing';
// import { Timetable } from '../../models/timetable.model';
// import { ActivatedRoute } from '@angular/router';
// import { RouterTestingModule } from '@angular/router/testing';

// describe('TimetableComponent', () => {
//   let component: TimetableComponent;
//   let fixture: ComponentFixture<TimetableComponent>;
//   let timetableService: jasmine.SpyObj<TimetableService>;

//   beforeEach(async () => {
//     const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getAll']);

//     await TestBed.configureTestingModule({
//       imports: [HttpClientTestingModule, RouterTestingModule, TimetableComponent], // Import TimetableComponent
//       providers: [
//         { provide: TimetableService, useValue: timetableServiceSpy },
//         { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '1' } } } }
//       ]
//     }).compileComponents();

//     fixture = TestBed.createComponent(TimetableComponent);
//     component = fixture.componentInstance;
//     timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
//     fixture.detectChanges();
//   });

//   it('should create', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should fetch all timetables', () => {
//     const mockTimetables: Timetable[] = [{ id: '1', timeslots: [] }, { id: '2', timeslots: [] }];
//     timetableService.getAll.and.returnValue(of(mockTimetables));

//     component.fetchAllTimetables();

//     expect(timetableService.getAll).toHaveBeenCalled();
//     expect(component.timetables).toEqual(mockTimetables);
//   });

//   it('should handle error when fetching all timetables', () => {
//     const error = new Error('Failed to fetch timetables');
//     timetableService.getAll.and.returnValue(throwError(() => error));

//     component.fetchAllTimetables();

//     expect(timetableService.getAll).toHaveBeenCalled();
//     expect(component.timetables).toEqual([]);
//   });

//   it('should navigate to the next timetable', () => {
//     component.timetables = [{ id: '1', timeslots: [] }, { id: '2', timeslots: [] }];
//     component.currentIndex = 0;

//     component.nextTimetable();

//     expect(component.currentIndex).toBe(1);
//   });

//   it('should navigate to the previous timetable', () => {
//     component.timetables = [{ id: '1', timeslots: [] }, { id: '2', timeslots: [] }];
//     component.currentIndex = 1;

//     component.previousTimetable();

//     expect(component.currentIndex).toBe(0);
//   });
//});
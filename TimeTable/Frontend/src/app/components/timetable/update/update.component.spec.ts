import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateComponent } from './update.component';
import { TimetableService } from '../../../services/timetable.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('UpdateComponent', () => {
  let component: UpdateComponent;
  let fixture: ComponentFixture<UpdateComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['update']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, UpdateComponent], // Import UpdateComponent instead of declaring
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;

    // Spy on the updateResult.emit method
    spyOn(component.updateResult, 'emit');
    
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // it('should call timetableService.update and emit success message', () => {
  //   const mockTimetable = { id: '1', timeslots: [] };
  //   component.timetable = mockTimetable;
  //   timetableService.update.and.returnValue(of(void 0));

  //   component.updateTimetable();

  //   expect(timetableService.update).toHaveBeenCalledWith('1', mockTimetable);
  //   expect(component.updateResult.emit).toHaveBeenCalledWith('Timetable updated successfully!');
  // });
});
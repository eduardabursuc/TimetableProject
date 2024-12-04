import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateComponent } from './create.component';
import { TimetableService } from '../../../services/timetable.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('CreateComponent', () => {
  let component: CreateComponent;
  let fixture: ComponentFixture<CreateComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['create']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, CreateComponent],
      providers: [
        provideHttpClient(),
        { provide: TimetableService, useValue: timetableServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should display error message for invalid JSON', () => {
    component.jsonInput = 'invalid json';
    component.createTimetable();
    expect(component.errorMessage).toBe('Invalid JSON format. Please provide valid JSON.');
  });

  it('should display error message for invalid input format', () => {
    component.jsonInput = '{}';
    component.createTimetable();
    expect(component.errorMessage).toBe('Invalid input format or event data.');
  });

  it('should call timetableService.create and navigate on success', () => {
    const validInput = '{"Events": [{"Group": "A", "EventName": "Test Event", "CourseName": "Test Course", "ProfessorId": "123", "WeekEvenness": true, "ProfessorName": "Prof. Test", "CourseCredits": 3, "CoursePackage": "Test Package"}]}';
    component.jsonInput = validInput;
    timetableService.create.and.returnValue(of({ id: '123' }));

    component.createTimetable();

    expect(timetableService.create).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/detail', { id: '123' }]);
  });

  it('should display error message on service error', () => {
    const validInput = '{"Events": [{"Group": "A", "EventName": "Test Event", "CourseName": "Test Course", "ProfessorId": "123", "WeekEvenness": true, "ProfessorName": "Prof. Test", "CourseCredits": 3, "CoursePackage": "Test Package"}]}';
    component.jsonInput = validInput;
    timetableService.create.and.returnValue(throwError(() => new Error('Service error')));

    component.createTimetable();

    expect(component.errorMessage).toBe('Failed to create timetable. Please try again.');
  });
});
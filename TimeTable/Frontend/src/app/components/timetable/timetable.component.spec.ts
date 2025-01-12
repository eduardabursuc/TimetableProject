import { TestBed, ComponentFixture } from '@angular/core/testing';
import { TimetableComponent } from './timetable.component';
import { TimetableService } from '../../services/timetable.service';
import { Router } from '@angular/router';
import { CookieService } from "ngx-cookie-service";
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('TimetableComponent', () => {
  let component: TimetableComponent;
  let fixture: ComponentFixture<TimetableComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;
  let router: jasmine.SpyObj<Router>;
  let cookieService: jasmine.SpyObj<CookieService>;

  const mockRouter = { 
    events: of(null), 
    navigate: jasmine.createSpy('navigate') 
  };

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getAll', 'getForProfessor']);
    const cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule,TimetableComponent ],
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy },
        { provide: Router, useValue: mockRouter },
        { provide: CookieService, useValue: cookieServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TimetableComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    cookieService = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to login if no auth token is present', () => {
    cookieService.get.and.returnValue('');
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should fetch all timetables if user is admin', () => {
    cookieService.get.and.returnValue('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RAZ21haWwuY29tIiwicm9sZSI6ImFkbWluIiwibmJmIjoxNzM2NTk4MzM0LCJleHAiOjE3MzY2ODQ3MzQsImlhdCI6MTczNjU5ODMzNH0.SRp2duRGuEJU3sCqe1SSeNpsFA3HDRLhH0bUxZ4o3Ds');
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'user') return 'adminUser';
      if (key === 'role') return 'admin';
      return null;
    });
    spyOn(component, 'fetchAllTimetables');

    component.ngOnInit();

    expect(component.user).toBe('adminUser');
    expect(component.fetchAllTimetables).toHaveBeenCalled();
  });

  it('should fetch all timetables for professor if user is professor', () => {
    cookieService.get.and.returnValue('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RAZ21haWwuY29tIiwicm9sZSI6ImFkbWluIiwibmJmIjoxNzM2NTk4MzM0LCJleHAiOjE3MzY2ODQ3MzQsImlhdCI6MTczNjU5ODMzNH0.SRp2duRGuEJU3sCqe1SSeNpsFA3HDRLhH0bUxZ4o3Ds');
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'user') return 'professorUser';
      if (key === 'role') return 'professor';
      return null;
    });
    spyOn(component, 'fetchAllByProfessor');

    component.ngOnInit();

    expect(component.user).toBe('professorUser');
    expect(component.fetchAllByProfessor).toHaveBeenCalled();
  });

  it('should not fetch timetables if role is not admin or professor', () => {
    cookieService.get.and.returnValue('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RAZ21haWwuY29tIiwicm9sZSI6ImFkbWluIiwibmJmIjoxNzM2NTk4MzM0LCJleHAiOjE3MzY2ODQ3MzQsImlhdCI6MTczNjU5ODMzNH0.SRp2duRGuEJU3sCqe1SSeNpsFA3HDRLhH0bUxZ4o3Ds');
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'user') return 'someUser';
      if (key === 'role') return 'student';
      return null;
    });
    spyOn(component, 'fetchAllTimetables');
    spyOn(component, 'fetchAllByProfessor');

    component.ngOnInit();

    expect(component.user).toBe('someUser');
    expect(component.fetchAllTimetables).not.toHaveBeenCalled();
    expect(component.fetchAllByProfessor).not.toHaveBeenCalled();
  });

  describe('fetchAllByProfessor', () => {
    it('should fetch timetables for professor and sort them by createdAt', () => {
      const mockTimetables = [
        { id: '1', createdAt: new Date('2023-02-01T00:00:00Z'), name: 'Timetable 1', events: [], isPublic: true, userEmail: 'some' },
        { id: '2', createdAt: new Date('2023-01-01T00:00:00Z'), name: 'Timetable 2', events: [], isPublic: true, userEmail: 'some' },
      ];
      timetableService.getForProfessor.and.returnValue(of(mockTimetables));

      component.fetchAllByProfessor();

      expect(timetableService.getForProfessor).toHaveBeenCalledWith(component.user);
      expect(component.timetables).toEqual([
        { id: '1', createdAt: new Date('2023-02-01T00:00:00Z'), name: 'Timetable 1', events: [], isPublic: true, userEmail: 'some' },
        { id: '2', createdAt: new Date('2023-01-01T00:00:00Z'), name: 'Timetable 2', events: [], isPublic: true, userEmail: 'some' },
      ]);
      expect(component.isLoading).toBeFalse();
    });

    it('should handle error when fetching timetables fails', () => {
      const mockError = { message: 'Request failed' };
      timetableService.getForProfessor.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.fetchAllByProfessor();

      expect(timetableService.getForProfessor).toHaveBeenCalledWith(component.user);
      expect(console.error).toHaveBeenCalledWith('Failed to fetch timetables:', mockError);
      expect(component.isLoading).toBeFalse();
    });
  });

  it('should navigate to timetable details', () => {
    const timetableId = '123';
    component.navigateToDetails(timetableId);
    expect(router.navigate).toHaveBeenCalledWith([`/detail/${timetableId}`]);
  });

  it('should navigate to create timetable page', () => {
    component.generateTimetable();
    expect(router.navigate).toHaveBeenCalledWith([`/create-timetable-step1`]);
  });

  it('should determine visibility based on role', () => {
    localStorage.setItem('role', 'admin');
    expect(component.isVisible()).toBeTrue();

    localStorage.setItem('role', 'professor');
    expect(component.isVisible()).toBeFalse();
  });

  it('should handle errors during timetable fetch', () => {
    timetableService.getAll.and.returnValue(throwError(() => new Error('Fetch error')));
    component.fetchAllTimetables();
    expect(component.timetables.length).toBe(0);
  });

  describe('previousPage', () => {
    it('should decrement currentPage if it is greater than 0', () => {
      component.currentPage = 1;

      component.previousPage();

      expect(component.currentPage).toBe(0);
    });

    it('should not decrement currentPage if it is 0', () => {
      component.currentPage = 0;

      component.previousPage();

      expect(component.currentPage).toBe(0);
    });
  });

  describe('totalPages', () => {
    it('should return the correct number of total pages', () => {
      component.timetables = new Array(15); // 15 timetables
      component.pageSize = 5;

      expect(component.totalPages).toBe(3);
    });
  });

  describe('paginatedTimetables', () => {
    it('should return the correct paginated timetables', () => {
      component.timetables = [
        { id: '1', createdAt: new Date('2023-01-01T00:00:00Z'), name: 'Timetable 1', events: [], isPublic: true, userEmail: 'some' },
        { id: '2', createdAt: new Date('2023-01-02T00:00:00Z'), name: 'Timetable 2', events: [], isPublic: true, userEmail: 'some' },
        { id: '3', createdAt: new Date('2023-01-03T00:00:00Z'), name: 'Timetable 3', events: [], isPublic: true, userEmail: 'some' },
        { id: '4', createdAt: new Date('2023-01-04T00:00:00Z'), name: 'Timetable 4', events: [], isPublic: true, userEmail: 'some' },
        { id: '5', createdAt: new Date('2023-01-05T00:00:00Z'), name: 'Timetable 5', events: [], isPublic: true, userEmail: 'some' },
        { id: '6', createdAt: new Date('2023-01-06T00:00:00Z'), name: 'Timetable 6', events: [], isPublic: true, userEmail: 'some' },
      ];
      component.pageSize = 2;
      component.currentPage = 1;

      expect(component.paginatedTimetables).toEqual([
        { id: '3', createdAt: new Date('2023-01-03T00:00:00Z'), name: 'Timetable 3', events: [], isPublic: true, userEmail: 'some' },
        { id: '4', createdAt: new Date('2023-01-04T00:00:00Z'), name: 'Timetable 4', events: [], isPublic: true, userEmail: 'some' },
      ]);
    });
  });

  describe('fetchAllTimetables', () => {
    it('should fetch all timetables and sort them by createdAt', () => {
      const mockTimetables = [
        { id: '1', createdAt: new Date('2023-01-01T00:00:00Z'), name: 'Timetable 1', events: [], isPublic: true, userEmail: 'some' },
        { id: '2', createdAt: new Date('2023-02-01T00:00:00Z'), name: 'Timetable 2', events: [], isPublic: true, userEmail: 'some' },
      ];
      timetableService.getAll.and.returnValue(of(mockTimetables));

      component.fetchAllTimetables();

      expect(timetableService.getAll).toHaveBeenCalledWith(component.user);
      expect(component.timetables).toEqual([
        { id: '2', createdAt: new Date('2023-02-01T00:00:00Z'), name: 'Timetable 2', events: [], isPublic: true, userEmail: 'some' },
        { id: '1', createdAt: new Date('2023-01-01T00:00:00Z'), name: 'Timetable 1', events: [], isPublic: true, userEmail: 'some' },
      ]);
      expect(component.isLoading).toBeFalse();
    });

    it('should handle error when fetching timetables fails', () => {
      const mockError = { message: 'Request failed' };
      timetableService.getAll.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.fetchAllTimetables();

      expect(timetableService.getAll).toHaveBeenCalledWith(component.user);
      expect(console.error).toHaveBeenCalledWith('Failed to fetch timetables:', mockError);
      expect(component.isLoading).toBeFalse();
    });
  });

});
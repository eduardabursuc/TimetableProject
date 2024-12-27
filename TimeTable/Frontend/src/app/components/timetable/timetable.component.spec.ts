import { TestBed, ComponentFixture } from '@angular/core/testing';
import { TimetableComponent } from './timetable.component';
import { TimetableService } from '../../services/timetable.service';
import { Router } from '@angular/router';
import { CookieService } from "ngx-cookie-service";
import { throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('TimetableComponent', () => {
  let component: TimetableComponent;
  let fixture: ComponentFixture<TimetableComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;
  let router: jasmine.SpyObj<Router>;
  let cookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getAll', 'getForProfessor']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule,TimetableComponent ],
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy },
        { provide: Router, useValue: routerSpy },
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
/*
  it('should fetch all timetables for admin role', () => {
    cookieService.get.and.returnValue('token');
    localStorage.setItem('role', 'admin');
    timetableService.getAll.and.returnValue(of([{ createdAt: '2023-12-20T12:00:00Z' }]));

    component.ngOnInit();
    expect(timetableService.getAll).toHaveBeenCalled();
    expect(component.timetables.length).toBe(1);
  });

  it('should fetch professor-specific timetables for professor role', () => {
    cookieService.get.and.returnValue('token');
    localStorage.setItem('role', 'professor');
    timetableService.getForProfessor.and.returnValue(of([{ createdAt: '2023-12-20T12:00:00Z' }]));

    component.ngOnInit();
    expect(timetableService.getForProfessor).toHaveBeenCalled();
    expect(component.timetables.length).toBe(1);
  });

  it('should handle pagination correctly', () => {
    component.timetables = new Array(10).fill({}).map((_, index) => ({ id: index, createdAt: '' }));
    component.pageSize = 4;

    expect(component.totalPages).toBe(3);

    component.nextPage();
    expect(component.currentPage).toBe(1);

    component.previousPage();
    expect(component.currentPage).toBe(0);
  });
*/
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
});

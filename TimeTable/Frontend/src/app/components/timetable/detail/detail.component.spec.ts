import { TestBed, ComponentFixture } from '@angular/core/testing';
import { DetailComponent } from './detail.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service';
import { TimetableService } from '../../../services/timetable.service';
import { CourseService } from '../../../services/course.service';
import { ProfessorService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { GroupService } from '../../../services/group.service';
import { Course } from '../../../models/course.model';
import { Professor } from '../../../models/professor.model';
import { Room } from '../../../models/room.model';
import { Group } from '../../../models/group.model';
import { of, throwError } from 'rxjs';

describe('DetailComponent', () => {
  let component: DetailComponent;
  let fixture: ComponentFixture<DetailComponent>;
  let timetableService: jasmine.SpyObj<TimetableService>;
  let courseService: jasmine.SpyObj<CourseService>;
  let professorService: jasmine.SpyObj<ProfessorService>;
  let roomService: jasmine.SpyObj<RoomService>;
  let groupService: jasmine.SpyObj<GroupService>;
  let cookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    const timetableServiceSpy = jasmine.createSpyObj('TimetableService', ['getById', 'delete', 'update']);
    const courseServiceSpy = jasmine.createSpyObj('CourseService', ['getAll', 'getById']);
    const professorServiceSpy = jasmine.createSpyObj('ProfessorService', ['getAll', 'getById']);
    const roomServiceSpy = jasmine.createSpyObj('RoomService', ['getAll', 'getById']);
    const groupServiceSpy = jasmine.createSpyObj('GroupService', ['getAll', 'getById']);
    const cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule, FormsModule, DetailComponent],
      providers: [
        { provide: TimetableService, useValue: timetableServiceSpy },
        { provide: CourseService, useValue: courseServiceSpy },
        { provide: ProfessorService, useValue: professorServiceSpy },
        { provide: RoomService, useValue: roomServiceSpy },
        { provide: GroupService, useValue: groupServiceSpy },
        { provide: CookieService, useValue: cookieServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DetailComponent);
    component = fixture.componentInstance;
    timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;
    courseService = TestBed.inject(CourseService) as jasmine.SpyObj<CourseService>;
    professorService = TestBed.inject(ProfessorService) as jasmine.SpyObj<ProfessorService>;
    roomService = TestBed.inject(RoomService) as jasmine.SpyObj<RoomService>;
    groupService = TestBed.inject(GroupService) as jasmine.SpyObj<GroupService>;
    cookieService = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
/*
  it('should fetch timetable data by ID on initialization', () => {
    const timetableMock = { id: '1', events: [] } as any;
    timetableService.getById.and.returnValue(of(timetableMock));
  
    component.getTimetableById('1'); // Call method
  
    expect(timetableService.getById).toHaveBeenCalledWith('1');
    expect(component.timetable).toEqual(timetableMock);
    expect(component.errorMessage).toBeUndefined(); // Check no errors occurred
  });
 */ 

  it('should handle error when fetching timetable fails', () => {
    timetableService.getById.and.returnValue(throwError('Error fetching timetable'));
    component.getTimetableById('1');
    expect(component.errorMessage).toContain('Failed to fetch details for ID: 1.');
  });

  it('should toggle edit mode', () => {
    component.isEditMode = false;
    component.toggleEditMode();
    expect(component.isEditMode).toBeTrue();
  });  

  it('should fetch related data during initialization', () => {
    const mockCourse: Course[] = [{ id: '1', courseName: 'Sample', package: 'compulsory', credits: 5, level: 'license', semester: 4 }];
    const mockProfessor: Professor[] = [{ id: '1', name: 'Sample', email: "some@gmail.com" }];
    const mockGroup: Group[] = [{ id: '1', name: 'Sample' }];
    const mockRoom: Room[] = [{ id: '1', name: 'Sample', capacity: 10 }];

    courseService.getAll.and.returnValue(of(mockCourse));
    professorService.getAll.and.returnValue(of(mockProfessor));
    groupService.getAll.and.returnValue(of(mockGroup));
    roomService.getAll.and.returnValue(of(mockRoom));

    component.fetchData();

    expect(component.courses).toEqual(mockCourse);
    expect(component.professors).toEqual(mockProfessor);
    expect(component.groups).toEqual(mockGroup);
    expect(component.rooms).toEqual(mockRoom);
  });

});
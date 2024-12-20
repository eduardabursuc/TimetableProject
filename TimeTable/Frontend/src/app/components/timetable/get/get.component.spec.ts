import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GetComponent } from './get.component';
import { TimetableService } from '../../../services/timetable.service';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Timetable } from '../../../models/timetable.model';

describe('GetComponent', () => {
    let component: GetComponent;
    let fixture: ComponentFixture<GetComponent>;
    let timetableService: jasmine.SpyObj<TimetableService>;

    beforeEach(async () => {
        const timetableServiceSpy = jasmine.createSpyObj('TimetableService', [
            'getPaginated', 
            'getByRoom', 
            'getByGroup', 
            'getByProfessor', 
            'getAll'
        ]);

        await TestBed.configureTestingModule({
            imports: [HttpClientTestingModule, GetComponent], // Import GetComponent instead of declaring
            providers: [
                { provide: TimetableService, useValue: timetableServiceSpy }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(GetComponent);
        component = fixture.componentInstance;
        timetableService = TestBed.inject(TimetableService) as jasmine.SpyObj<TimetableService>;

        // Mock EventEmitter methods
        spyOn(component.allTimetablesFetched, 'emit');
        spyOn(component.errorOccurred, 'emit');
        spyOn(component.timetableSelected, 'emit');

        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should handle error when fetching all timetables', () => {
        const error = new Error('Failed to fetch timetables');
        timetableService.getAll.and.returnValue(throwError(() => error));

        component.fetchAllTimetables();

        expect(timetableService.getAll).toHaveBeenCalled();
        expect(component.errorOccurred.emit).toHaveBeenCalledWith('Failed to fetch all timetables.');
    });


    it('should handle error when fetching timetable by room', () => {
        const error = new Error('Failed to fetch timetable');
        timetableService.getByRoom.and.returnValue(throwError(() => error));

        component.fetchTimetableByRoom('1', 'Room A');

        expect(timetableService.getByRoom).toHaveBeenCalledWith('1', 'Room A');
        expect(component.errorOccurred.emit).toHaveBeenCalledWith('Failed to fetch timetable for room: Room A.');
    });

    it('should handle error when fetching timetable by group', () => {
        const error = new Error('Failed to fetch timetable');
        timetableService.getByGroup.and.returnValue(throwError(() => error));

        component.fetchTimetableByGroup('1', 'Group A');

        expect(timetableService.getByGroup).toHaveBeenCalledWith('1', 'Group A');
        expect(component.errorOccurred.emit).toHaveBeenCalledWith('Failed to fetch timetable for group: Group A.');
    });

    it('should handle error when fetching timetable by professor', () => {
        const error = new Error('Failed to fetch timetable');
        timetableService.getByProfessor.and.returnValue(throwError(() => error));

        component.fetchTimetableByProfessor('1', '123');

        expect(timetableService.getByProfessor).toHaveBeenCalledWith('1', '123');
        expect(component.errorOccurred.emit).toHaveBeenCalledWith('Failed to fetch timetable for professor ID: 123.');
    });

});
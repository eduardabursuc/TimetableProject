import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CreateTimetableStep1Component } from './create-timetable-step1.component';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { of } from 'rxjs';
import { DayInterval } from '../../models/day-interval.model';
import { mock } from 'node:test';
import { ChangeDetectorRef } from '@angular/core';

describe('CreateTimetableStep1Component', () => {
  let component: CreateTimetableStep1Component;
  let fixture: ComponentFixture<CreateTimetableStep1Component>;
  let router: jasmine.SpyObj<Router>;
  let cookieServiceSpy: jasmine.SpyObj<CookieService>;
  let globalsServiceSpy: jasmine.SpyObj<GlobalsService>;
  let cdr: jasmine.SpyObj<ChangeDetectorRef>;
  
  const mockRouter = { 
      events: of(null), 
      navigate: jasmine.createSpy('navigate') 
    };

  beforeEach(async () => {
    const cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);
    globalsServiceSpy = jasmine.createSpyObj('GlobalsService', ['checkToken']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, CommonModule, CreateTimetableStep1Component],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: CookieService, useValue: cookieServiceSpy },
        { provide: GlobalsService, useValue: globalsServiceSpy },
        { provide: Router, useValue: mockRouter },
        { provide: ChangeDetectorRef, useValue: cdrSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTimetableStep1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    cdr = TestBed.inject(ChangeDetectorRef) as jasmine.SpyObj<ChangeDetectorRef>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
/*
  it('should redirect to login if no token is found', () => {
    cookieServiceSpy.get.and.returnValue('');
    component.ngOnInit();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
*/
  it('should call loadValidatedIntervals if token exists', () => {
    spyOn(component, 'loadValidatedIntervals');
    cookieServiceSpy.get.and.returnValue('validToken');
    component.ngOnInit();
    expect(component.loadValidatedIntervals).toHaveBeenCalled();
  });

  it('should toggle selected day', () => {
    const day = component.days[0];
    component.selectDay(day);
    expect(day.selected).toBeTrue();

    component.selectDay(day);
    expect(day.selected).toBeFalse();
  });

  it('should validate time correctly', () => {
    const day = { day: 'Monday', startTime: '10:00', endTime: '12:00', selected: false, valid: false };
    expect(component.isTimeValid(day)).toBeTrue();

    day.endTime = '09:00';
    expect(component.isTimeValid(day)).toBeFalse();
  });

  it('should show delete modal', () => {
    const interval = component.days[0];
    component.showDeleteModal(interval);
    expect(component.isModalVisible).toBeTrue();
    expect(component.modalType).toBe('delete');
  });

  it('should add a valid time interval', () => {
    const day = component.days[0];
    day.startTime = '10:00';
    day.endTime = '12:00';
    day.valid = true;
    day.selected = true;

    component.addTimeInterval();
    expect(component.validatedIntervals.length).toBe(1);
    expect(day.selected).toBeFalse();
  });

  it('should handle modal confirm for delete', () => {
    const interval = { day: 'Monday', startTime: '10:00', endTime: '12:00', selected: false, valid: true };
    component.validatedIntervals = [interval];
    component.selectedInterval = interval;
    component.modalType = 'delete';

    component.handleModalConfirm({ confirmed: true });
    expect(component.validatedIntervals.length).toBe(0);
  });
/*
  it('should update localStorage when updating intervals', () => {
    spyOn(localStorage, 'setItem');
    component.validatedIntervals = [{ day: 'Monday', startTime: '10:00', endTime: '12:00', selected: false, valid: true }];
    component.updateLocalStorage();
    expect(localStorage.setItem).toHaveBeenCalledWith('timeIntervals', jasmine.any(String));
  });

  it('should load intervals from localStorage', () => {
    const mockData = JSON.stringify([{ day: 'Monday', startTime: '10:00', endTime: '12:00', valid: true }]);
    spyOn(localStorage, 'getItem').and.returnValue(mockData);

    component.loadValidatedIntervals();
    expect(component.validatedIntervals.length).toBe(1);
  });
  */

  describe('onTimeChange', () => {
    const mockDay = { day: 'Monday', startTime: '', endTime: '', valid: false, selected: true };
    const mockEvent = { target: { value: '10:00' } } as unknown as Event;

    it('should update startTime and validate the day interval', () => {
      spyOn(component, 'isTimeValid').and.returnValue(true);

      component.onTimeChange(mockDay, 'start', mockEvent);

      expect(mockDay.startTime).toBe('10:00');
      expect(component.isTimeValid).toHaveBeenCalledWith(mockDay);
      expect(mockDay.valid).toBeTrue();
    });

    it('should update endTime and validate the day interval', () => {
      spyOn(component, 'isTimeValid').and.returnValue(true);

      component.onTimeChange(mockDay, 'end', mockEvent);

      expect(mockDay.endTime).toBe('10:00');
      expect(component.isTimeValid).toHaveBeenCalledWith(mockDay);
      expect(mockDay.valid).toBeTrue();
    });
  });

  describe('showUpdateModal', () => {
    it('should set modal properties and make it visible', () => {
      const mockInterval: DayInterval = { day: 'Monday', startTime: '08:00', endTime: '10:00', valid: true, selected: true };

      component.showUpdateModal(mockInterval);

      expect(component.selectedInterval).toBe(mockInterval);
      expect(component.modalTitle).toBe('Update Time Interval');
      expect(component.modalMessage).toBe('Do you want to update the time interval for Monday?');
      expect(component.modalType).toBe('update');
      expect(component.isModalVisible).toBeTrue();
      expect(component.showCancelButton).toBeTrue();
    });
  });

  describe('addTimeInterval', () => {
    it('should show invalid time interval modal if selected day is invalid', () => {
      spyOn(component, 'getSelectedDay').and.returnValue({ day: 'Monday', startTime: '10:00', endTime: '08:00', valid: false, selected: true });

      component.addTimeInterval();

      expect(component.modalTitle).toBe('Invalid Time Interval');
      expect(component.modalMessage).toBe('Please ensure that the start time is earlier than the end time for the selected day.');
      expect(component.isModalVisible).toBeTrue();
      expect(component.showCancelButton).toBeFalse();
    });

    it('should show update existing interval modal if selected day is valid and interval already exists', () => {
      spyOn(component, 'getSelectedDay').and.returnValue({ day: 'Monday', startTime: '08:00', endTime: '10:00', valid: true, selected: true });
      component.validatedIntervals = [{ day: 'Monday', startTime: '09:00', endTime: '11:00', valid: true, selected: true }];

      component.addTimeInterval();

      expect(component.modalTitle).toBe('Update Existing Interval');
      expect(component.modalMessage).toBe('An interval already exists for Monday. Do you want to update it?');
      expect(component.isModalVisible).toBeTrue();
      expect(component.showCancelButton).toBeTrue();
      expect(component.selectedInterval).toEqual({ day: 'Monday', startTime: '08:00', endTime: '10:00', valid: true, selected: true });
      expect(component.modalType).toBe('update');
    });

    it('should add new interval if selected day is valid and interval does not exist', () => {
      spyOn(component, 'getSelectedDay').and.returnValue({ day: 'Monday', startTime: '08:00', endTime: '10:00', valid: true, selected: true });
      component.validatedIntervals = [];

      component.addTimeInterval();

      expect(component.validatedIntervals).toEqual([{ day: 'Monday', startTime: '08:00', endTime: '10:00', valid: true, selected: true }]);
      expect(component.isModalVisible).toBeFalse();
    });
  });

  describe('onBack', () => {
    it('should navigate to /timetable', () => {
      component.onBack();

      expect(router.navigate).toHaveBeenCalledWith(['/timetable']);
    });
  });

  describe('onNext', () => {
    it('should show error modal if the number of valid intervals is less than 1', () => {
      component.validatedIntervals = [];

      component.onNext();

      expect(component.modalTitle).toBe('Error');
      expect(component.modalMessage).toBe('You must add between 1 and 7 valid time intervals.');
      expect(component.isModalVisible).toBeTrue();
      expect(component.showCancelButton).toBeFalse();
    });

    it('should show error modal if the number of valid intervals is more than 7', () => {
      component.validatedIntervals = new Array(8).fill({ valid: true });

      component.onNext();

      expect(component.modalTitle).toBe('Error');
      expect(component.modalMessage).toBe('You must add between 1 and 7 valid time intervals.');
      expect(component.isModalVisible).toBeTrue();
      expect(component.showCancelButton).toBeFalse();
    });

    it('should navigate to /create-timetable-step2 if the number of valid intervals is between 1 and 7', () => {
      component.validatedIntervals = new Array(3).fill({ valid: true });

      component.onNext();

      expect(router.navigate).toHaveBeenCalledWith(['/create-timetable-step2']);
    });
  });

  describe('closeModal', () => {
    it('should set isModalVisible to false', () => {
      component.isModalVisible = true;

      component.closeModal();

      expect(component.isModalVisible).toBeFalse();
    });
  });

  describe('handleUpdate', () => {
    it('should update the existing interval and day, and call updateLocalStorage and detectChanges', () => {
      component.selectedInterval = { day: 'Monday', startTime: '08:00', endTime: '10:00', selected: true, valid: true };
      component.validatedIntervals = [{ day: 'Monday', startTime: '09:00', endTime: '11:00', selected: true, valid: true }];
      component.days = [{ day: 'Monday', startTime: '', endTime: '', selected: true, valid: false }];

      spyOn(component, 'updateLocalStorage');

      component.handleUpdate('Monday');

      expect(component.validatedIntervals).toEqual([{ day: 'Monday', startTime: '08:00', endTime: '10:00', selected: true, valid: true }]);
      expect(component.days).toEqual([{ day: 'Monday', startTime: '08:00', endTime: '10:00', selected: false, valid: true }]);
      expect(component.updateLocalStorage).toHaveBeenCalled();
      //expect(cdr.detectChanges).toHaveBeenCalled();
    });

    it('should not update anything if selectedInterval is null', () => {
      component.selectedInterval = null;
      component.validatedIntervals = [{ day: 'Monday', startTime: '09:00', endTime: '11:00', selected: true, valid: true }];
      component.days = [{ day: 'Monday', startTime: '', endTime: '', selected: true, valid: false }];

      spyOn(component, 'updateLocalStorage');

      component.handleUpdate('Monday');

      expect(component.validatedIntervals).toEqual([{ day: 'Monday', startTime: '09:00', endTime: '11:00', selected: true, valid: true }]);
      expect(component.days).toEqual([{ day: 'Monday', startTime: '', endTime: '', selected: true, valid: false }]);
      expect(component.updateLocalStorage).not.toHaveBeenCalled();
      expect(cdr.detectChanges).not.toHaveBeenCalled();
    });
  });

  describe('loadValidatedIntervals', () => {
    it('should load intervals from localStorage and update days', () => {
      const mockData = JSON.stringify([{ day: 'Monday', startTime: '10:00', endTime: '12:00', valid: true, selected: false }]);
      spyOn(localStorage, 'getItem').and.returnValue(mockData);
      spyOn(component, 'isLocalStorageAvailable').and.returnValue(true);

      component.days = [{ day: 'Monday', startTime: '', endTime: '', valid: false, selected: false }];

      component.loadValidatedIntervals();

      expect(component.validatedIntervals).toEqual([{ day: 'Monday', startTime: '10:00', endTime: '12:00', valid: true, selected: false }]);
      expect(component.days).toEqual([{ day: 'Monday', startTime: '10:00', endTime: '12:00', valid: true, selected: false }]);
    });

    it('should not update days if localStorage is not available', () => {
      spyOn(localStorage, 'getItem').and.returnValue(null);
      spyOn(component, 'isLocalStorageAvailable').and.returnValue(false);

      component.days = [{ day: 'Monday', startTime: '', endTime: '', valid: false, selected: false }];

      component.loadValidatedIntervals();

      expect(component.validatedIntervals).toEqual([]);
      expect(component.days).toEqual([{ day: 'Monday', startTime: '', endTime: '', valid: false, selected: false }]);
    });

    it('should handle invalid JSON in localStorage gracefully', () => {
      spyOn(localStorage, 'getItem').and.returnValue('invalid JSON');
      spyOn(component, 'isLocalStorageAvailable').and.returnValue(true);

      component.days = [{ day: 'Monday', startTime: '', endTime: '', valid: false, selected: false }];

      component.loadValidatedIntervals();

      expect(component.validatedIntervals).toEqual([]);
      expect(component.days).toEqual([{ day: 'Monday', startTime: '', endTime: '', valid: false, selected: false }]);
    });
  });
  
});

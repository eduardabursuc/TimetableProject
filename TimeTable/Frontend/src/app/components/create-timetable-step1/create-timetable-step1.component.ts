import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { DayInterval } from '../../models/day-interval.model';

@Component({
  selector: 'app-create-timetable-step1',
  templateUrl: './create-timetable-step1.component.html',
  styleUrls: ['./create-timetable-step1.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule, SidebarMenuComponent, GenericModalComponent]
})
export class CreateTimetableStep1Component {
  modalType: 'delete' | 'update' | null = null;

  days: DayInterval[] = [
    { day: 'Monday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Tuesday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Wednesday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Thursday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Friday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Saturday', startTime: '', endTime: '', selected: false, valid: false },
    { day: 'Sunday', startTime: '', endTime: '', selected: false, valid: false },
  ];

  selectedInterval: DayInterval | null = null;
  validatedIntervals: DayInterval[] = [];
  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  isInputRequired: boolean = false;
  showCancelButton: boolean = true;

  constructor(private router: Router, private cdr: ChangeDetectorRef) {}

  getSelectedDay(): DayInterval | undefined {
    return this.days.find(day => day.selected);
  }

  selectDay(selectedDay: DayInterval) {
    selectedDay.selected = !selectedDay.selected;
  
    if (selectedDay.selected) {
      this.days.forEach(day => {
        if (day !== selectedDay) {
          day.selected = false;
        }
      });
    }
  }

  onTimeChange(day: DayInterval, type: 'start' | 'end', event: Event) {
    const input = event.target as HTMLInputElement;
    if (type === 'start') {
      day.startTime = input.value;
    } else {
      day.endTime = input.value;
    }
    day.valid = this.isTimeValid(day);
  }

  isTimeValid(day: DayInterval): boolean {
    return day.startTime !== '' && day.endTime !== '' && day.startTime < day.endTime;
  }

  showDeleteModal(interval: DayInterval) {
    this.selectedInterval = interval;
    this.modalTitle = 'Delete Time Interval';
    this.modalMessage = `Are you sure you want to delete the time interval for ${interval.day}?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
    this.showCancelButton = true;
  }

  showUpdateModal(interval: DayInterval) {
    this.selectedInterval = interval;
    this.modalTitle = 'Update Time Interval';
    this.modalMessage = `Do you want to update the time interval for ${interval.day}?`;
    this.modalType = 'update';
    this.isModalVisible = true;
    this.showCancelButton = true;
  }

  addTimeInterval() {
    const selectedDay = this.getSelectedDay();
    if (selectedDay && !selectedDay.valid) {
      this.modalTitle = 'Invalid Time Interval';
      this.modalMessage = 'Please ensure that the start time is earlier than the end time for the selected day.';
      this.isModalVisible = true;
      this.showCancelButton = false;
    } else if (selectedDay && selectedDay.valid) {
      const existingIntervalIndex = this.validatedIntervals.findIndex(interval => interval.day === selectedDay.day);
      if (existingIntervalIndex !== -1) {
        this.modalTitle = 'Update Existing Interval';
        this.modalMessage = `An interval already exists for ${selectedDay.day}. Do you want to update it?`;
        this.isModalVisible = true;
        this.showCancelButton = true;
        this.selectedInterval = { ...selectedDay };
        this.modalType = 'update';
      } else {
        this.validatedIntervals.push({ ...selectedDay });
        selectedDay.selected = false;
        selectedDay.valid = true;
        this.updateLocalStorage();
      }
    }
  }
  

  onBack() {
    this.router.navigate(['/create-timetable-step2']);
  }

  onNext() {
    const validIntervalsCount = this.validatedIntervals.filter(interval => interval.valid).length;
    if (validIntervalsCount < 1 || validIntervalsCount > 7) {
      this.modalTitle = 'Error';
      this.modalMessage = 'You must add between 1 and 7 valid time intervals.';
      this.isModalVisible = true;
      this.showCancelButton = false;
    } else {
      this.router.navigate(['/create-timetable-step2']);
    }
  }

  closeModal() {
    this.isModalVisible = false;
  }

  ngOnInit() {
    this.loadValidatedIntervals();
  }
  
  ngDoCheck() {}

  handleModalConfirm(event: { confirmed: boolean }) {
    if (event.confirmed && this.selectedInterval) {
      const selectedDay = this.selectedInterval.day;
  
      if (this.modalType === 'delete') {
        const intervalIndex = this.validatedIntervals.findIndex(interval => interval.day === selectedDay);
        if (intervalIndex !== -1) {
          this.validatedIntervals.splice(intervalIndex, 1);
        }
  
        const deletedDay = this.days.find(day => day.day === selectedDay);
        if (deletedDay) {
          deletedDay.valid = false;
          deletedDay.startTime = '';
          deletedDay.endTime = '';
          deletedDay.selected = false;
        }
        this.updateLocalStorage();
      } else if (this.modalType === 'update') {
        const existingIntervalIndex = this.validatedIntervals.findIndex(interval => interval.day === selectedDay);
        if (existingIntervalIndex !== -1) {
          this.validatedIntervals[existingIntervalIndex] = { 
            day: this.selectedInterval.day, 
            startTime: this.selectedInterval.startTime, 
            endTime: this.selectedInterval.endTime, 
            selected: this.selectedInterval.selected, 
            valid: this.selectedInterval.valid
          };
        }
  
        const updatedDay = this.days.find(day => day.day === selectedDay);
        if (updatedDay) {
          updatedDay.startTime = this.selectedInterval.startTime;
          updatedDay.endTime = this.selectedInterval.endTime;
          updatedDay.selected = false;
          updatedDay.valid = true;
        }
  
        this.updateLocalStorage();
        this.cdr.detectChanges();
      }
    }
  
    this.isModalVisible = false;
    this.selectedInterval = null;
    this.modalType = null;
  }
  
  isLocalStorageAvailable(): boolean {
    try {
      const testKey = 'test';
      localStorage.setItem(testKey, testKey);
      localStorage.removeItem(testKey);
      return true;
    } catch (e) {
      return false;
    }
  }

  updateLocalStorage() {
    if (this.isLocalStorageAvailable()) {
      localStorage.setItem('timeIntervals', JSON.stringify(this.validatedIntervals));
    }
  }

  loadValidatedIntervals() {
    if (this.isLocalStorageAvailable()) {
      const storedIntervals = localStorage.getItem('timeIntervals');
      if (storedIntervals) {
        try {
          this.validatedIntervals = JSON.parse(storedIntervals);
          this.validatedIntervals.forEach(interval => {
            const day = this.days.find(d => d.day === interval.day);
            if (day) {
              day.valid = interval.valid;
              day.startTime = interval.startTime;
              day.endTime = interval.endTime;
            }
          });
        } catch (error) {
          // Handle error if needed
        }
      }
    }
  }
}
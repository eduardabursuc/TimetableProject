import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TimetableService } from '../../../services/timetable.service';
import { Timetable } from '../../../models/timetable.model';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent {
  @Input() timetable: Timetable | null = null;
  @Output() updateResult = new EventEmitter<string>();

  constructor(private timetableService: TimetableService) {}

  updateTimetable(): void {
    if (!this.validateTimetable(this.timetable)) {
      this.updateResult.emit('Invalid timetable data. Please check the input.');
      return;
    }

    this.timetableService.update(this.timetable!.id, this.timetable!).subscribe({
      next: () => {
        this.updateResult.emit('Timetable updated successfully!');
      },
      error: (error) => {
        this.updateResult.emit('Failed to update timetable. Please try again.');
        console.error(error);
      }
    });
  }

  private validateTimetable(timetable: Timetable | null): boolean {
    if (!timetable) {
      console.error('Timetable is null or undefined.');
      return false;
    }

    if (!timetable.id || typeof timetable.id !== 'string' || timetable.id.trim().length === 0) {
      console.error('Invalid timetable ID.');
      return false;
    }

    if (!Array.isArray(timetable.events) || timetable.events.length === 0) {
      console.error('Timetable must contain at least one timeslot.');
      return false;
    }

    for (const timeslot of timetable.events) {
      if (!this.validateTimeslot(timeslot)) {
        console.error('Invalid timeslot found in timetable:', timeslot);
        return false;
      }
    }

    return true;
  }

  private validateTimeslot(timeslot: any): boolean {
    if (
      !timeslot.day ||
      !timeslot.time ||
      !timeslot.event ||
      typeof timeslot.day !== 'string' ||
      typeof timeslot.time !== 'string'
    ) {
      console.error('Timeslot must have valid "day" and "time" values.');
      return false;
    }

    return this.validateEvent(timeslot.event);
  }

  private validateEvent(event: any): boolean {
    if (
      !event.group ||
      !event.eventName ||
      !event.courseName ||
      !event.professorId ||
      typeof event.group !== 'string' ||
      typeof event.eventName !== 'string' ||
      typeof event.courseName !== 'string' ||
      typeof event.professorId !== 'string'
    ) {
      console.error('Event must have valid "group", "eventName", "courseName", and "professorId" values.');
      return false;
    }

    return true;
  }
}
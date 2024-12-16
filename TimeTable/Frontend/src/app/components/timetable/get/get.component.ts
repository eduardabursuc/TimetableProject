import { Component, EventEmitter, Output } from '@angular/core';
import { TimetableService } from '../../../services/timetable.service';
import { Timetable } from '../../../models/timetable.model';

@Component({
  selector: 'app-get',
  templateUrl: './get.component.html',
  styleUrls: ['./get.component.css']
})
export class GetComponent {
  @Output() timetableSelected = new EventEmitter<Timetable>();
  @Output() allTimetablesFetched = new EventEmitter<Timetable[]>();
  @Output() errorOccurred = new EventEmitter<string>();

  constructor(private timetableService: TimetableService) {}

  fetchAllTimetables(): void {
    this.timetableService.getAll('').subscribe({
      next: (response) => {
        this.allTimetablesFetched.emit(response);
      },
      error: (error) => {
        this.errorOccurred.emit('Failed to fetch all timetables.');
        console.error(error);
      }
    });
  }

  fetchTimetableByRoom(id: string, roomName: string): void {
    if (!this.isValidId(id) || !this.isNonEmptyString(roomName)) {
      this.errorOccurred.emit('Invalid input: ensure both ID and Room Name are provided and valid.');
      return;
    }

    this.timetableService.getByRoom(id, roomName).subscribe({
      next: (response) => {
        this.timetableSelected.emit(response);
      },
      error: (error) => {
        this.errorOccurred.emit(`Failed to fetch timetable for room: ${roomName}.`);
        console.error(error);
      }
    });
  }

  fetchTimetableByGroup(id: string, groupName: string): void {
    if (!this.isValidId(id) || !this.isNonEmptyString(groupName)) {
      this.errorOccurred.emit('Invalid input: ensure both ID and Group Name are provided and valid.');
      return;
    }

    this.timetableService.getByGroup(id, groupName).subscribe({
      next: (response) => {
        this.timetableSelected.emit(response);
      },
      error: (error) => {
        this.errorOccurred.emit(`Failed to fetch timetable for group: ${groupName}.`);
        console.error(error);
      }
    });
  }

  fetchTimetableByProfessor(id: string, professorId: string): void {
    if (!this.isValidId(id) || !this.isValidId(professorId)) {
      this.errorOccurred.emit('Invalid input: ensure both Timetable ID and Professor ID are provided and valid.');
      return;
    }

    this.timetableService.getByProfessor(id, professorId).subscribe({
      next: (response) => {
        this.timetableSelected.emit(response);
      },
      error: (error) => {
        this.errorOccurred.emit(`Failed to fetch timetable for professor ID: ${professorId}.`);
        console.error(error);
      }
    });
  }

  fetchPaginatedTimetables(page: number, pageSize: number): void {
    if (!this.isValidPage(page) || !this.isValidPageSize(pageSize)) {
      this.errorOccurred.emit('Invalid pagination input: Page and Page Size must be positive integers.');
      return;
    }

    this.timetableService.getPaginated('', page, pageSize).subscribe({
      next: (response) => {
        this.allTimetablesFetched.emit(response);
      },
      error: (error) => {
        this.errorOccurred.emit('Failed to fetch paginated timetables.');
        console.error(error);
      }
    });
  }

  private isValidId(id: string): boolean {
    return typeof id === 'string' && id.trim().length > 0;
  }

  private isNonEmptyString(value: string): boolean {
    return typeof value === 'string' && value.trim().length > 0;
  }

  private isValidPage(page: number): boolean {
    return Number.isInteger(page) && page > 0;
  }

  private isValidPageSize(pageSize: number): boolean {
    return Number.isInteger(pageSize) && pageSize > 0;
  }
}
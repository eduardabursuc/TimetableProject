import { Component, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CourseService } from '../../../services/course.service';
import { ProfessorsService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { TimetableService } from '../../../services/timetable.service';
import { GroupService } from '../../../services/group.service';

@Component({
  selector: 'app-update-button',
  templateUrl: './update-button.component.html',
  styleUrls: ['./update-button.component.css'],
})
export class UpdateButtonComponent {
  @Input() entity!: any; // The modified entity to be updated
  @Input() entityType!: 'Course' | 'Professor' | 'Room' | 'Timetable' | 'Group'; // The type of entity

  isLoading: boolean = false;

  constructor(
    private courseService: CourseService,
    private professorsService: ProfessorsService,
    private roomService: RoomService,
    private timetableService: TimetableService,
    private groupService: GroupService,
    private snackBar: MatSnackBar // For notifications
  ) {}

  updateEntity(): void {
    if (!this.entity) {
      this.showError('Entity data is missing. Please provide valid data.');
      return;
    }

    this.isLoading = true;

    // Determine the service to call based on the entity type
    let serviceCall;

    switch (this.entityType) {
      case 'Course':
        serviceCall = this.courseService.update(this.entity.courseName, this.entity);
        break;
      case 'Professor':
        serviceCall = this.professorsService.update(this.entity.id, this.entity);
        break;
      case 'Room':
        serviceCall = this.roomService.update(this.entity.name, this.entity);
        break;
      case 'Timetable':
        serviceCall = this.timetableService.update(this.entity.id, this.entity);
        break;
      case 'Group':
        serviceCall = this.groupService.update(this.entity.name, this.entity);
        break;
      default:
        this.showError('Invalid entity type.');
        this.isLoading = false;
        return;
    }

    // Call the service and handle the response
    serviceCall.subscribe({
      next: () => {
        this.isLoading = false;
        this.showSuccess('Entity updated successfully!');
      },
      error: (err) => {
        console.error('Error updating entity:', err);
        this.showError('Failed to update entity. Please try again.');
        this.isLoading = false;
      },
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['success-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }
}
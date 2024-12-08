import { Component, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { CourseService } from '../../../services/course.service';
import { ProfessorsService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { TimetableService } from '../../../services/timetable.service';
import { GroupService } from '../../../services/group.service';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-delete-button',
  templateUrl: './delete-button.component.html',
  styleUrls: ['./delete-button.component.css'],
})
export class DeleteButtonComponent {
  @Input() entity!: any; // The entity to be deleted
  @Input() entityType!: 'Course' | 'Professor' | 'Room' | 'Timetable' | 'Group'; // Specify the type of entity

  isLoading: boolean = false;

  constructor(
    private courseService: CourseService,
    private professorsService: ProfessorsService,
    private roomService: RoomService,
    private timetableService: TimetableService,
    private groupService: GroupService,
    private snackBar: MatSnackBar, // For notifications
    private dialog: MatDialog // For confirmation dialog
  ) {}

  deleteEntity(): void {
    if (!this.entity) {
      this.showError('Entity data is missing. Unable to delete.');
      return;
    }

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '300px',
      data: { message: 'Are you sure you want to delete this entity?' },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.performDelete();
      }
    });
  }

  private performDelete(): void {
    this.isLoading = true;

    // Determine the service to call based on the entity type
    let serviceCall;
    switch (this.entityType) {
      case 'Course':
        serviceCall = this.courseService.delete(this.entity.courseName);
        break;
      case 'Professor':
        serviceCall = this.professorsService.delete(this.entity.id);
        break;
      case 'Room':
        serviceCall = this.roomService.delete(this.entity.name);
        break;
      case 'Timetable':
        serviceCall = this.timetableService.delete("admin@gmail.com", this.entity.id);
        break;
      case 'Group':
        serviceCall = this.groupService.delete(this.entity.name);
        break;
      default:
        this.showError('Invalid entity type.');
        this.isLoading = false;
        return;
    }

    // Perform the delete operation and handle response
    serviceCall.subscribe({
      next: () => {
        this.isLoading = false;
        this.showSuccess('Entity deleted successfully!');
      },
      error: (err) => {
        console.error('Error deleting entity:', err);
        this.showError('Failed to delete entity. Please try again.');
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
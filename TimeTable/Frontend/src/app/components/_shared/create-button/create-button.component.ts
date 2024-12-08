import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar'; // Import MatSnackBar
import { CourseService } from '../../../services/course.service';
import { ProfessorsService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { TimetableService } from '../../../services/timetable.service';
import { GroupService } from '../../../services/group.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-create-button',
  templateUrl: './create-button.component.html',
  styleUrls: ['./create-button.component.css'],
})
export class CreateButtonComponent {
  @Input() entity!: any; // The entity to be created
  @Input() entityType!: 'Course' | 'Professor' | 'Room' | 'Timetable' | 'Group'; // Specify the type of entity
  @Output() entityCreated = new EventEmitter<any>(); // Emit the created entity

  isLoading: boolean = false;

  constructor(
    private courseService: CourseService,
    private professorsService: ProfessorsService,
    private roomService: RoomService,
    private timetableService: TimetableService,
    private groupService: GroupService,
    private snackBar: MatSnackBar // Inject MatSnackBar
  ) {}

  createEntity(): void {
    if (!this.entity) {
      this.showError('Entity data is missing. Please fill in the required fields.');
      return;
    }

    this.isLoading = true;

    // Determine the service to call based on the entity type
    let serviceCall: Observable<any>;

    switch (this.entityType) {
      case 'Course':
        serviceCall = this.courseService.create(this.entity);
        break;
      case 'Professor':
        serviceCall = this.professorsService.create(this.entity);
        break;
      case 'Room':
        serviceCall = this.roomService.create(this.entity);
        break;
      case 'Timetable':
        serviceCall = this.timetableService.create(this.entity);
        break;
      case 'Group':
        serviceCall = this.groupService.create(this.entity);
        break;
      default:
        this.showError('Invalid entity type.');
        this.isLoading = false;
        return;
    }

    // Call the service and handle the result
    serviceCall.subscribe({
      next: (createdEntity) => {
        this.isLoading = false;
        this.entityCreated.emit(createdEntity); // Notify parent component
        this.showSuccess('Entity created successfully!');
      },
      error: (err) => {
        console.error('Error creating entity:', err);
        this.showError('Failed to create entity. Please try again.');
        this.isLoading = false;
      },
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000, // Show for 5 seconds
      panelClass: ['error-snackbar'], // Add custom CSS class
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000, // Show for 5 seconds
      panelClass: ['success-snackbar'], // Add custom CSS class
      horizontalPosition: 'right',
      verticalPosition: 'top',
    });
  }
}
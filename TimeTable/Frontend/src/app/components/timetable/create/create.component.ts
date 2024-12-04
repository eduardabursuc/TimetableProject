import { Component } from '@angular/core';
import { TimetableService } from '../../../services/timetable.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Event } from '../../../models/event.model';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
  imports: [CommonModule, FormsModule],
})
export class CreateComponent {
  jsonInput: string = '';
  errorMessage: string | null = null;

  constructor(
    private timetableService: TimetableService,
    private router: Router
  ) {}

  createTimetable(): void {
    try {
      const parsedInput = JSON.parse(this.jsonInput);

      // Validate input format
      if (!this.validateInput(parsedInput)) {
        this.errorMessage = 'Invalid input format or event data.';
        return;
      }

      // Call the service with the parsed input
      this.timetableService.create(parsedInput).subscribe({
        next: (response) => {
          this.errorMessage = null;
          this.router.navigate(['/detail', response]);
        },
        error: (error) => {
          this.errorMessage = 'Failed to create timetable. Please try again.';
          console.error('Error during createTimetable:', error);
        },
      });
    } catch (e) {
      this.errorMessage = 'Invalid JSON format. Please provide valid JSON.';
      console.error('JSON parse error:', e);
    }
  }

  private validateInput(input: any): boolean {
    // Ensure input has the expected "Events" property and it's an array
    if (!input?.Events || !Array.isArray(input.Events)) {
      console.error('Invalid input structure: Missing "Events" array.');
      return false;
    }

    // Validate each event in the array
    return input.Events.every((event: Event) => this.validateEvent(event));
  }

  private validateEvent(event: any): boolean {
    // Validate the structure of each event
    return (
      event.Group &&
      event.EventName &&
      event.CourseName &&
      event.ProfessorId
    );
  }
}
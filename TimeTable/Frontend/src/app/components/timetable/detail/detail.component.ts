import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TimetableService } from '../../../services/timetable.service';
import { Timetable } from '../../../models/timetable.model';
import { UpdateComponent } from '../update/update.component';
import { Timeslot } from '../../../models/timeslot.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Event } from '../../../models/event.model';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  imports: [UpdateComponent, CommonModule, FormsModule],
})
export class DetailComponent implements OnInit {
  timetable: Timetable | null = null;
  filteredTimeslots: Timetable['timeslots'] = [];
  originalTimeslots: Timetable['timeslots'] = [];
  errorMessage: string | null = null;
  isEditMode: boolean = false;
  isFiltered: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private timetableService: TimetableService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = params['id'];
      if (!id) {
        this.errorMessage = 'ID is missing from the URL. Return to the previous page.';
        return;
      }
      this.getTimetableById(id);
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
  }

  getTimetableById(id: string): void {
    this.timetableService.getById(id).subscribe({
      next: (response) => {
        this.timetable = response;
        this.originalTimeslots = response.timeslots; // Save the original data
        this.filteredTimeslots = response.timeslots; // Initialize filtered data
        this.errorMessage = null;
      },
      error: (error) => {
        this.errorMessage = `Failed to fetch details for ID: ${id}.`;
        console.error(error);
      },
    });
  }

  deleteTimetable(id: string): void {
    if (!id) return;
    this.timetableService.delete(id).subscribe({
      next: () => {
        this.timetable = null;
        this.filteredTimeslots = [];
        this.errorMessage = null;
        console.log('Timetable deleted successfully.');
        this.router.navigate(['/timetable']);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete timetable. Please try again.';
        console.error(error);
      },
    });
  }

  filterByField(field: keyof Timeslot | keyof Event, value: any): void {
    if (this.isEditMode) return; // Disable filtering in edit mode
  
    this.filteredTimeslots = this.originalTimeslots.filter((timeslot) => {
      if (field in timeslot) {
        // Field exists directly in Timeslot
        return timeslot[field as keyof Timeslot] === value;
      } else if (field in timeslot.event) {
        // Field exists in the Event object
        return timeslot.event[field as keyof Event] === value;
      }
      return false; // Field not found in either Timeslot or Event
    });
  
    this.isFiltered = true;
  }

  resetFilters(): void {
    this.filteredTimeslots = [...this.originalTimeslots];
    this.isFiltered = false;
  }

  handleUpdateResult(message: string): void {
    alert(message);
    if (message === 'Timetable updated successfully!') {
      this.isEditMode = false;
      this.getTimetableById(this.timetable?.id!);
    } else {
      this.errorMessage = message;
    }
  }
}
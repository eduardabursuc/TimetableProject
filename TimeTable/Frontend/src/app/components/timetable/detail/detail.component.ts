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

  currentSortColumn: keyof Timeslot | keyof Event | null = null;
  isAscending: boolean = true;

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
        this.originalTimeslots = response.timeslots;
        this.filteredTimeslots = [...this.originalTimeslots];
        this.errorMessage = null;

        // Default sorting by "day"
        this.sortByColumn('day');
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
        this.router.navigate(['/timetable']);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete timetable. Please try again.';
        console.error(error);
      },
    });
  }

  filterByField(field: keyof Timeslot | keyof Event, value: any): void {
    if (this.isEditMode) return;

    this.filteredTimeslots = this.originalTimeslots.filter((timeslot) => {
      if (field in timeslot) {
        return timeslot[field as keyof Timeslot] === value;
      } else if (field in timeslot.event) {
        return timeslot.event[field as keyof Event] === value;
      }
      return false;
    });

    this.isFiltered = true;

    // Reapply sorting after filtering
    if (this.currentSortColumn) {
      this.sortByColumn(this.currentSortColumn);
      this.sortByColumn(this.currentSortColumn); // have to apply twice to maintain sort order
    }
  }

  resetFilters(): void {
    this.filteredTimeslots = [...this.originalTimeslots];
    this.isFiltered = false;

    // Reapply sorting after resetting filters
    if (this.currentSortColumn) {
      this.sortByColumn(this.currentSortColumn);
    }
  }

  sortByColumn(column: keyof Timeslot | keyof Event): void {
    if (this.currentSortColumn === column) {
      this.isAscending = !this.isAscending; // Toggle sort order
    } else {
      this.currentSortColumn = column;
      this.isAscending = true; // Default to ascending
    }

    if (column === 'day') {
      const dayOrder = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];
      this.filteredTimeslots.sort((a, b) => {
        const aIndex = dayOrder.indexOf(a.day);
        const bIndex = dayOrder.indexOf(b.day);

        return this.isAscending ? aIndex - bIndex : bIndex - aIndex;
      });
    } else {
      this.filteredTimeslots.sort((a, b) => {
        const aValue =
          column in a ? a[column as keyof Timeslot] : a.event[column as keyof Event];
        const bValue =
          column in b ? b[column as keyof Timeslot] : b.event[column as keyof Event];

        if (aValue < bValue) return this.isAscending ? -1 : 1;
        if (aValue > bValue) return this.isAscending ? 1 : -1;
        return 0;
      });
    }
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
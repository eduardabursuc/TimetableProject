import { Component, OnInit } from '@angular/core';
import { TimetableService } from '../../../services/timetable.service';
import { ActivatedRoute } from '@angular/router';
import { Timetable } from '../../../models/timetable.model';  // Adjust path as needed

@Component({
  selector: 'app-timetable-detail',
  templateUrl: './timetable-detail.component.html',
  styleUrls: ['./timetable-detail.component.css']
})
export class TimetableDetailComponent implements OnInit {
  timetable: Timetable | null = null;  // Store the fetched timetable
  errorMessage: string | null = null;
  isEditMode = false;  // Toggle for edit mode
  isFiltered = false;  // Toggle for filtering
  groupedEvents: { [key: string]: any[] } = {};  // Grouped events by day

  constructor(
    private timetableService: TimetableService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const timetableId = this.route.snapshot.paramMap.get('id');
    if (timetableId) {
      this.getTimetableById(timetableId);
    }
  }

  // Fetch timetable by ID and group events by day
  getTimetableById(id: string): void {
    this.timetableService.getById(id).subscribe(
      (data: Timetable) => {
        this.timetable = data;
        this.groupedEvents = this.groupEventsByDay(data.events);
      },
      (error) => {
        this.errorMessage = `Failed to fetch details for ID: ${id}.`;
      }
    );
  }

  // Group events by their 'day' field
  groupEventsByDay(events: any[]): { [key: string]: any[] } {
    return events.reduce((acc, event) => {
      if (!acc[event.day]) {
        acc[event.day] = [];
      }
      acc[event.day].push(event);
      return acc;
    }, {});
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
  }

  deleteTimetable(id: string): void {
    // Implement the delete functionality as needed
  }

  resetFilters(): void {
    this.isFiltered = false;
    // Reset filtering logic
  }

  handleModalConfirm(action: string): void {
    // Handle the modal confirmation logic here
  }
}

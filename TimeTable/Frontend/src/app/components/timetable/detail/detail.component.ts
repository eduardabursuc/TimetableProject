import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TimetableService } from '../../../services/timetable.service';
import { CourseService } from '../../../services/course.service';
import { ProfessorService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { GroupService } from '../../../services/group.service';
import { Timetable } from '../../../models/timetable.model';
import { UpdateComponent } from '../update/update.component';
import { Timeslot } from '../../../models/timeslot.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Event } from '../../../models/event.model';
import { SidebarMenuComponent } from '../../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../../generic-modal/generic-modal.component';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  imports: [UpdateComponent, CommonModule, FormsModule, SidebarMenuComponent, GenericModalComponent],
})
export class DetailComponent implements OnInit {
  timetable: Timetable | null = null;
  filteredEvents: Timetable['events'] = [];
  originalEvents: Timetable['events'] = [];
  errorMessage: string | null = null;
  isEditMode: boolean = false;
  isFiltered: boolean = false;

  groupedEvents: { [key: string]: Event[] } = {};
  sortedDays: string[] = [];

  currentSortColumn: keyof Timeslot | keyof Event | null = null;
  isAscending: boolean = true;

  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  inputValue: string = '';
  inputPlaceholder: string = '';
  isInputRequired: boolean = false;
  modalType: 'add' | 'delete' | 'edit' | null = null;
  eventToDelete: Timetable | null = null;
  eventToEdit: Timetable | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private timetableService: TimetableService,
    private courseService: CourseService,
    private professorService: ProfessorService,
    private roomService: RoomService,
    private groupService: GroupService
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
        console.log(response);
        this.originalEvents = response.events;
        this.filteredEvents = [...this.originalEvents];
        this.errorMessage = null;
  
        // Fetch additional details for each event
        this.populateEventDetails();
  
        this.groupEventsByDay();
      },
      error: (error) => {
        this.errorMessage = `Failed to fetch details for ID: ${id}.`;
        console.error(error);
      },
    });
  }
  
  // New method to populate course, professor, room, and group details for each event
  populateEventDetails(): void {
    this.filteredEvents.forEach((event) => {
      // Fetch course details
      this.courseService.getById(event.courseId).subscribe({
        next: (course) => {
          event.courseName = course.courseName;
          event.coursePackage = course.package;
        },
        error: (error) => {
          console.error('Failed to fetch course:', error);
        },
      });
  
      // Fetch professor details
      this.professorService.getById(event.professorId).subscribe({
        next: (professor) => {
          event.professorName = professor.name;
        },
        error: (error) => {
          console.error('Failed to fetch professor:', error);
        },
      });
  
      // Fetch room details
      this.roomService.getById(event.roomId).subscribe({
        next: (room) => {
          event.roomName = room.name;
        },
        error: (error) => {
          console.error('Failed to fetch room:', error);
        },
      });
  
      // Fetch group details
      this.groupService.getById(event.groupId).subscribe({
        next: (group) => {
          event.group = group.name;
        },
        error: (error) => {
          console.error('Failed to fetch group:', error);
        },
      });
    });
  }

  deleteTimetable(id: string): void {
    if (!id) return;
    this.timetableService.delete(id).subscribe({
      next: () => {
        this.timetable = null;
        this.filteredEvents = [];
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

    this.filteredEvents = this.originalEvents.filter((timeslot) => {
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
    this.filteredEvents = [...this.originalEvents];
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
      this.filteredEvents.sort((a, b) => {
        const aIndex = dayOrder.indexOf(a.day);
        const bIndex = dayOrder.indexOf(b.day);

        return this.isAscending ? aIndex - bIndex : bIndex - aIndex;
      });
    } else {
      this.filteredEvents.sort((a, b) => {
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

  handleModalConfirm(event: { confirmed: boolean; inputValue?: string }) {
    if (event.confirmed) {
      if (this.modalType === 'add') {
        // Handle add event confirmation if needed
      } else if (this.modalType === 'delete' && this.eventToDelete) {
        // Handle delete logic
      } else if (this.modalType === 'edit' && this.eventToEdit) {
        // Handle edit logic
      }
    }
    this.isModalVisible = false;
    this.modalType = null;
  }

  groupEventsByDay(): void {
    this.groupedEvents = this.filteredEvents.reduce((groups, event) => {
      const day = event.timeslot.day;
      if (!groups[day]) {
        groups[day] = [];
      }
      groups[day].push(event);
      return groups;
    }, {});

    // Sort days for display
    this.sortedDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'];
  }

}
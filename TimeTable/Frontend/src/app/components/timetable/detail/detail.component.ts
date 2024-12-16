import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { TimetableService } from '../../../services/timetable.service';
import { CourseService } from '../../../services/course.service';
import { ProfessorService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { GroupService } from '../../../services/group.service';
import { ConstraintService } from '../../../services/constraint.service';
import { Timetable } from '../../../models/timetable.model';
import { Course } from '../../../models/course.model';
import { Group } from '../../../models/group.model';   
import { Room } from '../../../models/room.model';   
import { Professor } from '../../../models/professor.model';
import { Timeslot } from '../../../models/timeslot.model';
import { Constraint } from '../../../models/constraint.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Event } from '../../../models/event.model';
import { SidebarMenuComponent } from '../../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../../generic-modal/generic-modal.component';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent, GenericModalComponent],
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
  modalType: 'add' | 'delete' | 'edit' | 'addConstraint' | 'deleteConstraint' | null = null;
  eventToDelete: Timetable | null = null;
  eventToEdit: Timetable | null = null;
  constraintToDelete: Constraint | null = null;

  courses: Course[] = [];        
  professors: Professor[] = [];  
  groups: Group[] = []; 
  rooms: Room[] = [];
  constraints: Constraint[] = [];
  token: string = '';
  newTime: string = '';
  endTime: string = ''; 
  startTime: string = ''; 
  eventName: string = ''; 
  eventTypes: string[] = ['Course', 'Laboratory', 'Seminary'];

  isAdmin: boolean = false;
  isProfessor: boolean = false;

  user: any = '';
  role: any = '';
  id: any = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private timetableService: TimetableService,
    private courseService: CourseService,
    private professorService: ProfessorService,
    private roomService: RoomService,
    private groupService: GroupService,
    private http: HttpClient,
    private cookieService: CookieService,
    private constraintService: ConstraintService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    if (this.token == '') {
      this.router.navigate(['/login']);
    }

    this.user = localStorage.getItem("user");
    this.role = localStorage.getItem("role");

    this.route.params.subscribe((params) => {
      const id = params['id'];
      if (!id) {
        this.errorMessage = 'ID is missing from the URL. Return to the previous page.';
        return;
      }
      this.id = id;
      this.getTimetableById(id);
    });

    if( this.role == 'admin' && this.timetable?.userEmail == this.user ) 
      this.isAdmin = true;

    if( this.role == 'professor')
      this.isProfessor = true;

    this.fetchData();
  }

  fetchData() { 

    this.courseService.getAll(this.user)
    .subscribe( (data) => this.courses = data, (error) => console.error("Error loading courses: ", error) );

    this.professorService.getAll(this.user)
    .subscribe( (data) => this.professors = data, (error) => console.error("Error loading professors: ", error) ); 
    
    this.groupService.getAll(this.user)
    .subscribe( (data) => this.groups = data, (error) => console.error("Error loading groups: ", error) ); 
    
    this.roomService.getAll(this.user)
    .subscribe( (data) => this.rooms = data, (error) => console.error("Error loading rooms: ", error) ); 

    if( this.role == "professor" ) {
      this.constraintService.getAllForProfessor(this.user, this.id)
      .subscribe( (data) => this.constraints = data, (error) => console.error("Error loading constraints: ", error) );
    }
  
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
  
  populateEventDetails(): void {
    this.filteredEvents.forEach((event) => {
      this.courseService.getById(event.courseId).subscribe({
        next: (course) => {
          event.courseName = course.courseName;
          event.coursePackage = course.package;
        },
        error: (error) => {
          console.error('Failed to fetch course:', error);
        },
      });
  
      this.professorService.getById(event.professorId).subscribe({
        next: (professor) => {
          event.professorName = professor.name;
        },
        error: (error) => {
          console.error('Failed to fetch professor:', error);
        },
      });
  
      this.roomService.getById(event.roomId).subscribe({
        next: (room) => {
          event.roomName = room.name;
        },
        error: (error) => {
          console.error('Failed to fetch room:', error);
        },
      });
  
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
        console.log('Timetable deleted successfully');
        this.timetable = null;
        this.filteredEvents = [];
        this.errorMessage = null;
        this.router.navigate(['/timetable']);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete timetable. Please try again.';
        console.error('Server error:', error);
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

  
  handleModalConfirm(event: { confirmed: boolean; inputValue?: string }): void {
    if (event.confirmed) {
      if (this.modalType === 'delete' && this.eventToDelete) {
        this.deleteTimetable(this.eventToDelete.id!);
      } else if (this.modalType === 'edit' && this.timetable) {
        const updatedTimetable: Timetable = {
          ...this.timetable,
          events: this.timetable.events.map(event => {
            const course = this.courses.find(course => course.id === event.courseId);
            const professor = this.professors.find(prof => prof.id === event.professorId);
            const room = this.rooms.find(room => room.id === event.roomId);
            const group = this.groups.find(group => group.name === event.group);

            const updatedEvent: Event = {
              ...event,
              courseId: course?.id || event.courseId,
              roomId: room?.id || event.roomId,
              professorId: professor?.id || event.professorId,
              groupId: group?.id || event.groupId,
              weekEvenness: event.weekEvenness,
              courseName: course?.courseName || event.courseName,
              roomName: room?.name || event.roomName,
              professorName: professor?.name || event.professorName,
              group: group?.name || event.group,
              timeslot: {
                ...event.timeslot,
                day: event.timeslot.day,
                time: `${event.timeslot.startTime}-${event.timeslot.endTime}`
              }
            };
            return updatedEvent;
          })
        };
  
        // Sending update request to API
        this.timetableService.update(updatedTimetable.id!, updatedTimetable).subscribe({
          next: (response) => {
            // Proceed with post-update logic
            this.isEditMode = false;
            this.getTimetableById(updatedTimetable.id!);
            this.router.navigate([`/detail/${updatedTimetable.id}`]);
          },
          error: (error) => {
            console.error('Failed to update timetable:', error);
            this.errorMessage = 'Failed to update timetable. Please try again.';
          }
        });
      } else if ( this.modalType == "addConstraint" ) {
        if( this.inputValue ) {
          this.constraintService.create( { professorEmail: this.user, input : this.inputValue} ).subscribe({
            next: (response) => {
              this.isInputRequired = false;
              this.modalMessage = "Constraint created!";
            },
            error: (error) => {
              console.error('Failed to update timetable:', error);
              this.errorMessage = 'Failed to update timetable. Please try again.';
            }
          });
        }
      } else if ( this.modalType == "deleteConstraint" ) {
        if( this.constraintToDelete )
        this.constraintService.delete(this.constraintToDelete.id!).subscribe({
          next: (response) => {
            this.router.navigate([`/detail/${this.id}`]);
          },
          error: (error) => {
            console.error('Failed to delete constraint:', error);
            this.errorMessage = 'Failed to delete constraint. Please try again.';
          }
        })
      }
    }
    this.isInputRequired = false;
    this.inputValue = '';
    this.isEditMode = false;
    this.isModalVisible = false;
    this.modalType = null;
    this.modalMessage = '';
  }  
  
  
  showDeleteModal(timetable: Timetable): void {
    this.eventToDelete = timetable;
    this.modalTitle = 'Confirm Deletion';
    this.modalMessage = `Are you sure you want to delete the timetable "${timetable.name}?"`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  showDeleteModalConstraint(constraint: Constraint): void {
    this.constraintToDelete = constraint;
    this.modalTitle = 'Confirm Deletion';
    this.modalMessage = `Are you sure you want to delete the constraint "${constraint.type}?"`;
    this.modalType = 'deleteConstraint';
    this.isModalVisible = true;
  }

  showSaveModal(timetable: Timetable): void {
    this.modalTitle = 'Confirm Saving Changes';
    this.modalMessage = `Confirm changes for timetable "${timetable.name}?"`;
    this.modalType = 'edit';
    this.isModalVisible = true;
  }

  addConstraint(timetable: Timetable): void {
    this.isInputRequired = true;
    this.modalTitle = 'Add constraint';
    this.inputPlaceholder = 'Constraint description';  
    this.modalType = 'addConstraint';
    this.isModalVisible = true;
  }
  
  onBack() {
    window.history.back();
  }
}
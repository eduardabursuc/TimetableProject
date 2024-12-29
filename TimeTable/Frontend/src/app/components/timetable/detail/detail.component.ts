import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TimetableService } from '../../../services/timetable.service';
import { CourseService } from '../../../services/course.service';
import { ProfessorService } from '../../../services/professor.service';
import { RoomService } from '../../../services/room.service';
import { GroupService } from '../../../services/group.service';
import { ConstraintService } from '../../../services/constraint.service';
import { GlobalsService } from '../../../services/globals.service';
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
import { LoadingComponent } from '../../loading/loading.component';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarMenuComponent, GenericModalComponent, LoadingComponent],
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
  timetableToDelete: Timetable | null = null;
  timetableToEdit: Timetable | null = null;
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

  isLoading: boolean = true;
  privacy: 'private' | 'public' = 'private';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly timetableService: TimetableService,
    private readonly courseService: CourseService,
    private readonly professorService: ProfessorService,
    private readonly roomService: RoomService,
    private readonly groupService: GroupService,
    private readonly cookieService: CookieService,
    private readonly constraintService: ConstraintService,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    this.globals.checkToken(this.token);

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

  }

  async fetchData() { 

    const owner = this.isAdmin ? this.user : this.timetable?.userEmail;

    this.courseService.getAll(owner)
    .subscribe({ next: (data) => this.courses = data, error: (error) => console.error("Error loading courses: ", error) });

    this.professorService.getAll(owner)
    .subscribe({ next: (data) => this.professors = data, error: (error) => console.error("Error loading professors: ", error) }); 
    
    this.groupService.getAll(owner)
    .subscribe({ next: (data) => this.groups = data, error: (error) => console.error("Error loading groups: ", error) }); 
    
    this.roomService.getAll(owner)
    .subscribe({ next: (data) => this.rooms = data, error: (error) => { console.error("Error loading rooms: ", error); this.isLoading = false; } , complete: () => this.isLoading = false }); 

    if( this.role == "professor" ) {
      this.constraintService.getAllForProfessor(this.user, this.id)
      .subscribe({ next: (data) => this.constraints = data, error: (error) => console.error("Error loading constraints: ", error) });
    }
  
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
  }
  

  getTimetableById(id: string): void {
    this.timetableService.getById(id).subscribe({
      next: (response) => {
        this.timetable = response;
        this.originalEvents = response.events;
        this.filteredEvents = [...this.originalEvents];
        this.errorMessage = null;
  
        // Fetch additional details for each event
        this.fetchData();

        this.populateEventDetails();
  
        this.groupEventsByDay();
      },
      error: (error) => {
        this.errorMessage = `Failed to fetch details for ID: ${id}.`;
        console.error(error);
      },
      complete: () => {
        this.timetable?.isPublic ? this.privacy = 'public' : this.privacy = 'private';

        if( this.role == 'admin' && this.timetable?.userEmail == this.user ) 
          this.isAdmin = true;
    
        if( this.role == 'professor')
          this.isProfessor = true;
      }
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
      if (this.modalType === 'delete' && this.timetableToDelete) {
        this.deleteTimetable(this.timetableToDelete.id);
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
              courseId: course?.id ?? event.courseId,
              roomId: room?.id ?? event.roomId,
              professorId: professor?.id ?? event.professorId,
              groupId: group?.id ?? event.groupId,
              weekEvenness: event.weekEvenness,
              courseName: course?.courseName ?? event.courseName,
              roomName: room?.name ?? event.roomName,
              professorName: professor?.name ?? event.professorName,
              group: group?.name ?? event.group,
              timeslot: {
                ...event.timeslot,
                day: event.timeslot.day,
                time: `${event.timeslot.startTime}-${event.timeslot.endTime}`
              }
            };
            return updatedEvent;
          })
        };
  
        console.log(updatedTimetable);
        // Sending update request to API
        this.timetableService.update(updatedTimetable.id, updatedTimetable).subscribe({
          next: (response) => {
            // Proceed with post-update logic
            this.isEditMode = false;
            this.getTimetableById(updatedTimetable.id);
            this.router.navigate([`/detail/${updatedTimetable.id}`]);
          },
          error: (error) => {
            console.error('Failed to update timetable:', error);
            this.errorMessage = 'Failed to update timetable. Please try again.';
          }
        });
      } else if ( this.modalType == "addConstraint" ) {
        this.isLoading = true;
        this.inputValue = event.inputValue? event.inputValue : "";
        if( this.inputValue ) {
          this.constraintService.create( { professorEmail: this.user, timetableId: this.id, input : this.inputValue} ).subscribe({
            next: (response) => {
              window.location.reload();
            },
            error: (error) => {
              console.error('Failed to add a constraint:', error);
              this.errorMessage = 'Failed to add a constraint. Please try again.';
              this.isLoading = false;
            },
            complete: () => {
              this.isLoading = false;
            }
          });
        }
      } else if ( this.modalType == "deleteConstraint" ) {
        if( this.constraintToDelete )
        this.constraintService.delete(this.constraintToDelete.id).subscribe({
          next: (response) => {
            const id = this.constraintToDelete ? this.constraintToDelete.id : null ;
            if ( id ) this.constraints = this.constraints.filter(constraint => constraint.id !== id);
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
    this.timetableToDelete = timetable;
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


  getCourseNameById( id: string ) {
    return this.courses.find(course => course.id === id)?.courseName;
  }

  getRoomNameById( id: string ) {
    return this.rooms.find(room => room.id === id)?.name;
  }

  getGroupNameById( id: string ) {
    return this.groups.find(group => group.id === id)?.name;
  }

  togglePrivacy() {
    if( this.timetable )
      this.timetable.isPublic = !this.timetable.isPublic;

    this.timetable?.isPublic ? this.privacy = 'public' : this.privacy = 'private';
  }

}
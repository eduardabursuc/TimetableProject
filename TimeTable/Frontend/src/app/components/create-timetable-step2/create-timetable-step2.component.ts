import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Course } from '../../models/course.model';
import { Group } from '../../models/group.model';   
import { Professor } from '../../models/professor.model';
import { TimetableService } from '../../services/timetable.service';
import { CourseService } from '../../services/course.service';
import { ProfessorService } from '../../services/professor.service';
import { GroupService } from '../../services/group.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { DayInterval } from '../../models/day-interval.model';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-create-timetable-step2',
  templateUrl: './create-timetable-step2.component.html',
  styleUrls: ['./create-timetable-step2.component.css'],
  standalone: true,
  imports: [FormsModule, SidebarMenuComponent, CommonModule, GenericModalComponent, LoadingComponent]
})
export class CreateTimetableStep2Component implements OnInit {
  courses: Course[] = [];        
  professors: Professor[] = [];  
  groups: Group[] = []; 
  addedEvents: any[] = []; // Store events temporarily

  selectedCourse: Course | null = null;
  selectedProfessor: Professor | null = null;
  selectedGroup: Group | null = null;
  eventDuration: number = 1;
  eventType: string = 'course';
  token: string = '';

  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  inputValue: string = '';
  inputPlaceholder: string = '';
  isInputRequired: boolean = false;
  modalType: 'add' | 'delete' | 'generate' | 'edit' | 'error' | null = null; // Added 'edit'
  eventToDelete: any = null;
  eventToEdit: any = null;
  showCancelButton: boolean = true;

  user: any = null;

  validatedIntervals: DayInterval[] = [];

  isLoading: boolean = true;

  constructor(
    private readonly router: Router, 
    private readonly cookieService: CookieService, 
    private readonly timetableService: TimetableService,
    private readonly courseService: CourseService,
    private readonly professorService: ProfessorService,
    private readonly groupService: GroupService,
    private readonly globals: GlobalsService
  ) { }

  ngOnInit(): void {

    this.token = this.cookieService.get('authToken');
    this.globals.checkToken(this.token);

    if (this.token == '') {
      this.router.navigate(['/login']);
    }

    this.user = localStorage.getItem("user");

    this.fetchData();
    
    // Load added events from localStorage if available
    if (this.isLocalStorageAvailable()) {
      const storedEvents = localStorage.getItem('addedEvents');
      if (storedEvents) {
        this.addedEvents = JSON.parse(storedEvents);
      }
  
      // Retrieve validated intervals
      const storedIntervals = localStorage.getItem('timeIntervals');
      if (storedIntervals) {
        this.validatedIntervals = JSON.parse(storedIntervals);
      }
  
    }
  }

  fetchData() {

    this.courseService.getAll(this.user).subscribe({
      next: (response) => {
        this.courses = response;
      },
      error: (error) => {
        console.error('Failed to fetch courses:', error);
      },
    });

    this.professorService.getAll(this.user).subscribe({
      next: (response) => {
        this.professors = response;
      },
      error: (error) => {
        console.error('Failed to fetch professors:', error);
      },
    });
      
    this.groupService.getAll(this.user).subscribe({
      next: (response) => {
        this.groups = response;
      },
      error: (error) => {
        console.error('Failed to fetch groups:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
  

  addEvent() {
    if (!this.selectedCourse || !this.selectedProfessor || !this.selectedGroup) {
      this.modalTitle = 'Missing Fields';
      this.modalMessage = 'Please fill in all fields before adding an event.';
      this.modalType = 'add';
      this.isModalVisible = true;
      return;
    }
  
    const newEvent = {
      course: this.selectedCourse.courseName,
      courseId: this.selectedCourse.id,  
      professor: this.selectedProfessor.name,
      professorId: this.selectedProfessor.id,
      group: this.selectedGroup.name,
      groupId: this.selectedGroup.id,
      duration: this.eventDuration,
      type: this.eventType
    };
  
    // Check if the event already exists
    const existingEvent = this.addedEvents.find(event =>
      event.course === newEvent.course &&
      event.professor === newEvent.professor &&
      event.group === newEvent.group &&
      event.duration === newEvent.duration &&
      event.type === newEvent.type
    );
  
    if (existingEvent) {
      // Show error modal if the event already exists
      this.modalTitle = 'Event Already Exists';
      this.modalMessage = 'This event is already added. Please modify it or remove the existing one.';
      this.modalType = 'add';
      this.isModalVisible = true;
      return;
    }
  
    // Add or update the event
    if (this.eventToEdit) {
      // Find the index of the event to edit in the addedEvents array
      const index = this.addedEvents.findIndex(event => event === this.eventToEdit);
      if (index !== -1) {
        // Replace the event at the index with the new event data
        this.addedEvents[index] = newEvent;
      }
  
      // Clear the eventToEdit after saving the updated event
      this.eventToEdit = null;
    } else {
      // Add the new event
      this.addedEvents.push(newEvent);
    }
  
    // Save events to localStorage if available
    if (this.isLocalStorageAvailable()) {
      localStorage.setItem('addedEvents', JSON.stringify(this.addedEvents));
    }
  
    // Reset the form
    this.selectedCourse = null;
    this.selectedProfessor = null;
    this.selectedGroup = null;
    this.eventDuration = 1;
    this.eventType = 'course';
  }
  

  editEvent(event: any) {
    this.eventToEdit = event;
    this.modalTitle = 'Confirm Edit';
    this.modalMessage = `Are you sure you want to edit the event for ${event.course}?`;
    this.modalType = 'edit';
    this.isModalVisible = true;
  }
  

  deleteEvent(event: any) {
    this.eventToDelete = event;
    this.modalTitle = 'Confirm Deletion';
    this.modalMessage = `Are you sure you want to delete the event for ${event.course}?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  handleModalConfirm(event: { confirmed: boolean, inputValue?: string }) {
    if (!event.confirmed) {
      this.closeModal();
      return;
    }
  
    switch (this.modalType) {
      case 'add':
        break;
      case 'delete':
        this.handleDeleteConfirmation();
        break;
      case 'edit':
        this.handleEditConfirmation();
        break;
      case 'generate':
        this.handleGenerateConfirmation(event.inputValue);
        break;
    }
  
    this.closeModal();
  }
  
  closeModal() {
    this.isModalVisible = false;
    this.modalType = null;
  }
  
  handleDeleteConfirmation() {
    if (this.eventToDelete) {
      this.addedEvents = this.addedEvents.filter(e => e !== this.eventToDelete);
      this.eventToDelete = null;
      this.saveEventsToLocalStorage();
    }
  }
  
  handleEditConfirmation() {
    if (this.eventToEdit) {
      this.loadEventForEdit();
    }
  }
  
  handleGenerateConfirmation(inputValue?: string) {
    if (this.addedEvents.length === 0) {
      this.closeModal();
    } else {
      this.inputValue = inputValue ?? "";
      if (this.inputValue) {
        this.isLoading = true;
        this.generateTimetable();
      }
    }
  }
  
  saveEventsToLocalStorage() {
    if (this.isLocalStorageAvailable()) {
      localStorage.setItem('addedEvents', JSON.stringify(this.addedEvents));
    }
  }

  loadEventForEdit() {
    if (this.eventToEdit) {
      this.selectedCourse = this.courses.find(course => course.courseName === this.eventToEdit.course) || null;
      this.selectedProfessor = this.professors.find(professor => professor.name === this.eventToEdit.professor) || null;
      this.selectedGroup = this.groups.find(group => group.name === this.eventToEdit.group) || null;
      this.eventDuration = this.eventToEdit.duration;
      this.eventType = this.eventToEdit.type;
  
      // Now show the edit form
      this.isModalVisible = true;
    }
  }

  onGenerate() {
    this.inputValue = ''; 
    if (this.addedEvents.length === 0) {
      this.modalTitle = 'No Events Added';
      this.modalMessage = 'No events have been added. Please add events before generating the timetable.';
      this.modalType = 'generate';
      this.isModalVisible = true;
      this.isInputRequired = false;
      this.showCancelButton = false;
    } else {
      this.modalTitle = 'Generate Timetable';
      this.modalMessage = 'Please enter a name for the timetable:';
      this.modalType = 'generate';
      this.isModalVisible = true;
      this.isInputRequired = true;
      this.inputPlaceholder = 'Timetable Name';
      this.showCancelButton = true;
    }
  }

  generateTimetable() {
    if (!this.inputValue.trim()) {
      console.error('Timetable name is required.');
      return;
    }
  
    // Prepare timeslot data from validated intervals
    const timeslots = this.validatedIntervals.map(interval => ({
      day: interval.day,
      time: `${interval.startTime} - ${interval.endTime}`
    }));

  
    // Construct the body
    const requestBody = {
      userEmail: this.user, // Replace with the actual user's email
      name: this.inputValue.trim(),
      events: this.addedEvents.map(event => ({
        EventName: event.type,
        CourseId: event.courseId,
        ProfessorId: event.professorId,
        GroupId: event.groupId,
        Duration: event.duration
      })),
      timeslots: timeslots 
    };

    console.log(requestBody);
    

    this.timetableService.create(requestBody).subscribe({
      next: (response) => {
        console.log('Timetable generated successfully:', response);
        // localStorage.clear();
        this.router.navigate([`/detail/${response}`]);
      },
      error: (error) => {
        this.isModalVisible = true;
        this.modalTitle = 'Creating error';
        this.modalMessage = 'No valid timetable can be generated.';
        this.modalType = 'error';
        console.error('Error generating timetable:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });

  }

  onBack() {
    this.router.navigate(['/create-timetable-step1']);
  }

  // Utility method to check if localStorage is available
  private isLocalStorageAvailable(): boolean {
    try {
      const testKey = 'test';
      localStorage.setItem(testKey, testKey);
      localStorage.removeItem(testKey);
      return true;
    } catch (e) {
      return false;
    }
  }
  

}
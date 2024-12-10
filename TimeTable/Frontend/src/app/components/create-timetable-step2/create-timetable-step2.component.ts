import { Component, OnInit } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Course } from '../../models/course.model';
import { Group } from '../../models/group.model';   
import { Professor } from '../../models/professor.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { DayInterval } from '../../models/day-interval.model';

@Component({
  selector: 'app-create-timetable-step2',
  templateUrl: './create-timetable-step2.component.html',
  styleUrls: ['./create-timetable-step2.component.css'],
  standalone: true,
  imports: [FormsModule, SidebarMenuComponent, CommonModule, GenericModalComponent, HttpClientModule]
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

  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  inputValue: string = '';
  inputPlaceholder: string = '';
  isInputRequired: boolean = false;
  modalType: 'add' | 'delete' | 'generate' | 'edit' | null = null; // Added 'edit'
  eventToDelete: any = null;
  eventToEdit: any = null;
  showCancelButton: boolean = true;

  validatedIntervals: DayInterval[] = [];

  private apiUrl = 'http://localhost:5088/api/v1';

  constructor(private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
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
        console.log(storedIntervals);
      }
    }
  }

  fetchData() {
    const userEmail = 'admin@gmail.com'; // Set userEmail value
  
    this.http.get<Course[]>(`${this.apiUrl}/courses?userEmail=${userEmail}`).subscribe(
      (data) => this.courses = data,
      
      (error) => console.error("Error loading courses: ", error)
    );
  
    this.http.get<Professor[]>(`${this.apiUrl}/professors?userEmail=${userEmail}`).subscribe(
      (data) => this.professors = data,
      (error) => console.error("Error loading professors: ", error)
    );
  
    this.http.get<Group[]>(`${this.apiUrl}/groups?userEmail=${userEmail}`).subscribe(
      (data) => this.groups = data,
      (error) => console.error("Error loading groups: ", error)
    );
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
    if (event.confirmed) {
      if (this.modalType === 'add') {
        // Handle add event confirmation, if needed
      } else if (this.modalType === 'delete' && this.eventToDelete) {
        this.addedEvents = this.addedEvents.filter(e => e !== this.eventToDelete);
        this.eventToDelete = null;
        // Save updated events to localStorage if available
        if (this.isLocalStorageAvailable()) {
          localStorage.setItem('addedEvents', JSON.stringify(this.addedEvents));
        }
      } else if (this.modalType === 'edit' && this.eventToEdit) {
        // Show the edit form with the details pre-filled 
        this.loadEventForEdit();
      } else if (this.modalType === 'generate') {
        if (this.addedEvents.length === 0) {
          // If no events are added, close the modal
          this.isModalVisible = false;
        } else {
          this.inputValue = event.inputValue? event.inputValue : "";
          if (this.inputValue) {
            this.generateTimetable();
          }
        }
      }
    }
    this.isModalVisible = false;
    this.modalType = null;
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
      this.isInputRequired = false;  // No input field for this case
      this.showCancelButton = false; // Hide the cancel button when no events are added
    } else {
      this.modalTitle = 'Generate Timetable';
      this.modalMessage = 'Please enter a name for the timetable:';
      this.modalType = 'generate';
      this.isModalVisible = true;
      this.isInputRequired = true;   // Show input field for name
      this.inputPlaceholder = 'Timetable Name';  // Placeholder for the input field
      this.showCancelButton = true; // Show the cancel button when input is required
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
      userEmail: 'admin@gmail.com', // Replace with the actual user's email
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

  
<<<<<<< Updated upstream
    // Make the POST request
     this.http.post(`${this.apiUrl}/timetables`, requestBody).subscribe(
       response => {
         console.log('Timetable generated successfully:', response);
         // localStorage.clear();
         this.router.navigate(['/generate-timetable']); // Navigate after success
         this.router.navigate([`/detail/${response}`]);
       },
       error => {
         console.error('Error generating timetable:', error);
       }
     );
=======
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.token}`,
    });
    
    // Make the POST request with headers
    this.http.post(`${this.apiUrl}/timetables`, requestBody, { headers }).subscribe(
      response => {
        console.log('Timetable generated successfully:', response);
        // localStorage.clear();
        this.router.navigate([`/detail/${response}`]); // Navigate after success
      },
      error => {
        this.isModalVisible = true;
        this.modalTitle = 'Creating error';
        this.modalMessage = error;
        this.modalType = 'error';
        console.error('Error generating timetable:', error);
      }
    );
>>>>>>> Stashed changes

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
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Course } from '../../models/course.model';
import { CourseService } from '../../services/course.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-courses',
  templateUrl: './courses.component.html',
  styleUrls: ['./courses.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    SidebarMenuComponent,
    CommonModule,
    GenericModalComponent,
    LoadingComponent
  ],
})
export class CoursesComponent implements OnInit {
  courses: Course[] = [];
  newCourse: Course = {id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
  courseToDelete: Course = {id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
  isAddCase: boolean = true;
  token: string = '';
  user: any = null;

  isModalVisible: boolean = false;
  cancelOption: boolean = false;
  modalType: 'delete' | 'error' | null = null;
  modalTitle: string = '';
  modalMessage: string = '';

  isLoading: boolean = true;

  constructor(
    private readonly router: Router,
    private readonly cookieService: CookieService,
    private readonly courseService: CourseService,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    this.globals.checkToken(this.token);

    if (this.token === '') {
      this.router.navigate(['/login']);
    }
    this.user = localStorage.getItem('user');
    this.fetchCourses();
  }

  fetchCourses(): void {
    this.courseService.getAll(this.user).subscribe({
      next: (response) => {
        this.courses = response;
      },
      error: (error) => {
        console.error('Failed to fetch courses:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  isValidCourse(): boolean {
    if (!this.newCourse.courseName.trim() || this.newCourse.credits === 0 || this.newCourse.semester === 0) {
        this.modalMessage = "Please provide valid details.";
        this.modalTitle = "Invalid course";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newCourse = {id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
        return false;
      }
  
      if ( this.courses.find( g => g.courseName == this.newCourse.courseName && g.id !== this.newCourse.id) ) {
        this.modalMessage = "A course with the same name already exists.";
        this.modalTitle = "Invalid course";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newCourse = {id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
        return false;
      }

      return true;
  }

  addCourse(): void {

    if( !this.isValidCourse() ) return;

    const requestBody = {
      userEmail: this.user,
      courseName: this.newCourse.courseName,
      credits: this.newCourse.credits,
      package: this.newCourse.package,
      semester: this.newCourse.semester,
      level: this.newCourse.level
    };

    this.courseService.create(requestBody).subscribe({
      next: (response) => {
        this.newCourse.id = response.id;
        this.courses.push(this.newCourse);
        this.newCourse = {id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
      },
      error: (err) => {
        console.error('Error adding course:', err);
      },
    });
  }

  editCourse(course: Course): void {
    this.newCourse = course;
    this.isAddCase = false;
  }

  updateCourse(): void {

    if( !this.isValidCourse() ) return;

    this.courseService.update(this.newCourse.id, this.newCourse).subscribe({
      next: () => {
        const index = this.courses.findIndex(
          (course) => course.id === this.newCourse.id
        );
        if (index !== -1) {
          this.courses[index] = { ...this.newCourse };
        }
      },
      error: (err) => {
        console.error('Error updating course:', err);
      },
    });

    this.newCourse = { id: '', courseName: '', credits: 0, package: 'compulsory', semester: 0, level: 'license'};
    this.isAddCase = true;
  }

  showDeleteModal(course: Course): void {
    this.courseToDelete = course;
    this.modalTitle = 'Delete course';
    this.cancelOption = true;
    this.modalMessage = `Are you sure you want to delete ${course.courseName} ?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  deleteCourse(): void {
    this.courseService.delete(this.courseToDelete.id).subscribe({
      next: () => {
        this.courses = this.courses.filter((r) => r.id !== this.courseToDelete.id);
      },
      error: (err) => {
        console.error('Error deleting course:', err);
      },
    });
  }

  handleModalConfirm(): void {
    this.isModalVisible = false;
    if ( this.modalType === 'delete' ){
        this.deleteCourse();
    }
  }

  onBack() {
    window.history.back();
  }
  

}

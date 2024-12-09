import { Component, OnInit } from '@angular/core';
import { CourseService } from '../../services/course.service';
import { Course } from '../../models/course.model';
import { TableComponent } from '../_shared/table/table.component';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';

@Component({
  selector: 'app-courses',
  templateUrl: './courses.component.html',
  styleUrls: ['./courses.component.css'],
  imports: [TableComponent, SidebarMenuComponent]
})
export class CoursesComponent implements OnInit {
  courses: Course[] = [];
  columns: { field: keyof Course; label: string; }[]  = [
    { field: 'id', label: 'ID' },
    { field: 'courseName', label: 'Course Name' },
    { field: 'credits', label: 'Credits' },
    { field: 'package', label: 'Package' },
    { field: 'semester', label: 'Semester' },
    { field: 'level', label: 'Level' },
  ];
  emptyCourse: Course = {
    id: '',
    courseName: '',
    credits: 0,
    package: '',
    semester: 0,
    level: ''
  };

  constructor(private courseService: CourseService) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.courseService.getAll("admin@gmail.com").subscribe((data) => {
      this.courses = data;
    });
  }

  onCreate(newCourse: Course): void {
    this.courseService.create(newCourse).subscribe(() => {
      this.loadCourses();
    });
  }

  onUpdate(updatedCourse: Course): void {
    this.courseService.update(updatedCourse.id, updatedCourse).subscribe(() => {
      this.loadCourses();
    });
  }

  onDelete(course: Course): void {
    this.courseService.delete(course.id).subscribe(() => {
      this.loadCourses();
    });
  }
}
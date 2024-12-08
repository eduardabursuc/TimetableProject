import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Course } from '../models/course.model';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private apiUrl = 'http://localhost:5088/api/v1/courses';

  constructor(private http: HttpClient) {}

  /**
   * Create a new course.
   * @param course - The course data to be created.
   * @returns Observable with the created course data.
   */
  create(course: Course): Observable<Course> {
    return this.http.post<Course>(this.apiUrl, course);
  }

  /**
   * Update an existing course.
   * @param id - The ID of the course to update.
   * @param course - The updated course object.
   * @returns Observable for the update operation.
   */
  update(id: string, course: Course): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, course);
  }

  /**
   * Get a course by ID.
   * @param id - The ID of the course to retrieve.
   * @returns Observable with the course data.
   */
  getById(id: string): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get all courses.
   * @param userEmail - The email of the user to retrieve courses for.
   * @returns Observable with a list of all courses.
   */
  getAll(userEmail: string): Observable<Course[]> {
    return this.http.get<Course[]>(`${this.apiUrl}?userEmail=${userEmail}`);
  }

  /**
   * Delete a course by ID.
   * @param id - The ID of the course to delete.
   * @returns Observable for the delete operation.
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
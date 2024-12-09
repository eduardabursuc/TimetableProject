import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Course } from '../models/course.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private apiUrl = 'https://timetablegenerator.best/api/v1/courses';

  //private apiUrl = 'http://localhost:5088/api/v1/courses';
  constructor(private http: HttpClient) {}

  create(data: { Events: any[] }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  getAll(userEmail: string): Observable<Course[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Course[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  update(id: string, course: Course): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, course);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
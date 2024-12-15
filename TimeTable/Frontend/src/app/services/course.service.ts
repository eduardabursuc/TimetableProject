import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Course } from '../models/course.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CourseService {

  private apiUrl: string;

  constructor(private http: HttpClient, private globals: GlobalsService) {
    this.apiUrl = `${this.globals.apiUrl}/v1/courses`;
  }

  create(data: { userEmail: string, courseName: string, credits: number, package: string, semester: number, level: string }): Observable<{ id: string }> {
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
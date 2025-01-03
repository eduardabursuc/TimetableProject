import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Course } from '../models/course.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CourseService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient, 
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/courses`;
  }

  create(data: { userEmail: string, courseName: string, credits: number, package: string, semester: number, level: string }): Observable<{ id: string }> {
    const headers = this.globals.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  getAll(userEmail: string): Observable<Course[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Course[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  update(id: string, course: Course): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, course, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

}
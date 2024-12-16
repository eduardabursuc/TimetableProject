import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Professor } from '../models/professor.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfessorService {

  private apiUrl: string;

  constructor(private http: HttpClient, private globals: GlobalsService) {
    this.apiUrl = `${this.globals.apiUrl}/v1/professors`;
  }

  create(data: { userEmail: string, name: string }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  getAll(userEmail: string): Observable<Professor[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Professor[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Professor> {
    return this.http.get<Professor>(`${this.apiUrl}/${id}`);
  }

  update(id: string, professor: Professor): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, professor);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addTimetable( data: {id: string, timetableId: string } ): Observable<void> {
    return this.http.post<void>(this.apiUrl, data);
  }

}
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Professor } from '../models/professor.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfessorService {
  //private apiUrl = 'https://timetablegenerator.best/api/v1/professors';

  private apiUrl = 'http://localhost:5088/api/v1/professors';
  constructor(private http: HttpClient) {}

  create(data: { Events: any[] }): Observable<{ id: string }> {
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

}
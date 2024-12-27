import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Professor } from '../models/professor.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfessorService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient,
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/professors`;
  }

  create(data: { userEmail: string, name: string, email: string }): Observable<{ id: string }> {
    const headers = this.globals.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  getAll(userEmail: string): Observable<Professor[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Professor[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Professor> {
    return this.http.get<Professor>(`${this.apiUrl}/${id}`);
  }

  update(id: string, professor: Professor): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, professor, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

}
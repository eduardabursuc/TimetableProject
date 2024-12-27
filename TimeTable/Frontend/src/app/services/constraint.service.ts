import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Constraint } from '../models/constraint.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConstraintService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient, 
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/constraints`;
  }

  create(data: { professorEmail: string, timetableId: string, input: string }): Observable<{ id: string }> {
    const headers = this.globals.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  getAllForProfessor(professorEmail: string, timetableId: string): Observable<Constraint[]> {
    const params = new HttpParams()
        .set('professorEmail', professorEmail)
        .set('timetableId', timetableId);
    return this.http.get<Constraint[]>(`${this.apiUrl}/forProfessor`, { params });
  }

  delete(id: string): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

}
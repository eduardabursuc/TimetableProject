import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Constraint } from '../models/constraint.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';
import { time } from 'console';

@Injectable({
  providedIn: 'root'
})
export class ConstraintService {

  private apiUrl: string;

  constructor(private http: HttpClient, private globals: GlobalsService) {
    this.apiUrl = `${this.globals.apiUrl}/v1/constraints`;
  }

  create(data: { professorEmail: string, input: string }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  getAllForProfessor(professorEmail: string, timetableId: string): Observable<Constraint[]> {
    const params = new HttpParams()
        .set('professorEmail', professorEmail)
        .set('timetableId', timetableId);
    return this.http.get<Constraint[]>(`${this.apiUrl}/forProfessor`, { params });
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
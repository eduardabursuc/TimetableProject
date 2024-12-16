import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Timetable } from '../models/timetable.model';
import { Observable } from 'rxjs';
import { GlobalsService } from './globals.service';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root'
})
export class TimetableService {

  private apiUrl: string;

  constructor(
    private http: HttpClient, 
    private cookieService: CookieService,
    private globals: GlobalsService,
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/timetables`;
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.cookieService.get('authToken');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }


  create(data: { userEmail: string, name: string, events: any[], timeslots: any[] }): Observable<{ id: string }> {
    const headers = this.getAuthHeaders();
    return this.http.post<{ id: string }>(`${this.apiUrl}`, data, { headers });
  }

  update(id: string, timetable: Timetable): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, timetable, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

  getAll(userEmail: string): Observable<Timetable[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Timetable[]>(`${this.apiUrl}`, { params });
  }
  
  getById(id: string): Observable<Timetable> {
    return this.http.get<Timetable>(`${this.apiUrl}/${id}`);
  }

  getByRoom(id: string, roomName: string): Observable<Timetable> {
    const params = new HttpParams()
      .set('id', id)
      .set('roomName', roomName);
    return this.http.get<Timetable>(`${this.apiUrl}/byRoom`, { params });
  }

  getByGroup(id: string, groupName: string): Observable<Timetable> {
    const params = new HttpParams()
      .set('id', id)
      .set('groupName', groupName);
    return this.http.get<Timetable>(`${this.apiUrl}/byGroup`, { params });
  }

  getByProfessor(id: string, professorId: string): Observable<Timetable> {
    const params = new HttpParams()
      .set('id', id)
      .set('professorId', professorId);
    return this.http.get<Timetable>(`${this.apiUrl}/byProfessor`, { params });
  }

  getPaginated(userEmail: string, page: number, pageSize: number): Observable<Timetable[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<Timetable[]>(`${this.apiUrl}/paginated`, { params });
  }

  getForProfessor(professorEmail: string): Observable<Timetable[]> {
    const params = new HttpParams()
      .set('professorEmail', professorEmail);
    return this.http.get<Timetable[]>(`${this.apiUrl}/forProfessor`, { params });
  }

}
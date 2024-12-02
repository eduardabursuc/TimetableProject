import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Timetable } from '../models/timetable.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimetableService {
  private apiUrl = 'http://localhost:5088/api/v1/timetables';

  constructor(private http: HttpClient) {}

  create(data: { Events: any[] }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  getAll(): Observable<Timetable[]> {
    return this.http.get<Timetable[]>(`${this.apiUrl}`);
  }

  getById(id: string): Observable<Timetable> {
    return this.http.get<Timetable>(`${this.apiUrl}/${id}`);
  }

  update(id: string, timetable: Timetable): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, timetable);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
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

  getPaginated(page: number, pageSize: number): Observable<Timetable[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<Timetable[]>(`${this.apiUrl}/paginated`, { params });
  }
}
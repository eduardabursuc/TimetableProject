import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Room } from '../models/room.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private apiUrl = 'https://timetablegenerator.best/api/v1/rooms';

  //private apiUrl = 'http://localhost:5088/api/v1/rooms';
  constructor(private http: HttpClient) {}

  create(data: { Events: any[] }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  getAll(userEmail: string): Observable<Room[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Room[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  update(id: string, room: Room): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, room);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
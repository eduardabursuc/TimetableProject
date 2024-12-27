import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Room } from '../models/room.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient, 
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/rooms`;
  }

  create(data: { userEmail: string, name: string, capacity: number }): Observable<{ id: string }> {
    const headers = this.globals.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  getAll(userEmail: string): Observable<Room[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Room[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  update(id: string, room: Room): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, room, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

}
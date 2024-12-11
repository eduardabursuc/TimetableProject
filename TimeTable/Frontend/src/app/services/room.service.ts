import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Room } from '../models/room.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';
import { GlobalPreloadContext } from 'module';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  private apiUrl: string;

  constructor(private http: HttpClient, private globals: GlobalsService) {
    this.apiUrl = `${this.globals.apiUrl}/v1/rooms`;
  }

  create(data: { room: Room }): Observable<{ id: string }> {
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
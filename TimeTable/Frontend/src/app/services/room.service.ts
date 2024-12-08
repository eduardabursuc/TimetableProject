import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Room } from '../models/room.model';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private apiUrl = 'http://localhost:5088/api/v1/rooms';

  constructor(private http: HttpClient) {}

  /**
   * Get all rooms.
   * @param userEmail - The email of the user to retrieve rooms for.
   * @returns Observable with a list of all rooms.
   */
  getAll(userEmail: string): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}?userEmail=${userEmail}`);
  }

  /**
   * Get a room by ID.
   * @param id - The ID of the room to retrieve.
   * @returns Observable with the room data.
   */
  getById(id: string): Observable<Room> {
    return this.http.get<Room>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new room.
   * @param room - The room data to be created.
   * @returns Observable with the created room ID.
   */
  create(room: Room): Observable<string> {
    return this.http.post<string>(this.apiUrl, room);
  }

  /**
   * Update an existing room.
   * @param id - The ID of the room to update.
   * @param room - The updated room object.
   * @returns Observable for the update operation.
   */
  update(id: string, room: Room): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, room);
  }

  /**
   * Delete a room by ID.
   * @param id - The ID of the room to delete.
   * @returns Observable for the delete operation.
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
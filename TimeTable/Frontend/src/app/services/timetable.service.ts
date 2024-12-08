import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Timetable } from '../models/timetable.model';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class TimetableService {
  private apiUrl = 'http://localhost:5088/api/v1/timetables';

  constructor(private http: HttpClient) {}

  /**
   * Create a new timetable.
   * @param data - An object containing the events to be included in the timetable.
   * @returns Observable with the ID of the created timetable.
   */
  create(data: { Events: any[] }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data);
  }

  /**
   * Update an existing timetable.
   * @param id - The ID of the timetable to update.
   * @param timetable - The updated timetable object.
   * @returns Observable for the update operation.
   */
  update(id: string, timetable: Timetable): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, timetable);
  }

  /**
   * Get a timetable by ID.
   * @param id - The ID of the timetable.
   * @returns Observable with the timetable data.
   */
  getById(id: string): Observable<Timetable> {
    return this.http.get<Timetable>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get a timetable by room.
   * @param id - The ID of the timetable.
   * @param roomId - The ID of the room.
   * @returns Observable with the timetable data.
   */
  getByRoom(id: string, roomId: string): Observable<Timetable> {
    const params = new HttpParams().set('id', id).set('roomId', roomId);
    return this.http.get<Timetable>(`${this.apiUrl}/byRoom`, { params });
  }

  /**
   * Get a timetable by group.
   * @param id - The ID of the timetable.
   * @param groupId - The ID of the group.
   * @returns Observable with the timetable data.
   */
  getByGroup(id: string, groupId: string): Observable<Timetable> {
    const params = new HttpParams().set('id', id).set('groupId', groupId);
    return this.http.get<Timetable>(`${this.apiUrl}/byGroup`, { params });
  }

  /**
   * Get a timetable by professor.
   * @param id - The ID of the timetable.
   * @param professorId - The ID of the professor.
   * @returns Observable with the timetable data.
   */
  getByProfessor(id: string, professorId: string): Observable<Timetable> {
    const params = new HttpParams().set('id', id).set('professorId', professorId);
    return this.http.get<Timetable>(`${this.apiUrl}/byProfessor`, { params });
  }

  /**
   * Get all timetables.
   * @param userEmail - The email of the user to retrieve timetables for.
   * @returns Observable with a list of all timetables.
   */
  getAll(userEmail: string): Observable<Timetable[]> {
    return this.http.get<Timetable[]>(`${this.apiUrl}?userEmail=${userEmail}`);
  }

  /**
   * Delete a timetable by ID.
   * @param userEmail - The email of the user.
   * @param id - The ID of the timetable to delete.
   * @returns Observable for the delete operation.
   */
  delete(userEmail: string, id: string): Observable<void> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { params });
  }

  /**
   * Get paginated timetables.
   * @param page - The page number.
   * @param pageSize - The number of items per page.
   * @returns Observable with the paginated timetables.
   */
  getFilteredTimetables(page: number, pageSize: number): Observable<PagedResult<Timetable>> {
    const params = new HttpParams().set('page', page.toString()).set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<Timetable>>(`${this.apiUrl}/paginated`, { params });
  }
}
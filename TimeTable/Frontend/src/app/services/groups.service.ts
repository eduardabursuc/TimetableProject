import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Group } from '../models/group.model';

@Injectable({
  providedIn: 'root'
})
export class GroupsService {
  private apiUrl = 'http://localhost:5088/api/v1/groups';

  constructor(private http: HttpClient) {}

  /**
   * Get all groups.
   * @param userEmail - The email of the user to retrieve groups for.
   * @returns Observable with a list of all groups.
   */
  getAll(userEmail: string): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.apiUrl}?userEmail=${userEmail}`);
  }

  /**
   * Get a group by ID.
   * @param id - The ID of the group to retrieve.
   * @returns Observable with the group data.
   */
  getById(id: string): Observable<Group> {
    return this.http.get<Group>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new group.
   * @param group - The group data to be created.
   * @returns Observable with the created group ID.
   */
  create(group: Group): Observable<string> {
    return this.http.post<string>(this.apiUrl, group);
  }

  /**
   * Update an existing group.
   * @param id - The ID of the group to update.
   * @param group - The updated group object.
   * @returns Observable for the update operation.
   */
  update(id: string, group: Group): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, group);
  }

  /**
   * Delete a group by ID.
   * @param id - The ID of the group to delete.
   * @returns Observable for the delete operation.
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
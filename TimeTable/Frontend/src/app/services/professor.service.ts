import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Professor } from '../models/professor.model';

@Injectable({
  providedIn: 'root',
})
export class ProfessorService {
  private apiUrl = 'http://localhost:5088/api/v1/professors';

  constructor(private http: HttpClient) {}

  /**
   * Create a new professor.
   * @param professor - The professor object to be created.
   * @returns Observable with the ID of the created professor.
   */
  create(professor: Professor): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, professor);
  }

  /**
   * Update an existing professor.
   * @param id - The ID of the professor to update.
   * @param professor - The updated professor object.
   * @returns Observable for the update operation.
   */
  update(id: string, professor: Professor): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, professor);
  }

  /**
   * Get a professor by ID.
   * @param id - The ID of the professor.
   * @returns Observable with the professor data.
   */
  getById(id: string): Observable<Professor> {
    return this.http.get<Professor>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get all professors.
   * @param userEmail - The email of the user to retrieve professors for.
   * @returns Observable with a list of all professors.
   */
  getAll(userEmail: string): Observable<Professor[]> {
    return this.http.get<Professor[]>(`${this.apiUrl}?userEmail=${userEmail}`);
  }

  /**
   * Delete a professor by ID.
   * @param id - The ID of the professor to delete.
   * @returns Observable for the delete operation.
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
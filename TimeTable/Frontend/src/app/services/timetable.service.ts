import { Injectable } from '@angular/core';
<<<<<<< Updated upstream
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
=======
import { HttpClient, HttpParams, HttpHeaders, HttpClientModule } from '@angular/common/http';
>>>>>>> Stashed changes
import { Timetable } from '../models/timetable.model';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TimetableService {
  private apiUrl = 'http://localhost:5088/api/v1/timetables';
  
  constructor(private http: HttpClient) {}

<<<<<<< Updated upstream
  create(data: { Events: any[] }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, data).pipe(
      catchError(this.handleError)
    );
=======
  private getAuthHeaders(): HttpHeaders {
    const token = this.cookieService.get('authToken');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  create(data: { userEmail: string, name: string, events: any[], timeslots: any[] }): Observable<{ id: string }> {
    const headers = this.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  update(id: string, timetable: Timetable): Observable<void> {
    const headers = this.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, timetable, { headers });
  }

  delete(userEmail: string, id: string): Observable<void> {
    const headers = this.getAuthHeaders();
    const params = new HttpParams().set('userEmail', userEmail);
    console.log('Headers:', headers);
    console.log('Params:', params);
    const url = `${this.apiUrl}/${id}`; 
    console.log('URL:', url); 
    return this.http.delete<void>(url, { headers, params });
>>>>>>> Stashed changes
  }
  

  getAll(userEmail: string): Observable<Timetable[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Timetable[]>(`${this.apiUrl}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  getById(id: string): Observable<Timetable> {
    return this.http.get<Timetable>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  update(id: string, timetable: Timetable): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, timetable).pipe(
      catchError(this.handleError)
    );
  }

  delete(userEmail: string, id: string): Observable<void> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server-side error: ${error.status} ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
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
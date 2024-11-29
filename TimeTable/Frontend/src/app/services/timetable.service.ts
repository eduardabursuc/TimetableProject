import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Timetable } from '../models/timetable.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimetableService {

  private apiURL = 'http://localhost:5088/api/v1/Timetables';

  constructor(private http: HttpClient) { }

  public getTimetables(): Observable<Timetable[]> {
    return this.http.get<Timetable[]>(this.apiURL);
  }

  public getTimetableById(id: string): Observable<Timetable> {
    return this.http.get<Timetable>(`${this.apiURL}/${id}`);
  }

  public createTimetable(timetable: Timetable): Observable<Timetable> {
    return this.http.post<Timetable>(this.apiURL, timetable);
  }
}
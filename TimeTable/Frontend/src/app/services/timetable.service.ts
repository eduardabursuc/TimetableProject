import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Timetable } from '../models/timetable.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class TimetableService {

  private apiURL = 'http://localhost:3000/timetable';

  constructor(private http: HttpClient) { }

  public getTimetables() : Observable<Timetable[]> {
    return this.http.get<Timetable[]>(this.apiURL);
  }

  public createTimetable(timetable: Timetable) : Observable<any> {
    return this.http.post<Timetable>(this.apiURL, timetable);
  }
}
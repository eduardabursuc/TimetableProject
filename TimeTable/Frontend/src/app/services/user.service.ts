import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  //private apiUrl = 'https://timetablegenerator.best/api/v1/auth';

  private apiUrl = 'http://localhost:5088/api/auth';
  constructor(private http: HttpClient) {}

   // Login method
   login(data: { email: string; password: string }): Observable<{ token: string }> {
    const url = `${this.apiUrl}/login`;
    return this.http.post<{ token: string }>(url, data);
  }

  // Register method
  register(data: { email: string; password: string, accountType: string }): Observable<{ email: string }> {
    const url = `${this.apiUrl}/register`;
    return this.http.post<{ email: string }>(url, data);
  }

}
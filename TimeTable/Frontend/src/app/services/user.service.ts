import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GlobalsService } from './globals.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient, 
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/auth`;
  }

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

  resetPassword(data: { email: string }): Observable<{ token: string}> {
    const url = `${this.apiUrl}/resetPassword`;
    return this.http.post<{ token: string }>(url, data);
  }

  changePassword(token: string, data: { email: string, newPassword: string }): Observable<{}> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    const url = `${this.apiUrl}/validateResetPassword`;
    return this.http.post<{}>(url, data, { headers });
  }
}
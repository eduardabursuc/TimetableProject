import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GlobalsService } from './globals.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl: string;

  constructor(private http: HttpClient, private globals: GlobalsService) {
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

}
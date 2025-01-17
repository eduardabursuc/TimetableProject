import { CookieService } from 'ngx-cookie-service';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from "jwt-decode";
import { Token } from '../models/token.model'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GlobalsService {

  constructor(
    private readonly cookieService: CookieService,
    private readonly http: HttpClient
  ) {}

  public apiUrl: string = 'https://timetablegenerator.best/api';
  //public apiUrl: string = 'http://localhost:5088/api';
  public flaskApiUrl: string = 'https://9bce-62-217-241-123.ngrok-free.app';

  public getAuthHeaders(): HttpHeaders {
    const token = this.cookieService.get('authToken');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  public checkToken(_token: string) {

    const token = this.decodeToken(_token);

    if (token?.exp) {
      const currentTime = Math.floor(Date.now() / 1000);
      const timeRemaining = token.exp - currentTime;

      if (timeRemaining < 15) {
          this.refreshToken({ email: token.unique_name }).subscribe({
            next: (data) => {
              this.cookieService.set("authToken", data.token);
            }
          })
      }
    }
  }
  
  private refreshToken(data: { email: string }): Observable<{ token: string }> {
    const headers = this.getAuthHeaders();
    const url = `${this.apiUrl}/refresh`;
    return this.http.post<{ token: string }>(url, data, { headers });
  }

  public decodeToken(token: string): Token {
    return jwtDecode<Token>(token, { header: false });
  }

}


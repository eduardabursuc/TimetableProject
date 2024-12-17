import { CookieService } from 'ngx-cookie-service';
import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GlobalsService {

  constructor(private cookieService: CookieService) {}

  //public apiUrl: string = 'https://timetablegenerator.best/api';
  public apiUrl: string = 'http://localhost:5088/api';

  public getAuthHeaders(): HttpHeaders {
    const token = this.cookieService.get('authToken');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }
}


import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Group } from '../models/group.model';
import { GlobalsService } from './globals.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  private readonly apiUrl: string;

  constructor(
    private readonly http: HttpClient, 
    private readonly globals: GlobalsService
  ) {
    this.apiUrl = `${this.globals.apiUrl}/v1/groups`;
  }

  create(data: { userEmail: string, name: string }): Observable<{ id: string }> {
    const headers = this.globals.getAuthHeaders();
    return this.http.post<{ id: string }>(this.apiUrl, data, { headers });
  }

  getAll(userEmail: string): Observable<Group[]> {
    const params = new HttpParams().set('userEmail', userEmail);
    return this.http.get<Group[]>(`${this.apiUrl}`, { params });
  }
  

  getById(id: string): Observable<Group> {
    return this.http.get<Group>(`${this.apiUrl}/${id}`);
  }

  update(id: string, group: Group): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.put<void>(`${this.apiUrl}/${id}`, group, { headers });
  }

  delete(id: string): Observable<void> {
    const headers = this.globals.getAuthHeaders();
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }

}
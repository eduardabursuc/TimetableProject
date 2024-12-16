import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GlobalsService {
  //public apiUrl: string = 'https://timetablegenerator.best/api';
  public apiUrl: string = 'http://localhost:5088/api';
}

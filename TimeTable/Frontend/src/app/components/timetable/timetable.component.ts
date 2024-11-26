import { Component, OnInit } from '@angular/core';
import { TimetableService } from '../../services/timetable.service';
import { Router } from '@angular/router';
import { Timetable } from '../../models/timetable.model';

@Component({
  selector: 'app-timetable',
  imports: [],
  templateUrl: './timetable.component.html',
  styleUrl: './timetable.component.css'
})
export class TimetableComponent implements OnInit {
  timetables: Timetable[] = [];
  constructor(private timetableService: TimetableService, private router: Router) { }

  ngOnInit(): void {
    this.timetableService.getTimetables().subscribe((data: Timetable[]) => {
      this.timetables = data;
      });
    }
}

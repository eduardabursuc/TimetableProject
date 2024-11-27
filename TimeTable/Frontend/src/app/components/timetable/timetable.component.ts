import { Component, OnInit } from '@angular/core';
import { TimetableService } from '../../services/timetable.service';
import { Router } from '@angular/router';
import { Timetable } from '../../models/timetable.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-timetable',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css'],
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
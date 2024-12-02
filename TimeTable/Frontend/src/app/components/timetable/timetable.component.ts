import { Component, OnInit } from '@angular/core';
import { Timetable } from '../../models/timetable.model';
import { TimetableService } from '../../services/timetable.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css'],
  imports: [RouterModule, CommonModule]
})
export class TimetableComponent implements OnInit {
  timetables: Timetable[] = [];
  currentIndex: number = 0;

  constructor(private timetableService: TimetableService) {}

  ngOnInit(): void {
    this.fetchAllTimetables();
  }

  get currentTimetable(): Timetable | null {
    return this.timetables[this.currentIndex] || null;
  }

  fetchAllTimetables(): void {
    this.timetableService.getAll().subscribe({
      next: (response) => {
        this.timetables = response;
      },
      error: (error) => {
        console.error('Failed to fetch timetables:', error);
      }
    });
  }

  nextTimetable(): void {
    if (this.currentIndex < this.timetables.length - 1) {
      this.currentIndex++;
    }
  }

  previousTimetable(): void {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }
}
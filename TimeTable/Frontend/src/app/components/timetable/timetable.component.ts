import { Component, OnInit } from '@angular/core';
import { Timetable } from '../../models/timetable.model';
import { TimetableService } from '../../services/timetable.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css'],
  imports: [RouterModule, CommonModule],
})
export class TimetableComponent implements OnInit {
  timetables: Timetable[] = []; // List of all timetables
  currentPage: number = 0; // Current page index
  pageSize: number = 7; // Number of timetables per page

  constructor(private timetableService: TimetableService) {}

  ngOnInit(): void {
    this.fetchAllTimetables();
  }

  get totalPages(): number {
    return Math.ceil(this.timetables.length / this.pageSize);
  }

  get paginatedTimetables(): Timetable[] {
    const startIndex = this.currentPage * this.pageSize;
    return this.timetables.slice(startIndex, startIndex + this.pageSize);
  }

  fetchAllTimetables(): void {
    this.timetableService.getAll("admin@gmail.com").subscribe({
      next: (response) => {
        this.timetables = response;
      },
      error: (error) => {
        console.error('Failed to fetch timetables:', error);
      },
    });
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages - 1) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 0) {
      this.currentPage--;
    }
  }
}
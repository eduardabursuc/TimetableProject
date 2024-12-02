import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TimetableService } from '../../../services/timetable.service';
import { Timetable } from '../../../models/timetable.model';
import { UpdateComponent } from '../update/update.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css'],
  imports: [UpdateComponent, CommonModule, FormsModule]
})
export class DetailComponent implements OnInit {
  timetable: Timetable | null = null;
  errorMessage: string | null = null;
  isEditMode: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private timetableService: TimetableService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = params['id'];
      if (!id) {
        this.errorMessage = 'ID is missing from the URL. Return to the previous page.';
        return;
      }
      this.getTimetableById(id);
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
  }

  getTimetableById(id: string): void {
    this.timetableService.getById(id).subscribe({
      next: (response) => {
        this.timetable = response;
        this.errorMessage = null;
      },
      error: (error) => {
        this.errorMessage = `Failed to fetch details for ID: ${id}.`;
        console.error(error);
      }
    });
  }

  deleteTimetable(id: string): void {
    if (!id) return;
    this.timetableService.delete(id).subscribe({
      next: () => {
        this.timetable = null;
        this.errorMessage = null;
        console.log('Timetable deleted successfully.');
        this.router.navigate(['/timetable']);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete timetable. Please try again.';
        console.error(error);
      }
    });
  }

  handleUpdateResult(message: string): void {
    alert(message);
    if (message === 'Timetable updated successfully!') {
      this.isEditMode = false;
      this.getTimetableById(this.timetable?.id!);
    } else {
      this.errorMessage = message;
    }
  }
}
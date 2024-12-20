import { Component, OnInit } from '@angular/core';
import { Timetable } from '../../models/timetable.model';
import { TimetableService } from '../../services/timetable.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { CookieService } from "ngx-cookie-service";

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css'],
  standalone: true, // Marks this as a standalone component
  imports: [RouterModule, CommonModule, SidebarMenuComponent, GenericModalComponent],
})
export class TimetableComponent implements OnInit {
  timetables: Timetable[] = []; // List of all timetables
  currentPage: number = 0; // Current page index
  pageSize: number = 4; // Number of timetables per page

  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';
  inputValue: string = '';
  inputPlaceholder: string = '';
  isInputRequired: boolean = false;
  modalType: 'add' | 'delete' | 'edit' | null = null;
  eventToDelete: Timetable | null = null;
  eventToEdit: Timetable | null = null;

  user: any = '';

  constructor(private timetableService: TimetableService, private router: Router, private cookieService: CookieService) {}

  ngOnInit(): void {
    const token = this.cookieService.get('authToken');
    if (!token) {
      this.router.navigate(['/login']);
    } else {
      this.user = localStorage.getItem("user");
      const role = localStorage.getItem("role");

      if( role == 'admin' ){
        this.fetchAllTimetables();
      } else if ( role == 'professor' ) {
        this.fetchAllByProfessor();
      }
      
    }

  }

  get totalPages(): number {
    return Math.ceil(this.timetables.length / this.pageSize);
  }

  get paginatedTimetables(): Timetable[] {
    const startIndex = this.currentPage * this.pageSize;
    return this.timetables.slice(startIndex, startIndex + this.pageSize);
  }

  fetchAllTimetables(): void {

    this.timetableService.getAll(this.user).subscribe({
      next: (response) => {
        // Sort by createdAt in descending order
        this.timetables = response.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
      },
      error: (error) => {
        console.error('Failed to fetch timetables:', error);
      },
    });
  }

  fetchAllByProfessor(): void {
    this.timetableService.getForProfessor(this.user).subscribe({
      next: (response) => {
        // Sort by createdAt in descending order
        this.timetables = response.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
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

  handleModalConfirm(event: { confirmed: boolean; inputValue?: string }) {
    if (event.confirmed) {
      if (this.modalType === 'add') {
        // Handle add event confirmation if needed
      } else if (this.modalType === 'delete' && this.eventToDelete) {
        // Handle delete logic
      } else if (this.modalType === 'edit' && this.eventToEdit) {
        // Handle edit logic
      }
    }
    this.isModalVisible = false;
    this.modalType = null;
  }

  navigateToDetails(timetableId: string): void {
    this.router.navigate([`/detail/${timetableId}`]);
  }

  generateTimetable(): void {
    this.router.navigate([`/create-timetable-step1`]);
  }

  isVisible(): boolean {
    const role = localStorage.getItem("role");
    if ( role == "admin" ) return true;
    return false;
  }
}

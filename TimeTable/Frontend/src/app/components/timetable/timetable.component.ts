import { Component, OnInit } from '@angular/core';
import { Timetable } from '../../models/timetable.model';
import { TimetableService } from '../../services/timetable.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { CookieService } from "ngx-cookie-service";
import { GlobalsService } from '../../services/globals.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css'],
  standalone: true,
  imports: [RouterModule, CommonModule, SidebarMenuComponent, LoadingComponent],
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

  isLoading: boolean = true;

  constructor(
    private readonly timetableService: TimetableService, 
    private readonly router: Router, 
    private readonly cookieService: CookieService,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    const token = this.cookieService.get('authToken');
    
    if (!token) {
      this.router.navigate(['/login']);
    } else {
      this.globals.checkToken(token);
      this.user = localStorage.getItem("user");
      const role = localStorage.getItem("role");

      if( role == 'admin' ){
        this.fetchAllTimetables();
      } else if ( role == 'professor' ) {
        this.fetchAllByProfessor();
      }   
    }
  }

  set totalPages(value: number) {
    this.totalPages = value;
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
        response.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.timetables = response;
      },
      error: (error) => {
        console.error('Failed to fetch timetables:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  fetchAllByProfessor(): void {
    this.timetableService.getForProfessor(this.user).subscribe({
      next: (response) => {
        response.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.timetables = response;
      },
      error: (error) => {
        console.error('Failed to fetch timetables:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
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

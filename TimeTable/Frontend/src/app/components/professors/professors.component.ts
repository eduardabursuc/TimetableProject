import { Component, OnInit } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { Professor } from '../../models/professor.model';
import { ProfessorService } from '../../services/professor.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-professors',
  templateUrl: './professors.component.html',
  styleUrls: ['./professors.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    SidebarMenuComponent,
    CommonModule,
    GenericModalComponent,
    HttpClientModule,
  ],
})
export class ProfessorsComponent implements OnInit {
  professors: Professor[] = [];
  newProfessor: Professor = {id: '', name: '', email: ''};
  professorToDelete: Professor = {id: '', name: '', email: ''};
  isAddCase: boolean = true;
  token: string = '';
  user: any = null;

  isModalVisible: boolean = false;
  cancelOption: boolean = false;
  modalType: 'delete' | 'error' | null = null;
  modalTitle: string = '';
  modalMessage: string = '';

  constructor(
    private router: Router,
    private cookieService: CookieService,
    private professorService: ProfessorService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    if (this.token === '') {
      this.router.navigate(['/login']);
    }
    this.user = localStorage.getItem('user');
    this.fetchProfessors();
  }

  fetchProfessors(): void {
    this.professorService.getAll(this.user).subscribe({
      next: (response) => {
        this.professors = response;
      },
      error: (error) => {
        console.error('Failed to fetch professors:', error);
      },
    });
  }

  isValidProfessor(): boolean {
    if (!this.newProfessor.name.trim()) {
        this.modalMessage = "Please fill in professor's name.";
        this.modalTitle = "Invalid professor";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newProfessor = {id: '', name: '', email: ''};
        return false;
      }
  
      if ( this.professors.find( g => g.name == this.newProfessor.name && g.id !== this.newProfessor.id) ) {
        this.modalMessage = "A professor with the same name already exists.";
        this.modalTitle = "Invalid professor";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newProfessor = {id: '', name: '', email: ''};
        return false;
      }

      return true;
  }

  addProfessor(): void {

    if( !this.isValidProfessor() ) return;

    const requestBody = {
      userEmail: this.user,
      name: this.newProfessor.name,
      email: this.newProfessor.email
    };

    this.professorService.create(requestBody).subscribe({
      next: (response) => {
        this.newProfessor.id = response.id;
        this.professors.push(this.newProfessor);
        this.newProfessor = {id: '', name: '', email: ''};
      },
      error: (err) => {
        console.error('Error adding professor:', err);
      },
    });
  }

  editProfessor(professor: Professor): void {
    this.newProfessor = professor;
    this.isAddCase = false;
  }

  updateProfessor(): void {

    if( !this.isValidProfessor() ) return;

    this.professorService.update(this.newProfessor.id, this.newProfessor).subscribe({
      next: () => {
        const index = this.professors.findIndex(
          (professor) => professor.id === this.newProfessor.id
        );
        if (index !== -1) {
          this.professors[index] = { ...this.newProfessor };
        }
      },
      error: (err) => {
        console.error('Error updating professor:', err);
      },
    });

    this.newProfessor = { id: '', name: '', email: ''};
    this.isAddCase = true;
  }

  showDeleteModal(professor: Professor): void {
    this.professorToDelete = professor;
    this.modalTitle = 'Delete professor';
    this.cancelOption = true;
    this.modalMessage = `Are you sure you want to delete ${professor.name} ?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  deleteProfessor(): void {
    this.professorService.delete(this.professorToDelete.id).subscribe({
      next: () => {
        this.professors = this.professors.filter((r) => r.id !== this.professorToDelete.id);
      },
      error: (err) => {
        console.error('Error deleting professor:', err);
      },
    });
  }

  handleModalConfirm(): void {
    this.isModalVisible = false;
    if ( this.modalType === 'delete' ){
        this.deleteProfessor;
    }
  }

  onBack() {
    window.history.back();
  }
  

}

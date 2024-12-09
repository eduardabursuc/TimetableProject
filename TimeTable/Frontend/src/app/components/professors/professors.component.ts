import { Component, OnInit } from '@angular/core';
import { ProfessorService } from '../../services/professor.service';
import { Professor } from '../../models/professor.model';
import { TableComponent } from '../_shared/table/table.component';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';

@Component({
  selector: 'app-professors',
  templateUrl: './professors.component.html',
  styleUrls: ['./professors.component.css'],
  imports: [TableComponent, SidebarMenuComponent]
})
export class ProfessorsComponent implements OnInit {
  professors: Professor[] = [];
  columns: { field: keyof Professor; label: string; }[] = [
    { field: 'id', label: 'ID' },
    { field: 'name', label: 'Name' },
  ];

  emptyProfessor: Professor = { id: '', name: '', userEmail: '' };

  constructor(private professorService: ProfessorService) {}

  ngOnInit(): void {
    this.loadProfessors();
  }

  loadProfessors(): void {
    this.professorService.getAll("admin@gmail.com").subscribe((data) => {
      this.professors = data;
    });
  }

  onCreate(newProfessor: Professor): void {
    this.professorService.create(newProfessor).subscribe(() => {
      this.loadProfessors();
    });
  }

  onUpdate(updatedProfessor: Professor): void {
    this.professorService.update(updatedProfessor.id, updatedProfessor).subscribe(() => {
      this.loadProfessors();
    });
  }

  onDelete(professor: Professor): void {
    this.professorService.delete(professor.id).subscribe(() => {
      this.loadProfessors();
    });
  }
}
import { Component, OnInit } from '@angular/core';
import { ProfessorsService } from '../../services/professor.service';
import { Professor } from '../../models/professor.model';
import { TableComponent } from '../_shared/table/table.component';

@Component({
  selector: 'app-professors',
  templateUrl: './professors.component.html',
  styleUrls: ['./professors.component.css'],
  imports: [TableComponent]
})
export class ProfessorsComponent implements OnInit {
  professors: Professor[] = [];
  columns: { field: keyof Professor; label: string; }[] = [
    { field: 'id', label: 'ID' },
    { field: 'name', label: 'Name' },
  ];

  emptyProfessor: Professor = { id: '', name: '', userEmail: '' };

  constructor(private professorsService: ProfessorsService) {}

  ngOnInit(): void {
    this.loadProfessors();
  }

  loadProfessors(): void {
    this.professorsService.getAll("admin@gmail.com").subscribe((data) => {
      this.professors = data;
    });
  }

  onCreate(newProfessor: Professor): void {
    this.professorsService.create(newProfessor).subscribe(() => {
      this.loadProfessors();
    });
  }

  onUpdate(updatedProfessor: Professor): void {
    this.professorsService.update(updatedProfessor.id, updatedProfessor).subscribe(() => {
      this.loadProfessors();
    });
  }

  onDelete(professor: Professor): void {
    this.professorsService.delete(professor.id).subscribe(() => {
      this.loadProfessors();
    });
  }
}
import { Component, OnInit } from '@angular/core';
import { TimetableService } from '../../services/timetable.service';
import { Timetable } from '../../models/timetable.model';
import { TableComponent } from '../_shared/table/table.component';

@Component({
  selector: 'app-timetables',
  templateUrl: './timetables.component.html',
  styleUrls: ['./timetables.component.css'],
  imports: [TableComponent]
})
export class TimetablesComponent implements OnInit {
  timetables: Timetable[] = [];
  columns: { field: keyof Timetable; label: string; }[]  = [
    { field: 'id', label: 'ID' },
    { field: 'name', label: 'Name' },
  ];
  emptyTimetable: Timetable = { id: '', userEmail: '', name: '', events: [] };

  constructor(private timetableService: TimetableService) {}

  ngOnInit(): void {
    this.loadTimetables();
  }

  loadTimetables(): void {
    this.timetableService.getAll("admin@gmail.com").subscribe((data) => {
      this.timetables = data;
    });
  }

  onCreate(newTimetable: Timetable): void {
    this.timetableService.create({ Events: newTimetable.events }).subscribe((created) => {
      this.loadTimetables();
    });
  }

  onUpdate(updatedTimetable: Timetable): void {
    this.timetableService.update(updatedTimetable.id, updatedTimetable).subscribe(() => {
      this.loadTimetables();
    });
  }

  onDelete(timetable: Timetable): void {
    this.timetableService.delete("admin@gmail.com", timetable.id).subscribe(() => {
      this.loadTimetables();
    });
  }
}
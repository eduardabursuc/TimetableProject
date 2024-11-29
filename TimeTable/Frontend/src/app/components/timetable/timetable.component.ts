import { Component, OnInit } from '@angular/core';
import { TimetableService } from '../../services/timetable.service';
import { Timetable } from '../../models/timetable.model';
import { Timeslot } from '../../models/timeslot.model';

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css']
})
export class TimetableComponent implements OnInit {
  timetables: Timetable[] = [];
  groupedTimetables: { [key: string]: Timeslot[] } = {};

  constructor(private timetableService: TimetableService) {}

  ngOnInit(): void {
    this.timetableService.createTimetable({} as Timetable).subscribe(
      (data: Timetable) => {
        this.timetables.push(data);
        this.groupTimeslotsByGroup(data.timeslots);
      }
    );
  }

  private groupTimeslotsByGroup(timeslots: Timeslot[]): void {
    this.groupedTimetables = timeslots.reduce((groups, timeslot) => {
      const group = timeslot.event.group;
      if (!groups[group]) {
        groups[group] = [];
      }
      groups[group].push(timeslot);
      return groups;
    }, {} as { [key: string]: Timeslot[] });
  }
}
import { Routes } from '@angular/router';
import { TimetableComponent } from './components/timetable/timetable.component';
import { CreateComponent } from './components/timetable/create/create.component';
import { DetailComponent } from './components/timetable/detail/detail.component';
import { CreateTimetableStep1Component } from './components/create-timetable-step1/create-timetable-step1.component';
import { CreateTimetableStep2Component } from './components/create-timetable-step2/create-timetable-step2.component';
import {TableComponent} from './components/_shared/table/table.component';

export const routes: Routes = [
  { path: '', redirectTo: 'timetable', pathMatch: 'full' },
  { path: 'timetable', component: TimetableComponent },
  { path: 'create', component: CreateComponent },
  { path: 'detail/:id', component: DetailComponent },
  { path: 'create-timetable-step1', component: CreateTimetableStep1Component },
  { path: 'create-timetable-step2', component: CreateTimetableStep2Component },
  { path: 'table', component: TableComponent },
];
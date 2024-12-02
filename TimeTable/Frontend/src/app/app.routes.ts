import { Routes } from '@angular/router';
import { TimetableComponent } from './components/timetable/timetable.component';
import { CreateComponent } from './components/timetable/create/create.component';
import { DetailComponent } from './components/timetable/detail/detail.component';

export const routes: Routes = [
  { path: '', redirectTo: 'timetable', pathMatch: 'full' },
  { path: 'timetable', component: TimetableComponent },
  { path: 'create', component: CreateComponent },
  { path: 'detail/:id', component: DetailComponent },
];
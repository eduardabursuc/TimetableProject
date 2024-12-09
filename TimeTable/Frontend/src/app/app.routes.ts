import { Routes } from '@angular/router';
import { CreateComponent } from './components/timetable/create/create.component';
import { DetailComponent } from './components/timetable/detail/detail.component';
import { CreateTimetableStep1Component } from './components/create-timetable-step1/create-timetable-step1.component';
import { CreateTimetableStep2Component } from './components/create-timetable-step2/create-timetable-step2.component';

import { TimetableComponent } from './components/timetable/timetable.component';
import { GroupsComponent } from './components/groups/groups.component';
import { RoomsComponent } from './components/rooms/rooms.component';
import { ProfessorsComponent } from './components/professors/professors.component';
import { CoursesComponent } from './components/courses/courses.component';

export const routes: Routes = [
  { path: '', redirectTo: 'timetable', pathMatch: 'full' },
  { path: 'timetable', component: TimetableComponent },
  { path: 'groups', component: GroupsComponent },
  { path: 'rooms', component: RoomsComponent },
  { path: 'professors', component: ProfessorsComponent },
  { path: 'courses', component: CoursesComponent },
  { path: 'create', component: CreateComponent },
  { path: 'detail/:id', component: DetailComponent },
  { path: 'create-timetable-step1', component: CreateTimetableStep1Component },
  { path: 'create-timetable-step2', component: CreateTimetableStep2Component },
];
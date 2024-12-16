import { Routes } from '@angular/router';
import { TimetableComponent } from './components/timetable/timetable.component';
import { DetailComponent } from './components/timetable/detail/detail.component';
import { CreateTimetableStep1Component } from './components/create-timetable-step1/create-timetable-step1.component';
import { CreateTimetableStep2Component } from './components/create-timetable-step2/create-timetable-step2.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { GroupsComponent } from './components/groups/groups.component';
import { RoomsComponent } from './components/rooms/rooms.component';
import { CoursesComponent } from './components/courses/courses.component';
import { ProfessorsComponent } from './components/professors/professors.component';

export const routes: Routes = [
  { path: '', redirectTo: 'timetable', pathMatch: 'full' },
  { path: 'timetable', component: TimetableComponent },
  { path: 'rooms', component: RoomsComponent },
  { path: 'groups', component: GroupsComponent },
  { path: 'courses', component: CoursesComponent },
  { path: 'professors', component: ProfessorsComponent },
  { path: 'detail/:id', component: DetailComponent },
  { path: 'create-timetable-step1', component: CreateTimetableStep1Component },
  { path: 'create-timetable-step2', component: CreateTimetableStep2Component },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }
];
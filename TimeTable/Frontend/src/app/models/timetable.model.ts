import { Event } from './event.model';

export interface Timetable {
  userEmail: string;
  id: string;
  name: string;
  createdAt?: Date;
  events: Event[];
}
import { Event } from './event.model';

export interface Timeslot {
  timetableId: string;
  day: string;
  time: string;
  isAvailable: boolean;
  event: Event;
  roomName: string;
}
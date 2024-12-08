import { Event } from './event.model';

export interface Timeslot {
  day: string;
  time: string;
  isAvailable: boolean;
  event: Event;
  roomName: string;
}
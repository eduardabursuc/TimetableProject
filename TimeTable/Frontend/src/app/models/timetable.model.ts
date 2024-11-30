import { Timeslot } from './timeslot.model';

export interface Timetable {
  id: string;
  timeslots: Timeslot[];
}
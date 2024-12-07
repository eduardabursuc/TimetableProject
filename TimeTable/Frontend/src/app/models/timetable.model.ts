import { Timeslot } from './timeslot.model';

export interface Timetable {
  owner: string;
  id: string;
  timeslots: Timeslot[];
}
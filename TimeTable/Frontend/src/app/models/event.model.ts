import { Timeslot } from './timeslot.model';
export interface Event {
  timetableId?: string;
  id?: string;
  eventName: string;
  courseId: string;
  professorId: string;
  groupId: string;
  duration: number;
  roomId?: string;
  timeslot?: Timeslot;
}
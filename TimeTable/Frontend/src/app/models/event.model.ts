import { Timeslot } from './timeslot.model';

export interface Event {
    timetableId: string;
    groupId: string;
    eventName: string;
    courseId: string;
    professorId: string;
    weekEvenness: boolean;
    duration: number;
    group: string;
    coursePackage: number;
    courseName: string;
    roomId: string;
    roomName: string;
    professorName: string;
    timeslot: Timeslot;
  }
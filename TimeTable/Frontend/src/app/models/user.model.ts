import { Timetable } from './timetable.model';
import { Professor } from './professor.model';
import { Room } from './room.model';
import { Course } from './course.model';
import { Group } from './group.model';

export interface User {
    email: string;
    password: string;
    accountType: string;
    timetables: Timetable[];
    professors: Professor[];
    rooms: Room[];
    courses: Course[];
    groups: Group[];
}
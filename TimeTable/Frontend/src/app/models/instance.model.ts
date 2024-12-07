import { Event } from './event.model';
import { Timeslot } from './timeslot.model';

export interface Instance {
    events: Event[];
    timeslots: Timeslot[];
}
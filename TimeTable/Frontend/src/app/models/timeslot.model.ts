export class Timeslot {
  day: string;
  time: string;
  isAvailable: boolean;
  event: Event;
  roomName: string;

  constructor(day: string, time: string, isAvailable: boolean, event: Event, roomName: string) {
    this.day = day;
    this.time = time;
    this.isAvailable = isAvailable;
    this.event = event;
    this.roomName = roomName;
  }

  get startTime(): string {
    return this.time ? this.time.split('-')[0].trim() : '';
  }

  set startTime(value: string) {
    const endTime = this.endTime;
    this.time = `${value}-${endTime}`;
  }

  get endTime(): string {
    return this.time ? this.time.split('-')[1].trim() : '';
  }

  set endTime(value: string) {
    const startTime = this.startTime;
    this.time = `${startTime}-${value}`;
  }
}

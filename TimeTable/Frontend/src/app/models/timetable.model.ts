export interface Timetable {
  userEmail: string;
  id: string;
  name: string;
  createdAt: Date;
  isPublic: boolean;
  events: any[];
}
import { Component, OnInit } from '@angular/core';
import { RoomService } from '../../services/room.service';
import { Room } from '../../models/room.model';
import { TableComponent } from '../_shared/table/table.component';

@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.css'],
  imports: [TableComponent]
})
export class RoomsComponent implements OnInit {
  rooms: Room[] = [];
  columns: { field: keyof Room; label: string; }[]  = [
    { field: 'name', label: 'Name' },
    { field: 'capacity', label: 'Capacity' },
  ];
  emptyRoom: Room = { name: '', capacity: 0, userEmail: '', id: '' };

  constructor(private roomService: RoomService) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.roomService.getAll("admin@gmail.com").subscribe((data) => {
      this.rooms = data;
    });
  }

  onCreate(newRoom: Room): void {
    this.roomService.create(newRoom).subscribe(() => {
      this.loadRooms();
    });
  }

  onUpdate(updatedRoom: Room): void {
    this.roomService.update(updatedRoom.name, updatedRoom).subscribe(() => {
      this.loadRooms();
    });
  }

  onDelete(room: Room): void {
    this.roomService.delete(room.name).subscribe(() => {
      this.loadRooms();
    });
  }
}
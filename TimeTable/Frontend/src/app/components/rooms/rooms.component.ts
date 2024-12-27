import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Room } from '../../models/room.model';
import { RoomService } from '../../services/room.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    SidebarMenuComponent,
    CommonModule,
    GenericModalComponent,
    LoadingComponent
  ],
})
export class RoomsComponent implements OnInit {
  rooms: Room[] = [];
  newRoom: Room = {id: '', name: '', capacity: 0 };
  roomToDelete: Room = {id: '', name: '', capacity: 0 };
  isAddCase: boolean = true;
  token: string = '';
  user: any = null;

  isModalVisible: boolean = false;
  cancelOption: boolean = false;
  modalType: 'delete' | 'error' | null = null;
  modalTitle: string = '';
  modalMessage: string = '';

  isLoading: boolean = true;

  constructor(
    private readonly router: Router,
    private readonly cookieService: CookieService,
    private readonly roomService: RoomService,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    this.globals.checkToken(this.token);

    if (this.token === '') {
      this.router.navigate(['/login']);
    }
    this.user = localStorage.getItem('user');
    this.fetchRooms();
  }

  fetchRooms(): void {
    this.roomService.getAll(this.user).subscribe({
      next: (response) => {
        this.rooms = response;
      },
      error: (error) => {
        console.error('Failed to fetch rooms:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  isValidRoom(): boolean {
    if (!this.newRoom.name.trim() || this.newRoom.capacity <= 0) {
        this.modalMessage = "Please fill in both fields.";
        this.modalTitle = "Invalid room";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newRoom = {id: '', name: '', capacity: 0 };
        return false;
      }
  
      if ( this.rooms.find( r => r.name == this.newRoom.name && r.id !== this.newRoom.id) ) {
        this.modalMessage = "A room with the same name already exists.";
        this.modalTitle = "Invalid room";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newRoom = {id: '', name: '', capacity: 0 };
        return false;
      }

      return true;
  }

  addRoom(): void {

    if( !this.isValidRoom() ) return;

    const requestBody = {
      userEmail: this.user,
      name: this.newRoom.name,
      capacity: this.newRoom.capacity,
    };

    this.roomService.create(requestBody).subscribe({
      next: (response) => {
        this.newRoom.id = response.id;
        this.rooms.push(this.newRoom);
        this.newRoom = {id: '', name: '', capacity: 0 };
      },
      error: (err) => {
        console.error('Error adding room:', err);
      },
    });
  }

  editRoom(room: Room): void {
    this.newRoom = room;
    this.isAddCase = false;
  }

  updateRoom(): void {

    if( !this.isValidRoom() ) return;

    this.roomService.update(this.newRoom.id, this.newRoom).subscribe({
      next: () => {
        const index = this.rooms.findIndex(
          (room) => room.id === this.newRoom.id
        );
        if (index !== -1) {
          this.rooms[index] = { ...this.newRoom };
        }
      },
      error: (err) => {
        console.error('Error updating room:', err);
      },
    });

    this.newRoom = { id: '', name: '', capacity: 0};
    this.isAddCase = true;
  }

  showDeleteModal(room: Room): void {
    this.roomToDelete = room;
    this.modalTitle = 'Delete room';
    this.cancelOption = true;
    this.modalMessage = `Are you sure you want to delete ${room.name} ?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  deleteRoom(): void {
    this.roomService.delete(this.roomToDelete.id).subscribe({
      next: () => {
        this.rooms = this.rooms.filter((r) => r.id !== this.roomToDelete.id);
      },
      error: (err) => {
        console.error('Error deleting room:', err);
      },
    });
  }

  handleModalConfirm(): void {
    this.isModalVisible = false;
    if ( this.modalType === 'delete' ){
        this.deleteRoom();
    }
  }

  onBack() {
    window.history.back();
  }
  

}

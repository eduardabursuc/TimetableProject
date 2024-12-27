import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Group } from '../../models/group.model';
import { GroupService } from '../../services/group.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';
import { CookieService } from 'ngx-cookie-service';
import { GlobalsService } from '../../services/globals.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    SidebarMenuComponent,
    CommonModule,
    GenericModalComponent,
    LoadingComponent
  ],
})
export class GroupsComponent implements OnInit {
  groups: Group[] = [];
  newGroup: Group = {id: '', name: ''};
  groupToDelete: Group = {id: '', name: ''};
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
    private readonly groupService: GroupService,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    this.token = this.cookieService.get('authToken');
    this.globals.checkToken(this.token);

    if (this.token === '') {
      this.router.navigate(['/login']);
    }
    this.user = localStorage.getItem('user');
    this.fetchGroups();
  }

  fetchGroups(): void {
    this.groupService.getAll(this.user).subscribe({
      next: (response) => {
        this.groups = response;
      },
      error: (error) => {
        console.error('Failed to fetch groups:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  isValidGroup(): boolean {
    if (!this.newGroup.name.trim()) {
        this.modalMessage = "Please fill in group's name.";
        this.modalTitle = "Invalid group";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newGroup = {id: '', name: ''};
        return false;
      }
  
      if ( this.groups.find( g => g.name == this.newGroup.name && g.id !== this.newGroup.id) ) {
        this.modalMessage = "A group with the same name already exists.";
        this.modalTitle = "Invalid group";
        this.modalType = 'error';
        this.isModalVisible = true;
        this.cancelOption = false;
        this.newGroup = {id: '', name: ''};
        return false;
      }

      return true;
  }

  addGroup(): void {

    if( !this.isValidGroup() ) return;

    const requestBody = {
      userEmail: this.user,
      name: this.newGroup.name
    };

    this.groupService.create(requestBody).subscribe({
      next: (response) => {
        this.newGroup.id = response.id;
        this.groups.push(this.newGroup);
        this.newGroup = {id: '', name: ''};
      },
      error: (err) => {
        console.error('Error adding group:', err);
      },
    });
  }

  editGroup(group: Group): void {
    this.newGroup = group;
    this.isAddCase = false;
  }

  updateGroup(): void {

    if( !this.isValidGroup() ) return;

    this.groupService.update(this.newGroup.id, this.newGroup).subscribe({
      next: () => {
        const index = this.groups.findIndex(
          (group) => group.id === this.newGroup.id
        );
        if (index !== -1) {
          this.groups[index] = { ...this.newGroup };
        }
      },
      error: (err) => {
        console.error('Error updating group:', err);
      },
    });

    this.newGroup = { id: '', name: ''};
    this.isAddCase = true;
  }

  showDeleteModal(group: Group): void {
    this.groupToDelete = group;
    this.modalTitle = 'Delete group';
    this.cancelOption = true;
    this.modalMessage = `Are you sure you want to delete ${group.name} ?`;
    this.modalType = 'delete';
    this.isModalVisible = true;
  }

  deleteGroup(): void {
    this.groupService.delete(this.groupToDelete.id).subscribe({
      next: () => {
        this.groups = this.groups.filter((r) => r.id !== this.groupToDelete.id);
      },
      error: (err) => {
        console.error('Error deleting group:', err);
      },
    });
  }

  handleModalConfirm(): void {
    this.isModalVisible = false;
    if ( this.modalType === 'delete' ){
        this.deleteGroup();
    }
  }

  onBack() {
    window.history.back();
  }
  

}

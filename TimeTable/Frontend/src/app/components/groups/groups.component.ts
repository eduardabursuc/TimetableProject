import { Component, OnInit } from '@angular/core';
import { GroupService } from '../../services/group.service';
import { Group } from '../../models/group.model';
import { TableComponent } from '../_shared/table/table.component';
import { SidebarMenuComponent } from '../sidebar-menu/sidebar-menu.component';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  imports: [TableComponent, SidebarMenuComponent]
})
export class GroupsComponent implements OnInit {
  groups: Group[] = [];
  columns: { field: keyof Group; label: string; }[] = [
    { field: 'id', label: 'ID' },
    { field: 'name', label: 'Name' },
  ];
  emptyGroup: Group = { id: '', name: '' };

  constructor(private groupService: GroupService) {}

  ngOnInit(): void {
    this.loadGroups();
  }

  loadGroups(): void {
    this.groupService.getAll("admin@gmail.com").subscribe((data) => {
      this.groups = data;
    });
  }

  onCreate(newGroup: Group): void {
    this.groupService.create(newGroup).subscribe(() => {
      this.loadGroups();
    });
  }

  onUpdate(updatedGroup: Group): void {
    this.groupService.update(updatedGroup.id, updatedGroup).subscribe(() => {
      this.loadGroups();
    });
  }

  onDelete(group: Group): void {
    this.groupService.delete(group.id).subscribe(() => {
      this.loadGroups();
    });
  }
}
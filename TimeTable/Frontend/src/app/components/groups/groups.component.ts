import { Component, OnInit } from '@angular/core';
import { GroupService } from '../../services/group.service';
import { Group } from '../../models/group.model';
import { TableComponent } from '../_shared/table/table.component';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css'],
  imports: [TableComponent]
})
export class GroupsComponent implements OnInit {
  groups: Group[] = [];
  columns: { field: keyof Group; label: string; }[] = [
    { field: 'id', label: 'ID' },
    { field: 'name', label: 'Name' },
  ];
  emptyGroup: Group = { name: '', userEmail: '', id: '' };

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
    this.groupService.update(updatedGroup.name, updatedGroup).subscribe(() => {
      this.loadGroups();
    });
  }

  onDelete(group: Group): void {
    this.groupService.delete(group.name).subscribe(() => {
      this.loadGroups();
    });
  }
}
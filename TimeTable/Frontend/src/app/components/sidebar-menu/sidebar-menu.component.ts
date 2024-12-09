import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.css']
})
export class SidebarMenuComponent {
  currentRoute: string = '';

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.updateSelectedRoute();
    });
  }

  ngOnInit() {
    this.updateSelectedRoute();
  }

  updateSelectedRoute() {
    this.currentRoute = this.router.url;
  }

  isSelected(route: string): boolean {
    if (route === '/timetables') {
      return this.currentRoute.includes('timetable');
    }
    return this.currentRoute === route;
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}

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

  // Function to update the current route based on the active URL
  updateSelectedRoute() {
    this.currentRoute = this.router.url; // Gets the current route URL
  }

  // Function to check if the route matches the menu item
  isSelected(route: string): boolean {
    // Special case for "Generate Timetable", it will match /create-timetable-step1 or /create-timetable-step2
    if (route === '/timetables') {
      return this.currentRoute.includes('create-timetable-step');
    }
    return this.currentRoute === route;  // Default case for exact matches
  }
}

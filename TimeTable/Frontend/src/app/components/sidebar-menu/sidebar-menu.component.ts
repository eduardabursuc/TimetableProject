import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from "ngx-cookie-service";
import { GenericModalComponent } from '../generic-modal/generic-modal.component';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.css'],
  imports: [GenericModalComponent]
})
export class SidebarMenuComponent {
  currentRoute: string = '';

  isModalVisible: boolean = false;
  modalTitle: string = '';
  modalMessage: string = '';

  constructor(private router: Router, private cookieService: CookieService) {
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
    if ( route === '/timetables' ) {
      return this.currentRoute.includes('timetable') || this.currentRoute.includes('detail');
    }
    return this.currentRoute === route;  // Default case for exact matches
  }

  isVisible(component: string): boolean {
    const role = localStorage.getItem("role");

    if ( component == "timetables" ) return true;
    if ( role == "admin" ) return true;
    if ( role == "professor" && component == "constraints") return true;
    return false;
  }

  logout(): void {
    this.cookieService.delete("authToken");
    localStorage.clear();
    this.router.navigate(['/login']);
  }

  logoutModal(): void {
    this.isModalVisible = true;
    this.modalMessage = "Are you sure you want to log out ?";
    this.modalTitle = "Logout";
  }

  handleModalConfirm(event: { confirmed: boolean; inputValue?: string }) {
    if (event.confirmed) {
      this.logout();
    }
    this.isModalVisible = false;
  }

  navigateTo(route: string) : void {
    this.router.navigate([route]);
  }
}

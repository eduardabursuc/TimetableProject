import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { CookieService } from "ngx-cookie-service";
import { jwtDecode } from "jwt-decode";
import { Token } from '../../models/token.model';
import { GenericModalComponent } from '../generic-modal/generic-modal.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, GenericModalComponent],
})
export class LoginComponent implements OnInit {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  isModalVisible: boolean = false;

  constructor(
    private readonly userService: UserService,
    private readonly router: Router,
    private readonly cookieService: CookieService
  ) {}

  ngOnInit(): void {
    // Check if the token exists in localStorage or cookies
    const token = this.cookieService.get('authToken');
    if (token) {
      // If token exists, redirect to /timetable
      this.router.navigate(['/timetable']);
    }
  }

  onSubmit() {
    const credentials = { email: this.email, password: this.password };
    this.userService.login(credentials).subscribe({
      next : (response) => {
        const decodedToken = jwtDecode<Token>(response.token, { header: false });
        localStorage.setItem("user", decodedToken.unique_name);
        localStorage.setItem("role", decodedToken.role);
        
        // Store the token in cookies
        this.cookieService.set('authToken', response.token, 1);

        // Navigate to /timetable
        this.router.navigate(['/timetable']);
      },
      error: (error) => {
        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password. Please try again.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
        console.error('Login error:', error);
      }
    });
  }
  
  navigateToRegister() {
    this.router.navigate(['/register']); // Navigate to the /register route
  }

  getResetPasswordModal() {
    this.isModalVisible = true;
  }

  resetPassword(event: { confirmed: boolean, inputValue?: string }) {
    if( event.confirmed && event.inputValue ) {
      this.userService.resetPassword({ email: event.inputValue }).subscribe ({
        error: (error) => {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
      });
    }
    this.isModalVisible = false;
  }
}

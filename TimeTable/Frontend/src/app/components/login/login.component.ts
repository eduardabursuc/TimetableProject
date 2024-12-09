import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { CookieService } from "ngx-cookie-service";
; // Add this import

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class LoginComponent implements OnInit {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private userService: UserService,
    private router: Router,
    private cookieService: CookieService // Inject CookieService
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
    this.userService.login(credentials).subscribe(
      (response) => {
        console.log('Login successful, token:', response.token);

        // Store the token in localStorage and cookies
        this.cookieService.set('authToken', response.token, 1);

        // Navigate to /timetable
        this.router.navigate(['/timetable']);
      },
      (error) => {
        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password. Please try again.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
        console.error('Login error:', error);
      }
    );
  }
  
  navigateToRegister() {
    this.router.navigate(['/register']); // Navigate to the /register route
  }
}

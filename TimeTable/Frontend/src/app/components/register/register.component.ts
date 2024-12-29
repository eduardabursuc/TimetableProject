import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class RegisterComponent{
  email: string = '';
  password: string = '';
  accountType: string = 'professor';
  errorMessage: string = '';

  constructor(
    private readonly userService: UserService,
    private readonly router: Router
  ) {}

  onSubmit() {
    const credentials = { email: this.email, password: this.password, accountType: this.accountType };
    this.userService.register(credentials).subscribe({
      next: (response) => {
        console.log('Success.');
        // Navigate to /login
        this.router.navigate(['/login']);
      },
      error : (error) => {
        if (error.status === 401) {
          this.errorMessage = 'User with this email already exists.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
      }
    });
  }

  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}

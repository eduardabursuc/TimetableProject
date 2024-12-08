import { Component, OnInit } from '@angular/core';
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
export class RegisterComponent implements OnInit {
  email: string = '';
  password: string = '';
  accountType: string = 'professor';
  errorMessage: string = '';

  constructor(
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
  }

  onSubmit() {
    const credentials = { email: this.email, password: this.password, accountType: this.accountType };
    this.userService.register(credentials).subscribe(
      (response) => {
        console.log('Success.');
        // Navigate to /login
        this.router.navigate(['/login']);
      },
      (error) => {
        if (error.status === 401) {
          this.errorMessage = 'User with this email already exists.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
      }
    );
  }

  navigateToLogin() {
    this.router.navigate(['/login']);
  }
}

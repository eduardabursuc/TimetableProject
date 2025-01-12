import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { CookieService } from "ngx-cookie-service";
import { GlobalsService } from '../../services/globals.service';

@Component({
  selector: 'app-login',
  templateUrl: './reset-password.component.html',
  styleUrls: ['reset-password.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class ResetPasswordComponent implements OnInit {
  newPassword: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  userEmail: string = '';
  resetToken: string | null = null;
  expired: boolean = false;

  constructor(
    private readonly userService: UserService,
    private readonly router: Router,
    private readonly cookieService: CookieService,
    private readonly route: ActivatedRoute,
    private readonly globals: GlobalsService
  ) {}

  ngOnInit(): void {
    // Check if the token exists in cookies
    const token = this.cookieService.get('authToken');
    if (token) {
      // If token exists, redirect to /timetable
      this.router.navigate(['/timetable']);
    }

    this.resetToken = this.route.snapshot.queryParamMap.get('token');
    if ( !this.resetToken ) {
      this.router.navigate(['/login']);
      return;
    }

    const tokenObj = this.globals.decodeToken(this.resetToken);

    // check if the token is expired
    const currentTime = Math.floor(Date.now() / 1000);
    const timeRemaining = tokenObj.exp - currentTime;
    if ( timeRemaining < 0 ) {
      this.expired = true;
    }
    this.userEmail = tokenObj.unique_name;
  }

  onSubmit() {

    if ( this.newPassword != this.confirmPassword ) {
      this.errorMessage = "The password does not match.";
      return;
    } 

    if ( this.resetToken )
    this.userService.changePassword(this.resetToken, { email: this.userEmail, newPassword: this.newPassword }).subscribe ({
      error: (error) => {
        this.errorMessage = 'An error occurred. Please try again later.';
      },
      complete: () => {
        this.navigateToLogin();
      }
    });

  }
  
  navigateToLogin() {
    this.router.navigate(['/login']);
  }

}

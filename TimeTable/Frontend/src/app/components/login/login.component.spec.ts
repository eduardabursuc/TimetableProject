import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;
  let cookieService: jasmine.SpyObj<CookieService>;

  beforeEach(() => {
    const spyUserService = jasmine.createSpyObj('UserService', ['login']);
    const spyRouter = jasmine.createSpyObj('Router', ['navigate']);
    const spyCookieService = jasmine.createSpyObj('CookieService', ['get', 'set']);

    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, LoginComponent],
      providers: [
        { provide: UserService, useValue: spyUserService },
        { provide: Router, useValue: spyRouter },
        { provide: CookieService, useValue: spyCookieService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    cookieService = TestBed.inject(CookieService) as jasmine.SpyObj<CookieService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should display error message on invalid login', () => {
    userService.login.and.returnValue(throwError({ status: 401 }));
    
    component.onSubmit();

    expect(component.errorMessage).toBe('Invalid email or password. Please try again.');
  });

  it('should display a generic error message for other errors', () => {
    userService.login.and.returnValue(throwError({ status: 500 }));
    
    component.onSubmit();

    expect(component.errorMessage).toBe('An error occurred. Please try again later.');
  });

  it('should navigate to /register on calling navigateToRegister', () => {
    component.navigateToRegister();

    expect(router.navigate).toHaveBeenCalledWith(['/register']);
  });

  it('should check for existing token in cookies on initialization', () => {
    cookieService.get.and.returnValue('mockToken');
    router.navigate.and.stub();

    component.ngOnInit();

    expect(router.navigate).toHaveBeenCalledWith(['/timetable']);
  });

  describe('getResetPasswordModal', () => {
    it('should set isModalVisible to true', () => {
      component.getResetPasswordModal();

      expect(component.isModalVisible).toBeTrue();
    });
  });

});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RegisterComponent } from './register.component';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const spyUserService = jasmine.createSpyObj('UserService', ['register']);
    const spyRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, RegisterComponent],  // Ensure RegisterComponent is imported
      providers: [
        { provide: UserService, useValue: spyUserService },
        { provide: Router, useValue: spyRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default values', () => {
    expect(component.email).toBe('');
    expect(component.password).toBe('');
    expect(component.accountType).toBe('professor');
    expect(component.errorMessage).toBe('');
  });

  it('should call navigateToLogin on form reset', () => {
    spyOn(component, 'navigateToLogin');

    component.navigateToLogin();

    expect(component.navigateToLogin).toHaveBeenCalled();
  });

  describe('onSubmit Method', () => {
    it('should navigate to /login on successful registration', () => {
      const mockResponse = { email: 'test@example.com' };
      userService.register.and.returnValue(of(mockResponse));
    
      component.onSubmit();
    
      expect(router.navigate).toHaveBeenCalledWith(['/login']);
    });
    

    it('should display an error message on registration failure', () => {
      userService.register.and.returnValue(throwError({ status: 401 }));

      component.onSubmit();

      expect(component.errorMessage).toBe('User with this email already exists.');
    });

    it('should display a generic error message for other errors', () => {
      userService.register.and.returnValue(throwError({ status: 500 }));

      component.onSubmit();

      expect(component.errorMessage).toBe('An error occurred. Please try again later.');
    });
  });
});

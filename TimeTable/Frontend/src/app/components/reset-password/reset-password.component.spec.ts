import { TestBed, ComponentFixture } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ResetPasswordComponent } from './reset-password.component';
import { UserService } from '../../services/user.service';
import { CookieService } from "ngx-cookie-service";
import { GlobalsService } from '../../services/globals.service';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let routerSpy = jasmine.createSpyObj('Router', ['navigate']);
  let cookieServiceSpy = jasmine.createSpyObj('CookieService', ['get']);
  let userServiceSpy = jasmine.createSpyObj('UserService', ['changePassword']);
  let globalsSpy = jasmine.createSpyObj('GlobalsService', ['decodeToken']);
  let activatedRouteSpy = {
    snapshot: {
      queryParamMap: {
        get: jasmine.createSpy('get')
      }
    }
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ResetPasswordComponent],
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: CookieService, useValue: cookieServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: GlobalsService, useValue: globalsSpy },
        { provide: ActivatedRoute, useValue: activatedRouteSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
  });

  it('should redirect to /timetable if authToken cookie exists', () => {
    cookieServiceSpy.get.and.returnValue('dummyToken');
    component.ngOnInit();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/timetable']);
  });

  it('should redirect to /login if resetToken is missing', () => {
    activatedRouteSpy.snapshot.queryParamMap.get.and.returnValue(null);
    component.ngOnInit();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should decode token and set userEmail if resetToken is valid', () => {
    const decodedToken = { exp: Math.floor(Date.now() / 1000) + 60, unique_name: 'test@example.com' };
    activatedRouteSpy.snapshot.queryParamMap.get.and.returnValue('validToken');
    globalsSpy.decodeToken.and.returnValue(decodedToken);

    component.ngOnInit();

    expect(component.expired).toBeFalse();
    expect(component.userEmail).toBe('test@example.com');
  });

  it('should set expired to true if token is expired', () => {
    const decodedToken = { exp: Math.floor(Date.now() / 1000) - 60, unique_name: 'test@example.com' };
    activatedRouteSpy.snapshot.queryParamMap.get.and.returnValue('expiredToken');
    globalsSpy.decodeToken.and.returnValue(decodedToken);

    component.ngOnInit();

    expect(component.expired).toBeTrue();
  });

  it('should show error message if passwords do not match', () => {
    component.newPassword = 'password1';
    component.confirmPassword = 'password2';
    component.onSubmit();
    expect(component.errorMessage).toBe('The password does not match.');
  });

  it('should call changePassword and navigate to /login on success', () => {
    component.newPassword = 'password';
    component.confirmPassword = 'password';
    component.resetToken = 'validToken';
    component.userEmail = 'test@example.com';

    userServiceSpy.changePassword.and.returnValue(of(null));
    spyOn(component, 'navigateToLogin');

    component.onSubmit();

    expect(userServiceSpy.changePassword).toHaveBeenCalledWith('validToken', { email: 'test@example.com', newPassword: 'password' });
    expect(component.navigateToLogin).toHaveBeenCalled();
  });

  it('should set error message on changePassword failure', () => {
    component.newPassword = 'password';
    component.confirmPassword = 'password';
    component.resetToken = 'validToken';
    component.userEmail = 'test@example.com';

    userServiceSpy.changePassword.and.returnValue(throwError(() => new Error('Error')));

    component.onSubmit();

    expect(component.errorMessage).toBe('An error occurred. Please try again later.');
  });

  it('should navigate to /login', () => {
    component.navigateToLogin();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
});

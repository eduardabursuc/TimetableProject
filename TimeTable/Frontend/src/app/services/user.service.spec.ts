import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from './user.service';
import { GlobalsService } from './globals.service';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;
  let mockGlobalsService: jasmine.SpyObj<GlobalsService>;

  const mockApiUrl = 'https://api.mock.com/auth';

  beforeEach(() => {
    mockGlobalsService = jasmine.createSpyObj('GlobalsService', [], { apiUrl: 'https://api.mock.com' });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        UserService,
        { provide: GlobalsService, useValue: mockGlobalsService },
      ],
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('login', () => {
    it('should send a POST request to login and return a token', () => {
      const loginData = { email: 'user@example.com', password: 'password123' };

      service.login(loginData).subscribe((response) => {
        expect(response.token).toBe('mock-token');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(loginData);
      req.flush({ token: 'mock-token' });
    });
  });

  describe('register', () => {
    it('should send a POST request to register a new user and return the email', () => {
      const registerData = { email: 'user@example.com', password: 'password123', accountType: 'standard' };

      service.register(registerData).subscribe((response) => {
        expect(response.email).toBe('user@example.com');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/register`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(registerData);
      req.flush({ email: 'user@example.com' });
    });
  });

  describe('resetPassword', () => {
    it('should send a POST request to reset the password and return a token', () => {
      const resetData = { email: 'user@example.com' };

      service.resetPassword(resetData).subscribe((response) => {
        expect(response.token).toBe('mock-reset-token');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/resetPassword`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(resetData);
      req.flush({ token: 'mock-reset-token' });
    });
  });

  describe('changePassword', () => {
    it('should send a POST request to change the password with an authorization token', () => {
      const token = 'mock-reset-token';
      const changePasswordData = { email: 'user@example.com', newPassword: 'newPassword123' };

      service.changePassword(token, changePasswordData).subscribe((response) => {
        expect(response).toEqual({});
      });

      const req = httpMock.expectOne(`${mockApiUrl}/validateResetPassword`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(changePasswordData);
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
      req.flush({});
    });
  });
});

import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GlobalsService } from './globals.service';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

describe('GlobalsService', () => {
  let service: GlobalsService;
  let httpMock: HttpTestingController;
  let mockCookieService: jasmine.SpyObj<CookieService>;
  const mockToken = 'mockToken';
  const mockDecodedToken = { exp: Math.floor(Date.now() / 1000) + 10, unique_name: 'test@example.com', role: 'admin' };
  const mockRefreshTokenResponse = { token: 'newMockToken' };

  beforeEach(() => {
    mockCookieService = jasmine.createSpyObj('CookieService', ['get', 'set']);
    mockCookieService.get.and.returnValue(mockToken);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        GlobalsService,
        { provide: CookieService, useValue: mockCookieService }
      ]
    });

    service = TestBed.inject(GlobalsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('getAuthHeaders', () => {
    it('should return headers with the auth token from cookies', () => {
      const headers = service.getAuthHeaders();
      expect(headers.get('Authorization')).toBe('Bearer mockToken');
    });
  });

  describe('checkToken', () => {
    it('should refresh token if expiration time is less than 15 seconds', () => {
      spyOn(service, 'decodeToken').and.returnValue(mockDecodedToken);
      mockCookieService.set.and.callFake(() => {});

      service.checkToken(mockToken);

      const req = httpMock.expectOne(`${service.apiUrl}/refresh`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email: mockDecodedToken.unique_name });
      req.flush(mockRefreshTokenResponse);

      expect(mockCookieService.set).toHaveBeenCalledWith('authToken', 'newMockToken');
    });

    it('should not refresh token if expiration time is more than 15 seconds', () => {
      const validToken = { exp: Math.floor(Date.now() / 1000) + 100, unique_name: 'test@example.com', role: 'admin' };
      spyOn(service, 'decodeToken').and.returnValue(validToken);

      service.checkToken(mockToken);

      httpMock.verify(); // Ensure no HTTP request is made
      expect(mockCookieService.set).not.toHaveBeenCalled();
    });
  });
  /*
  describe('decodeToken', () => {
    it('should decode the token and return decoded object', () => {
      spyOn(jwtDecode, 'default').and.returnValue(mockDecodedToken);

      const decoded = service.decodeToken(mockToken);

      expect(decoded).toEqual(mockDecodedToken);
      expect(jwtDecode.default).toHaveBeenCalledWith(mockToken, { header: false });
    });
  });
  */
});

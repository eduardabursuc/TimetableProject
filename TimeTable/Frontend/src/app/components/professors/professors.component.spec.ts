import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfessorsComponent } from './professors.component';
import { Router } from '@angular/router';
import { ProfessorService } from '../../services/professor.service';
import { CookieService } from 'ngx-cookie-service';
import { of, throwError } from 'rxjs';

class MockRouter {
  events = of();
  navigate = jasmine.createSpy('navigate');
}

describe('ProfessorsComponent', () => {
  let component: ProfessorsComponent;
  let fixture: ComponentFixture<ProfessorsComponent>;
  let mockRouter: MockRouter;
  let mockProfessorService: jasmine.SpyObj<ProfessorService>;
  let mockCookieService: jasmine.SpyObj<CookieService>;

  beforeEach(async () => {
    mockRouter = new MockRouter();
    mockProfessorService = jasmine.createSpyObj('ProfessorService', ['getAll', 'create', 'update', 'delete']);
    mockCookieService = jasmine.createSpyObj('CookieService', ['get']);

    await TestBed.configureTestingModule({
      imports: [ProfessorsComponent],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: ProfessorService, useValue: mockProfessorService },
        { provide: CookieService, useValue: mockCookieService },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfessorsComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
/*
  describe('ngOnInit', () => {
    it('should redirect to login if no token is present', () => {
      mockCookieService.get.and.returnValue('');
      component.ngOnInit();
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('should fetch professors if a token is present', () => {
      mockCookieService.get.and.returnValue('mockToken');
      mockProfessorService.getAll.and.returnValue(of([{ id: '1', name: 'Prof. A', email: 'a@example.com' }]));

      component.ngOnInit();

      expect(component.token).toBe('mockToken');
      expect(mockProfessorService.getAll).toHaveBeenCalledWith(component.user);
    });
  });
*/
  describe('fetchProfessors', () => {
    it('should populate professors on successful fetch', () => {
      const mockProfessors = [{ id: '1', name: 'Prof. A', email: 'a@example.com' }];
      mockProfessorService.getAll.and.returnValue(of(mockProfessors));

      component.fetchProfessors();

      expect(component.professors).toEqual(mockProfessors);
    });

    it('should handle errors when fetching professors', () => {
      spyOn(console, 'error');
      mockProfessorService.getAll.and.returnValue(throwError(() => new Error('Fetch error')));

      component.fetchProfessors();

      expect(console.error).toHaveBeenCalledWith('Failed to fetch professors:', jasmine.any(Error));
    });
  });

  describe('isValidProfessor', () => {
    beforeEach(() => {
      component.professors = [{ id: '1', name: 'Prof. A', email: 'a@example.com' }];
    });

    it('should return false if professor name is empty', () => {
      component.newProfessor = { id: '', name: '', email: '' };
      const result = component.isValidProfessor();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe("Please fill in professor's name.");
    });

    it('should return false if professor name already exists', () => {
      component.newProfessor = { id: '', name: 'Prof. A', email: 'a@example.com' };
      const result = component.isValidProfessor();
      expect(result).toBe(false);
      expect(component.modalMessage).toBe('A professor with the same name already exists.');
    });

    it('should return true for valid professor', () => {
      component.newProfessor = { id: '', name: 'Prof. B', email: 'b@example.com' };
      const result = component.isValidProfessor();
      expect(result).toBe(true);
    });
  });

  describe('addProfessor', () => {
    it('should not add professor if validation fails', () => {
      component.newProfessor = { id: '', name: '', email: '' };
      spyOn(component, 'isValidProfessor').and.returnValue(false);

      component.addProfessor();

      expect(mockProfessorService.create).not.toHaveBeenCalled();
    });

    it('should add professor if validation passes', () => {
      component.newProfessor = { id: '', name: 'Prof. C', email: 'c@example.com' };
      component.user = 'mockUser';
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      mockProfessorService.create.and.returnValue(of({ id: '2' }));

      component.addProfessor();

      expect(mockProfessorService.create).toHaveBeenCalledWith({
        userEmail: 'mockUser',
        name: 'Prof. C',
        email: 'c@example.com',
      });
      expect(component.professors.length).toBe(1);
      expect(component.professors[0].name).toBe('Prof. C');
      expect(component.newProfessor).toEqual({ id: '', name: '', email: '' });
    });
  });

  describe('updateProfessor', () => {
    it('should not update professor if validation fails', () => {
      component.newProfessor = { id: '1', name: '', email: '' };
      spyOn(component, 'isValidProfessor').and.returnValue(false);

      component.updateProfessor();

      expect(mockProfessorService.update).not.toHaveBeenCalled();
    });
    /*
    it('should update professor if validation passes', () => {
      component.newProfessor = { id: '1', name: 'Updated Prof', email: 'updated@example.com' };
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      mockProfessorService.update.and.returnValue(of());

      component.updateProfessor();

      expect(mockProfessorService.update).toHaveBeenCalledWith('1', component.newProfessor);
      expect(component.professors[0].name).toBe('Updated Prof');
      expect(component.newProfessor).toEqual({ id: '', name: '', email: '' });
    });
    */
  });
/*
  describe('deleteProfessor', () => {
    it('should delete a professor', () => {
      const professorToDelete = { id: '1', name: 'Prof. A', email: 'a@example.com' };
      component.professors = [professorToDelete];
      component.professorToDelete = professorToDelete;
      mockProfessorService.delete.and.returnValue(of());

      component.deleteProfessor();

      expect(mockProfessorService.delete).toHaveBeenCalledWith('1');
      expect(component.professors.length).toBe(0);
    });
  });
*/
  describe('handleModalConfirm', () => {
    it('should close modal and delete professor if modal type is delete', () => {
      component.modalType = 'delete';
      component.professorToDelete = { id: '1', name: 'Prof. A', email: 'a@example.com' };
      spyOn(component, 'deleteProfessor');

      component.handleModalConfirm();

      expect(component.isModalVisible).toBe(false);
      expect(component.deleteProfessor).toHaveBeenCalled();
    });
  });

  describe('onBack', () => {
    it('should go back in history', () => {
      spyOn(window.history, 'back');
      component.onBack();
      expect(window.history.back).toHaveBeenCalled();
    });
  });
});

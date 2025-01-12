import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProfessorsComponent } from './professors.component';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { ProfessorService } from '../../services/professor.service';
import { GlobalsService } from '../../services/globals.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Professor } from '../../models/professor.model';

describe('ProfessorsComponent', () => {
  let component: ProfessorsComponent;
  let fixture: ComponentFixture<ProfessorsComponent>;
  let professorServiceMock: jasmine.SpyObj<ProfessorService>;
  let cookieServiceMock: jasmine.SpyObj<CookieService>;
  let globalsServiceMock: jasmine.SpyObj<GlobalsService>;
  let routerMock: jasmine.SpyObj<Router>;

  const mockRouter = { 
    events: of(null), 
    navigate: jasmine.createSpy('navigate') 
  };

  beforeEach(() => {
    professorServiceMock = jasmine.createSpyObj('ProfessorService', ['getAll', 'create', 'update', 'delete']);
    cookieServiceMock = jasmine.createSpyObj('CookieService', ['get']);
    globalsServiceMock = jasmine.createSpyObj('GlobalsService', ['checkToken']);
    
    TestBed.configureTestingModule({
      imports: [FormsModule, ProfessorsComponent],
      providers: [
        { provide: ProfessorService, useValue: professorServiceMock },
        { provide: CookieService, useValue: cookieServiceMock },
        { provide: GlobalsService, useValue: globalsServiceMock },
        { provide: Router, useValue: mockRouter },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProfessorsComponent);
    component = fixture.componentInstance;
  });


  describe('ngOnInit', () => {
    it('should fetch professors if token is valid', () => {
      cookieServiceMock.get.and.returnValue('valid-token');
      globalsServiceMock.checkToken.and.stub();
      professorServiceMock.getAll.and.returnValue(of([]));
      
      component.ngOnInit();
      
      expect(professorServiceMock.getAll).toHaveBeenCalled();
    });
  });

  describe('addProfessor', () => {
    it('should add a valid professor', () => {
      component.newProfessor = { id: '', name: 'Jane Doe', email: 'jane@example.com' };
      component.user = 'user@example.com';
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      professorServiceMock.create.and.returnValue(of({ id: '1' }));

      component.addProfessor();

      expect(professorServiceMock.create).toHaveBeenCalledWith({
        userEmail: 'user@example.com',
        name: 'Jane Doe',
        email: 'jane@example.com'
      });
      expect(component.professors.length).toBe(1);
      expect(component.newProfessor).toEqual({ id: '', name: '', email: '' });
    });

    it('should not add an invalid professor', () => {
      spyOn(component, 'isValidProfessor').and.returnValue(false);

      component.addProfessor();

      expect(professorServiceMock.create).not.toHaveBeenCalled();
    });

    it('should log an error if adding the professor fails', () => {
      const mockError = { message: 'Request failed' };
      component.newProfessor = { id: '', name: 'Jane Doe', email: 'jane@example.com' };
      component.user = 'user@example.com';
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      professorServiceMock.create.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.addProfessor();

      expect(professorServiceMock.create).toHaveBeenCalledWith({
        userEmail: 'user@example.com',
        name: 'Jane Doe',
        email: 'jane@example.com'
      });
      expect(console.error).toHaveBeenCalledWith('Error adding professor:', mockError);
    });
  });

  describe('editProfessor', () => {
    it('should set newProfessor and isAddCase correctly', () => {
      const mockProfessor: Professor = { id: '1', name: 'John Doe', email: 'john@example.com' };

      component.editProfessor(mockProfessor);

      expect(component.newProfessor).toBe(mockProfessor);
      expect(component.isAddCase).toBeFalse();
    });
  });

  describe('updateProfessor', () => {
    it('should update the professor and update the professors list', () => {
      component.newProfessor = { id: '1', name: 'Updated Professor', email: 'updated@example.com' };
      component.professors = [{ id: '1', name: 'John Doe', email: 'john@example.com' }, { id: '2', name: 'Jane Doe', email: 'jane@example.com' }];
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      professorServiceMock.update.and.returnValue(of());

      component.updateProfessor();

      expect(professorServiceMock.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Professor', email: 'updated@example.com' });
      expect(component.newProfessor).toEqual({ id: '', name: '', email: '' });
      expect(component.isAddCase).toBeTrue();
    });

    it('should log an error if updating the professor fails', () => {
      const mockError = { message: 'Request failed' };
      component.newProfessor = { id: '1', name: 'Updated Professor', email: 'updated@example.com' };
      spyOn(component, 'isValidProfessor').and.returnValue(true);
      professorServiceMock.update.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.updateProfessor();

      expect(professorServiceMock.update).toHaveBeenCalledWith('1', { id: '1', name: 'Updated Professor', email: 'updated@example.com' });
      expect(console.error).toHaveBeenCalledWith('Error updating professor:', mockError);
    });

    it('should not update the professor if it is not valid', () => {
      component.newProfessor = { id: '1', name: 'Updated Professor', email: 'updated@example.com' };
      spyOn(component, 'isValidProfessor').and.returnValue(false);

      component.updateProfessor();

      expect(professorServiceMock.update).not.toHaveBeenCalled();
    });
  });

  describe('showDeleteModal', () => {
    it('should set modal properties and make it visible', () => {
      const mockProfessor: Professor = { id: '1', name: 'John Doe', email: 'john@example.com' };

      component.showDeleteModal(mockProfessor);

      expect(component.professorToDelete).toBe(mockProfessor);
      expect(component.modalTitle).toBe('Delete professor');
      expect(component.cancelOption).toBeTrue();
      expect(component.modalMessage).toBe('Are you sure you want to delete John Doe ?');
      expect(component.modalType).toBe('delete');
      expect(component.isModalVisible).toBeTrue();
    });
  });

  describe('deleteProfessor', () => {
    
    it('should delete a professor', () => {
      component.professors = [{ id: '1', name: 'John Doe', email: 'john@example.com' }];
      component.professorToDelete = component.professors[0];
      
      professorServiceMock.delete.and.returnValue(of());
      component.deleteProfessor();
      
      expect(professorServiceMock.delete).toHaveBeenCalledWith('1');
    });
    
    it('should handle error during deletion', () => {
      component.professors = [{ id: '1', name: 'John Doe', email: 'john@example.com' }];
      component.professorToDelete = component.professors[0];
      
      professorServiceMock.delete.and.returnValue(throwError('Error deleting'));
      component.deleteProfessor();
      
      expect(component.professors.length).toBe(1);
    });
  });

  describe('handleModalConfirm', () => {
    it('should call deleteProfessor if modal type is delete', () => {
      spyOn(component, 'deleteProfessor');
      component.modalType = 'delete';
      component.handleModalConfirm();
      
      expect(component.deleteProfessor).toHaveBeenCalled();
    });
  });
  
  describe('isValidProfessor', () => {
    it('should return false if professor name is empty', () => {
      component.newProfessor = { id: '', name: '', email: '' };
      const isValid = component.isValidProfessor();
      
      expect(isValid).toBeFalse();
      expect(component.modalMessage).toBe("Please fill in professor's name.");
    });

    it('should return false if professor name already exists', () => {
      component.professors = [{ id: '1', name: 'John Doe', email: 'john@example.com' }];
      component.newProfessor = { id: '', name: 'John Doe', email: '' };
      
      const isValid = component.isValidProfessor();
      
      expect(isValid).toBeFalse();
      expect(component.modalMessage).toBe("A professor with the same name already exists.");
    });

    it('should return true if professor name is valid and unique', () => {
      component.professors = [{ id: '1', name: 'John Doe', email: 'john@example.com' }];
      component.newProfessor = { id: '', name: 'Jane Doe', email: '' };
      
      const isValid = component.isValidProfessor();
      
      expect(isValid).toBeTrue();
    });
  });

  describe('onBack', () => {
    it('should navigate back in history', () => {
      spyOn(window.history, 'back');

      component.onBack();

      expect(window.history.back).toHaveBeenCalled();
    });
  });
});

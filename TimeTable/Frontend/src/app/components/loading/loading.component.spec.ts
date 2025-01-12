import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { LoadingComponent } from './loading.component';
import { CommonModule } from '@angular/common';

describe('LoadingComponent', () => {
  let component: LoadingComponent;
  let fixture: ComponentFixture<LoadingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [CommonModule, LoadingComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show loading', (done) => {
    component.show();
    component.loading$.subscribe((loading) => {
      expect(loading).toBeTrue();
      done();
    });
  });

  it('should hide loading', (done) => {
    component.hide();
    component.loading$.subscribe((loading) => {
      expect(loading).toBeFalse();
      done();
    });
  });
});
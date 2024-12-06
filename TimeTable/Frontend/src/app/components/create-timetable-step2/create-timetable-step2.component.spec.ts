import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTimetableStep2Component } from './create-timetable-step2.component';

describe('CreateTimetableStep2Component', () => {
  let component: CreateTimetableStep2Component;
  let fixture: ComponentFixture<CreateTimetableStep2Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTimetableStep2Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateTimetableStep2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

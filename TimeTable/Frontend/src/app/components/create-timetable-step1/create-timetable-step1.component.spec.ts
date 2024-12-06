import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTimetableStep1Component } from './create-timetable-step1.component';

describe('CreateTimetableStep1Component', () => {
  let component: CreateTimetableStep1Component;
  let fixture: ComponentFixture<CreateTimetableStep1Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTimetableStep1Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateTimetableStep1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

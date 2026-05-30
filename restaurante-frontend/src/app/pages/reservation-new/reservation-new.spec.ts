import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReservationNew } from './reservation-new';

describe('ReservationNew', () => {
  let component: ReservationNew;
  let fixture: ComponentFixture<ReservationNew>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReservationNew],
    }).compileComponents();

    fixture = TestBed.createComponent(ReservationNew);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

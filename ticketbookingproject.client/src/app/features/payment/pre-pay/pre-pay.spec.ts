import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrePay } from './pre-pay';

describe('PrePay', () => {
  let component: PrePay;
  let fixture: ComponentFixture<PrePay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrePay],
    }).compileComponents();

    fixture = TestBed.createComponent(PrePay);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

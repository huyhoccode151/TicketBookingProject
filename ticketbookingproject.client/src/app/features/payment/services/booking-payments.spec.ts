import { TestBed } from '@angular/core/testing';

import { BookingPayments } from './booking-payments';

describe('BookingPayments', () => {
  let service: BookingPayments;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BookingPayments);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

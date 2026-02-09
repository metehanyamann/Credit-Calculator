import { TestBed } from '@angular/core/testing';

import { Hesaplama } from './hesaplama';

describe('Hesaplama', () => {
  let service: Hesaplama;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Hesaplama);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

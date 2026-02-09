import { TestBed } from '@angular/core/testing';

import { KrediUrunleri } from './kredi-urunleri';

describe('KrediUrunleri', () => {
  let service: KrediUrunleri;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(KrediUrunleri);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

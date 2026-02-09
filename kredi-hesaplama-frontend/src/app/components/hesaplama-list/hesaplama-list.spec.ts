import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HesaplamaList } from './hesaplama-list';

describe('HesaplamaList', () => {
  let component: HesaplamaList;
  let fixture: ComponentFixture<HesaplamaList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HesaplamaList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HesaplamaList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

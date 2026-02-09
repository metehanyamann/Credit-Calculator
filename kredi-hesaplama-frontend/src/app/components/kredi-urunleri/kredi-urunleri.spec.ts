import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KrediUrunleri } from './kredi-urunleri';

describe('KrediUrunleri', () => {
  let component: KrediUrunleri;
  let fixture: ComponentFixture<KrediUrunleri>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KrediUrunleri]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KrediUrunleri);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

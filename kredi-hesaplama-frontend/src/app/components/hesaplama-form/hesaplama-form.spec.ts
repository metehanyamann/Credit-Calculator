import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HesaplamaFormComponent } from './hesaplama-form';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { GeminiService } from '../../services/gemini';

describe('HesaplamaFormComponent', () => {
  let component: HesaplamaFormComponent;
  let fixture: ComponentFixture<HesaplamaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HesaplamaFormComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule
      ],
      providers: [
        {
          provide: GeminiService,
          useValue: {
            generate: jasmine.createSpy('generate').and.returnValue(Promise.resolve('Test cevabı'))
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HesaplamaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

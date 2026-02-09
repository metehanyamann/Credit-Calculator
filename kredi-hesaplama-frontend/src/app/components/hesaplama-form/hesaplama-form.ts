import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { GroqService } from '../../services/groq'; // Injectable servis

@Component({
  selector: 'app-hesaplama-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './hesaplama-form.html',
  styleUrl: './hesaplama-form.scss'
})
export class HesaplamaFormComponent implements OnInit {
  hesaplamaForm: FormGroup;
  krediTurleri = [
    { ad: 'İhtiyaç Kredisi', value: 'ihtiyaç kredisi' },
    { ad: 'Taşıt Kredisi', value: 'taşıt kredisi' },
    { ad: 'Konut Kredisi', value: 'konut kredisi' }
  ];
  faizOrani: number | null = null;
  sonuc: any = null;
  odemePlani: any[] = [];
  hata: string = '';
  loading = false;

  groqYanit: string = ''; // 
  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private groqService: GroqService // <-- Inject edilen servis
  ) {
    this.hesaplamaForm = this.fb.group({
      krediTipi: ['', Validators.required],
      tutar: [null, [Validators.required]],
      vade: [null, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.hesaplamaForm.get('krediTipi')?.valueChanges.subscribe(() => {
      this.faizOrani = null;
      this.hesaplamaForm.patchValue({ tutar: null, vade: null });
      this.sonuc = null;
      this.odemePlani = [];
      this.hata = '';
    });

    this.hesaplamaForm.get('vade')?.valueChanges.subscribe(() => {
      this.faizOrani = this.getFaizOrani();
    });
  }

  getFaizOrani(): number | null {
    const krediTipi = this.hesaplamaForm.get('krediTipi')?.value;
    const vade = this.hesaplamaForm.get('vade')?.value;
    if (krediTipi === 'ihtiyaç kredisi') return 4.99;
    if (krediTipi === 'taşıt kredisi') return 3.84;
    if (krediTipi === 'konut kredisi') {
      if (!vade) return null;
      return vade <= 60 ? 2.99 : 2.89;
    }
    return null;
  }

  getTutarLimits() {
    const krediTipi = this.hesaplamaForm.get('krediTipi')?.value;
    if (krediTipi === 'ihtiyaç kredisi') return { min: 3000, max: 250000 };
    if (krediTipi === 'konut kredisi') return { min: 100000, max: 8500000 };
    return { min: 1, max: 10000000 };
  }

  getVadeLimits() {
    const krediTipi = this.hesaplamaForm.get('krediTipi')?.value;
    const tutar = this.hesaplamaForm.get('tutar')?.value;
    if (krediTipi === 'ihtiyaç kredisi') return { min: 3, max: 36 };
    if (krediTipi === 'konut kredisi') return { min: 3, max: 120 };
    if (krediTipi === 'taşıt kredisi') {
      if (!tutar) return { min: 3, max: 48 };
      if (tutar <= 400000) return { min: 3, max: 48 };
      if (tutar <= 800000) return { min: 3, max: 36 };
      if (tutar <= 1200000) return { min: 3, max: 24 };
      return { min: 3, max: 12 };
    }
    return { min: 1, max: 120 };
  }

  hesapla() {
    this.sonuc = null;
    this.odemePlani = [];
    this.hata = '';
    this.groqYanit = '';
    if (this.hesaplamaForm.invalid) return;
    this.loading = true;

    const formData = {
      krediTipi: this.hesaplamaForm.value.krediTipi,
      tutar: this.hesaplamaForm.value.tutar,
      vade: this.hesaplamaForm.value.vade,
      faiz: this.getFaizOrani()
    };

    this.http.post<any>('http://localhost:5090/api/Hesaplama', formData).subscribe({
      next: async (res) => {
        this.sonuc = res;
        this.odemePlani = res.odemePlani || [];
        this.loading = false;

        const prompt = `Kullanıcı ${formData.krediTipi} seçti. Tutar: ${formData.tutar} TL, Vade: ${formData.vade} ay, Faiz: ${formData.faiz}%. Bu kredi hakkında kısa bir bilgilendirme yap.`;
        this.groqYanit = await this.groqService.generate(prompt);
      },
      error: (err) => {
        console.error('API hatası:', err);
        this.hata = err.error?.hata || 'Bir hata oluştu.';
        this.loading = false;
      }
    });
  }

  resetForm() {
    this.hesaplamaForm.reset({ krediTipi: '', tutar: null, vade: null });
    this.sonuc = null;
    this.odemePlani = [];
    this.hata = '';
    this.groqYanit = '';
  }
}

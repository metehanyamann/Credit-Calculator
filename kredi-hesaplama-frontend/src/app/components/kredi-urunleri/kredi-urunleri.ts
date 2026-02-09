import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { KrediUrunleriService } from '../../services/kredi-urunleri';
import { KrediUrunu } from '../../models/kredi-urun.model';

@Component({
  selector: 'app-kredi-urunleri',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './kredi-urunleri.html',
  styleUrl: './kredi-urunleri.scss'
})
export class KrediUrunleriComponent implements OnInit {
  krediUrunleri: KrediUrunu[] = [];
  urunForm: FormGroup;
  loading = false;
  error = '';
  editingId: number | null = null;

  constructor(
    private krediUrunleriService: KrediUrunleriService,
    private fb: FormBuilder
  ) {
    this.urunForm = this.fb.group({
      urunAdi: ['', Validators.required],
      faizOrani: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      minTutar: [0, [Validators.required, Validators.min(0)]],
      maxTutar: [0, [Validators.required, Validators.min(0)]],
      minVade: [12, [Validators.required, Validators.min(1), Validators.max(120)]],
      maxVade: [120, [Validators.required, Validators.min(1), Validators.max(120)]]
    });
  }

  ngOnInit(): void {
    this.loadKrediUrunleri();
  }

  loadKrediUrunleri(): void {
    this.loading = true;
    this.error = '';
    
    this.krediUrunleriService.getAll().subscribe({
      next: (urunler) => {
        this.krediUrunleri = urunler;
        this.loading = false;
      },
      error: (error) => {
        console.error('Kredi ürünleri yüklenirken hata:', error);
        this.error = 'Kredi ürünleri yüklenirken bir hata oluştu.';
        this.loading = false;
      }
    });
  }

  addUrun(): void {
    if (this.urunForm.valid) {
      const urun: KrediUrunu = this.urunForm.value;
      
      this.krediUrunleriService.add(urun).subscribe({
        next: (newUrun) => {
          this.krediUrunleri.push(newUrun);
          this.resetForm();
        },
        error: (error) => {
          console.error('Ürün eklenirken hata:', error);
          this.error = 'Ürün eklenirken bir hata oluştu.';
        }
      });
    }
  }

  editUrun(urun: KrediUrunu): void {
    this.editingId = urun.id!;
    this.urunForm.patchValue(urun);
  }

  updateUrun(): void {
    if (this.urunForm.valid && this.editingId) {
      const urun: KrediUrunu = { ...this.urunForm.value, id: this.editingId };
      
      this.krediUrunleriService.update(this.editingId, urun).subscribe({
        next: () => {
          const index = this.krediUrunleri.findIndex(u => u.id === this.editingId);
          if (index !== -1) {
            this.krediUrunleri[index] = urun;
          }
          this.resetForm();
        },
        error: (error) => {
          console.error('Ürün güncellenirken hata:', error);
          this.error = 'Ürün güncellenirken bir hata oluştu.';
        }
      });
    }
  }

  deleteUrun(id: number): void {
    if (confirm('Bu ürünü silmek istediğinizden emin misiniz?')) {
      this.krediUrunleriService.delete(id).subscribe({
        next: () => {
          this.krediUrunleri = this.krediUrunleri.filter(u => u.id !== id);
        },
        error: (error) => {
          console.error('Ürün silinirken hata:', error);
          this.error = 'Ürün silinirken bir hata oluştu.';
        }
      });
    }
  }

  cancelEdit(): void {
    this.editingId = null;
    this.resetForm();
  }

  resetForm(): void {
    this.urunForm.reset({
      urunAdi: '',
      faizOrani: 0,
      minTutar: 0,
      maxTutar: 0,
      minVade: 12,
      maxVade: 120
    });
    this.editingId = null;
  }

  isEditing(): boolean {
    return this.editingId !== null;
  }
}

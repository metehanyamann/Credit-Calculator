import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HesaplamaService } from '../../services/hesaplama';
import { Hesaplama } from '../../models/hesaplama.model';

@Component({
  selector: 'app-hesaplama-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hesaplama-list.html',
  styleUrl: './hesaplama-list.scss'
})
export class HesaplamaListComponent implements OnInit {
  hesaplamalar: Hesaplama[] = [];
  loading = false;
  error = '';

  constructor(private hesaplamaService: HesaplamaService) { }

  ngOnInit(): void {
    this.loadHesaplamalar();
  }

  loadHesaplamalar(): void {
    this.loading = true;
    this.error = '';
    
    this.hesaplamaService.getAll().subscribe({
      next: (hesaplamalar) => {
        this.hesaplamalar = hesaplamalar;
        this.loading = false;
      },
      error: (error) => {
        console.error('Hesaplamalar yüklenirken hata:', error);
        this.error = 'Hesaplamalar yüklenirken bir hata oluştu.';
        this.loading = false;
      }
    });
  }

  formatDate(date: Date | string): string {
    return new Date(date).toLocaleDateString('tr-TR');
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('tr-TR', {
      style: 'currency',
      currency: 'TRY'
    }).format(amount);
  }
}

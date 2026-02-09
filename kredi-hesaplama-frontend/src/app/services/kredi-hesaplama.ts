import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface VadeHesaplamaRequest {
  krediTipi: string;
  aylikGelir: number;
  krediTutari: number;
}

export interface VadeHesaplamaResponse {
  krediTipi: string;
  optimalVade: number;
  aylikTaksit: number;
  toplamOdeme: number;
  faizOrani: number;
  basarili: boolean;
  mesaj: string;
}

@Injectable({
  providedIn: 'root'
})
export class KrediHesaplamaService {
  private readonly API_URL = 'http://localhost:5090/api/kredi';

  constructor(private http: HttpClient) {}

  hesaplaOptimalVade(request: VadeHesaplamaRequest): Observable<VadeHesaplamaResponse> {
    return this.http.post<VadeHesaplamaResponse>(`${this.API_URL}/vade-hesapla`, request);
  }
} 
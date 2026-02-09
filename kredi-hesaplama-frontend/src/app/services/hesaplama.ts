import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Hesaplama, HesaplamaRequest } from '../models/hesaplama.model';

@Injectable({
  providedIn: 'root'
})
export class HesaplamaService {
  private apiUrl = 'http://localhost:5090/api/Hesaplama';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Hesaplama[]> {
    return this.http.get<Hesaplama[]>(this.apiUrl);
  }

  getById(id: number): Observable<Hesaplama> {
    return this.http.get<Hesaplama>(`${this.apiUrl}/${id}`);
  }

  add(hesaplama: HesaplamaRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, hesaplama);
  }

  calculateMonthlyPayment(tutar: number, vade: number, faiz: number): number {
    const monthlyRate = faiz / 100 / 12;
    const numberOfPayments = vade;
    
    if (monthlyRate === 0) {
      return tutar / numberOfPayments;
    }
    
    const monthlyPayment = tutar * (monthlyRate * Math.pow(1 + monthlyRate, numberOfPayments)) / 
                          (Math.pow(1 + monthlyRate, numberOfPayments) - 1);
    
    return Math.round(monthlyPayment * 100) / 100;
  }

  calculateTotalPayment(monthlyPayment: number, vade: number): number {
    return Math.round(monthlyPayment * vade * 100) / 100;
  }
}

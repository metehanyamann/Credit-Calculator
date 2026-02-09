import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { KrediUrunu } from '../models/kredi-urun.model';

@Injectable({
  providedIn: 'root'
})
export class KrediUrunleriService {
  private apiUrl = 'http://localhost:5090/api/KrediUrunu';

  constructor(private http: HttpClient) { }

  getAll(): Observable<KrediUrunu[]> {
    return this.http.get<KrediUrunu[]>(this.apiUrl);
  }

  getById(id: number): Observable<KrediUrunu> {
    return this.http.get<KrediUrunu>(`${this.apiUrl}/${id}`);
  }

  add(urun: KrediUrunu): Observable<KrediUrunu> {
    return this.http.post<KrediUrunu>(this.apiUrl, urun);
  }

  update(id: number, urun: KrediUrunu): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, urun);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}

export interface Hesaplama {
  id?: number;
  krediTipi: string;
  tutar: number;
  vade: number;
  faiz: number;
  aylikOdeme: number;
  toplamOdeme: number;
  odemePlaniJson?: string;
  hesaplamaTarihi?: Date;
}

export interface HesaplamaRequest {
  krediTipi: string;
  tutar: number;
  vade: number;
  faiz: number;
} 
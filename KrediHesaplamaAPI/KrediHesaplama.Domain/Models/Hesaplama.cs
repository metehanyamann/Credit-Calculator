using System;

namespace KrediHesaplamaAPI.Models
{
    public class Hesaplama
    {
        public int Id { get; set; }

        public string KrediTipi { get; set; } = string.Empty;

        public decimal Tutar { get; set; }

        public int Vade { get; set; }

        public double Faiz { get; set; }

        public decimal AylikOdeme { get; set; }

        public decimal ToplamOdeme { get; set; }

        public string? OdemePlaniJson { get; set; }

        public DateTime HesaplamaTarihi { get; set; } = DateTime.Now;
    }
}

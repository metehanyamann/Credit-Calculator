using KrediHesaplama.Infrastructure.Interfaces;
using KrediHesaplamaAPI.Models;
using KrediHesaplama.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KrediHesaplama.Application.Services
{
    public class HesaplamaService : IHesaplamaService
    {
        private readonly AppDbContext _context;

        public HesaplamaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Hesaplama>> GetAllAsync()
        {
            return await _context.Hesaplamalar.ToListAsync();
        }

        public async Task<Hesaplama?> GetByIdAsync(int id)
        {
            return await _context.Hesaplamalar.FindAsync(id);
        }

        public async Task<int> AddAsync(Hesaplama hesaplama)
        {
            // Kredi türüne göre kurallar ve faiz oranı
            double faiz = 0;
            int minVade = 0, maxVade = 0;
            decimal minTutar = 0, maxTutar = 0;
            string krediTipi = hesaplama.KrediTipi.ToLower();

            if (krediTipi == "ihtiyaç kredisi")
            {
                faiz = 4.99;
                minTutar = 3000; maxTutar = 250000;
                minVade = 3; maxVade = 36;
                if (hesaplama.Tutar < minTutar || hesaplama.Tutar > maxTutar || hesaplama.Vade < minVade || hesaplama.Vade > maxVade)
                    throw new Exception("İhtiyaç kredisi için tutar veya vade sınırları dışında değer girildi.");
            }
            else if (krediTipi == "taşıt kredisi")
            {
                faiz = 3.84;
                decimal tutar = hesaplama.Tutar;
                if (tutar <= 400_000) maxVade = 48;
                else if (tutar <= 800_000) maxVade = 36;
                else if (tutar <= 1_200_000) maxVade = 24;
                else maxVade = 12;
                minVade = 3;
                if (hesaplama.Vade < minVade || hesaplama.Vade > maxVade)
                    throw new Exception($"Taşıt kredisi için bu tutarda maksimum vade {maxVade} aydır.");
            }
            else if (krediTipi == "konut kredisi")
            {
                minTutar = 100000; maxTutar = 8500000;
                minVade = 3; maxVade = 120;
                if (hesaplama.Tutar < minTutar || hesaplama.Tutar > maxTutar || hesaplama.Vade < minVade || hesaplama.Vade > maxVade)
                    throw new Exception("Konut kredisi için tutar veya vade sınırları dışında değer girildi.");
                faiz = hesaplama.Vade <= 60 ? 2.99 : 2.89;
            }
            else
            {
                throw new Exception("Geçersiz kredi tipi.");
            }

            // Hesaplama
            double aylikFaiz = faiz / 100;
            int vade = hesaplama.Vade;
            double tutarD = (double)hesaplama.Tutar;
            double aylikOdeme = (tutarD * aylikFaiz) / (1 - Math.Pow(1 + aylikFaiz, -vade));
            aylikOdeme = Math.Round(aylikOdeme, 2);
            double toplamOdeme = Math.Round(aylikOdeme * vade, 2);

            // Ödeme planı
            var odemePlani = new List<OdemePlaniSatiri>();
            double kalanAnaPara = tutarD;
            for (int ay = 1; ay <= vade; ay++)
            {
                double faizTutari = kalanAnaPara * aylikFaiz;
                double anapara = aylikOdeme - faizTutari;
                kalanAnaPara -= anapara;
                odemePlani.Add(new OdemePlaniSatiri
                {
                    Ay = ay,
                    Tutar = Math.Round(aylikOdeme, 2),
                    Anapara = Math.Round(anapara, 2),
                    Faiz = Math.Round(faizTutari, 2),
                    KalanAnaPara = Math.Max(0, Math.Round(kalanAnaPara, 2))
                });
            }

            hesaplama.Faiz = faiz;
            hesaplama.AylikOdeme = (decimal)aylikOdeme;
            hesaplama.ToplamOdeme = (decimal)toplamOdeme;
            hesaplama.OdemePlaniJson = JsonSerializer.Serialize(odemePlani);
            hesaplama.HesaplamaTarihi = DateTime.Now;

            _context.Hesaplamalar.Add(hesaplama);
            await _context.SaveChangesAsync();
            return hesaplama.Id;
        }
    }
}

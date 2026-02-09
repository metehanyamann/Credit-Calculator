using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KrediHesaplama.Application.Services;
using KrediHesaplama.Domain.Models;
using KrediHesaplama.Infrastructure.Interfaces;
using KrediHesaplama.Application;

namespace KrediHesaplamaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KrediController : ControllerBase
    {
        private readonly KonutKrediService _konutKrediService;
        private readonly IhtiyacKrediService _ihtiyacKrediService;

        public KrediController(KonutKrediService konutKrediService, IhtiyacKrediService ihtiyacKrediService)
        {
            _konutKrediService = konutKrediService;
            _ihtiyacKrediService = ihtiyacKrediService;
        }

        [HttpGet("deneme")]
        public IActionResult Deneme()
        {
            return Ok("API çalışıyor !");
        }

        [HttpPost("vade-hesapla")]
        public IActionResult OptimalVadeHesapla([FromBody] VadeHesaplamaRequest request)
        {
            try
            {
                if (request.AylikGelir <= 0 || request.KrediTutari <= 0)
                {
                    return BadRequest("Aylık gelir ve kredi tutarı pozitif olmalıdır.");
                }

                var optimalVade = new VadeHesaplamaResponse();

                if (request.KrediTipi.ToLower() == "konut")
                {
                    optimalVade = HesaplaOptimalKonutVadesi(request.AylikGelir, request.KrediTutari);
                }
                else if (request.KrediTipi.ToLower() == "ihtiyaç" || request.KrediTipi.ToLower() == "ihtiyac")
                {
                    optimalVade = HesaplaOptimalIhtiyacVadesi(request.AylikGelir, request.KrediTutari);
                }
                else if (request.KrediTipi.ToLower() == "taşıt" || request.KrediTipi.ToLower() == "tasit")
                {
                    optimalVade = HesaplaOptimalTasitVadesi(request.AylikGelir, request.KrediTutari);
                }
                else
                {
                    return BadRequest("Geçersiz kredi türü. 'Konut', 'İhtiyaç' veya 'Taşıt' kredisi seçebilirsiniz.");
                }

                return Ok(optimalVade);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Hesaplama sırasında bir hata oluştu.", details = ex.Message });
            }
        }

        private VadeHesaplamaResponse HesaplaOptimalKonutVadesi(double aylikGelir, double krediTutari)
        {
            // Konut kredisi kuralları: 100000-8500000 TL, 3-120 ay vade
            if (krediTutari < 100000 || krediTutari > 8500000)
            {
                return new VadeHesaplamaResponse
                {
                    Basarili = false,
                    Mesaj = $"Konut kredisi için kredi tutarı 100.000 TL ile 8.500.000 TL arasında olmalıdır. Girdiğiniz tutar: {krediTutari:N2} TL"
                };
            }

            double maxTaksit = aylikGelir * 0.5; // Maksimum aylık taksit gelirin %50'si
            int maxVade = 120; // Konut kredisi maksimum 120 ay
            int minVade = 3; // Minimum 3 ay

            // Faiz oranını belirle (60 aya kadar 2.99, sonrası 2.89)
            double faizOrani = 0.0299; // Varsayılan %2.99

            var optimalVade = UygunVadeBul(krediTutari, faizOrani, maxTaksit, maxVade, minVade);

            // Eğer uygun vade bulunamadıysa
            if (optimalVade > maxVade)
            {
                return new VadeHesaplamaResponse
                {
                    Basarili = false,
                    Mesaj = $"Bu kredi tutarı ({krediTutari:N2} TL) sizin aylık gelirinizle ({aylikGelir:N2} TL) orantısız. Maksimum aylık ödeme gücünüz: {maxTaksit:N2} TL. Daha düşük bir kredi tutarı veya daha yüksek bir gelir öneriyoruz."
                };
            }

            // Optimal vadeye göre faiz oranını güncelle
            if (optimalVade > 60)
            {
                faizOrani = 0.0289; // %2.89
            }

            double aylikTaksit = HesaplaAylikTaksit(krediTutari, faizOrani, optimalVade);
            double toplamOdeme = aylikTaksit * optimalVade;

            return new VadeHesaplamaResponse
            {
                KrediTipi = "Konut Kredisi",
                OptimalVade = optimalVade,
                AylikTaksit = Math.Round(aylikTaksit, 2),
                ToplamOdeme = Math.Round(toplamOdeme, 2),
                FaizOrani = optimalVade > 60 ? 2.89 : 2.99,
                Basarili = true,
                Mesaj = $"Konut kredisi için optimal vade: {optimalVade} ay. Aylık taksit: {Math.Round(aylikTaksit, 2)} TL, Toplam ödeme: {Math.Round(toplamOdeme, 2)} TL"
            };
        }

        private VadeHesaplamaResponse HesaplaOptimalIhtiyacVadesi(double aylikGelir, double krediTutari)
        {
            // İhtiyaç kredisi kuralları: 3000-250000 TL, 3-36 ay vade, %4.99 faiz
            if (krediTutari < 3000 || krediTutari > 250000)
            {
                return new VadeHesaplamaResponse
                {
                    Basarili = false,
                    Mesaj = $"İhtiyaç kredisi için kredi tutarı 3.000 TL ile 250.000 TL arasında olmalıdır. Girdiğiniz tutar: {krediTutari:N2} TL"
                };
            }

            double maxTaksit = aylikGelir * 0.5;
            double faizOrani = 0.0499; // %4.99 yıllık faiz
            int maxVade = 36; // İhtiyaç kredisi maksimum 36 ay
            int minVade = 3; // Minimum 3 ay

            var optimalVade = UygunVadeBul(krediTutari, faizOrani, maxTaksit, maxVade, minVade);

            // Eğer uygun vade bulunamadıysa
            if (optimalVade > maxVade)
            {
                return new VadeHesaplamaResponse
                {
                    Basarili = false,
                    Mesaj = $"Bu kredi tutarı ({krediTutari:N2} TL) sizin aylık gelirinizle ({aylikGelir:N2} TL) orantısız. Maksimum aylık ödeme gücünüz: {maxTaksit:N2} TL. Daha düşük bir kredi tutarı veya daha yüksek bir gelir öneriyoruz."
                };
            }

            double aylikTaksit = HesaplaAylikTaksit(krediTutari, faizOrani, optimalVade);
            double toplamOdeme = aylikTaksit * optimalVade;

            return new VadeHesaplamaResponse
            {
                KrediTipi = "İhtiyaç Kredisi",
                OptimalVade = optimalVade,
                AylikTaksit = Math.Round(aylikTaksit, 2),
                ToplamOdeme = Math.Round(toplamOdeme, 2),
                FaizOrani = 4.99,
                Basarili = true,
                Mesaj = $"İhtiyaç kredisi için optimal vade: {optimalVade} ay. Aylık taksit: {Math.Round(aylikTaksit, 2)} TL, Toplam ödeme: {Math.Round(toplamOdeme, 2)} TL"
            };
        }

        private VadeHesaplamaResponse HesaplaOptimalTasitVadesi(double aylikGelir, double krediTutari)
        {
            // Taşıt kredisi kuralları: Kasko değerine göre vade belirleme
            int maxVade;
            if (krediTutari <= 400000)
            {
                maxVade = 48;
            }
            else if (krediTutari <= 800000)
            {
                maxVade = 36;
            }
            else if (krediTutari <= 1200000)
            {
                maxVade = 24;
            }
            else
            {
                maxVade = 12;
            }

            double maxTaksit = aylikGelir * 0.5;
            double faizOrani = 0.0384; // %3.84 yıllık faiz
            int minVade = 3; // Minimum 3 ay

            var optimalVade = UygunVadeBul(krediTutari, faizOrani, maxTaksit, maxVade, minVade);

            // Eğer uygun vade bulunamadıysa
            if (optimalVade > maxVade)
            {
                return new VadeHesaplamaResponse
                {
                    Basarili = false,
                    Mesaj = $"Bu kredi tutarı ({krediTutari:N2} TL) sizin aylık gelirinizle ({aylikGelir:N2} TL) orantısız. Maksimum aylık ödeme gücünüz: {maxTaksit:N2} TL. Daha düşük bir kredi tutarı veya daha yüksek bir gelir öneriyoruz."
                };
            }

            double aylikTaksit = HesaplaAylikTaksit(krediTutari, faizOrani, optimalVade);
            double toplamOdeme = aylikTaksit * optimalVade;

            string vadeAciklama = "";
            if (krediTutari <= 400000)
            {
                vadeAciklama = "400.000 TL ve altı araçlar için maksimum 48 ay vade";
            }
            else if (krediTutari <= 800000)
            {
                vadeAciklama = "400.001-800.000 TL arası araçlar için maksimum 36 ay vade";
            }
            else if (krediTutari <= 1200000)
            {
                vadeAciklama = "800.001-1.200.000 TL arası araçlar için maksimum 24 ay vade";
            }
            else
            {
                vadeAciklama = "1.200.000 TL üzeri araçlar için maksimum 12 ay vade";
            }

            return new VadeHesaplamaResponse
            {
                KrediTipi = "Taşıt Kredisi",
                OptimalVade = optimalVade,
                AylikTaksit = Math.Round(aylikTaksit, 2),
                ToplamOdeme = Math.Round(toplamOdeme, 2),
                FaizOrani = 3.84,
                Basarili = true,
                Mesaj = $"Taşıt kredisi için optimal vade: {optimalVade} ay. ({vadeAciklama}) Aylık taksit: {Math.Round(aylikTaksit, 2)} TL, Toplam ödeme: {Math.Round(toplamOdeme, 2)} TL"
            };
        }

        private int UygunVadeBul(double krediTutari, double aylikFaizOrani, double maxTaksit, int maxVade, int minVade)
        {
            // En düşük vadeden başlayarak ödeme gücüne uygun vadeyi bul
            for (int vade = minVade; vade <= maxVade; vade++)
            {
                double taksit = HesaplaAylikTaksit(krediTutari, aylikFaizOrani, vade);
                if (taksit <= maxTaksit)
                {
                    return vade;
                }
            }
            
            // Hiçbir vade uygun değilse, en uzun vadeyi döndür ama başarısız olarak işaretle
            return maxVade + 1; // Başarısız olduğunu belirtmek için
        }

        private double HesaplaAylikTaksit(double krediTutari, double aylikFaizOrani, int vade)
        {
            if (aylikFaizOrani == 0)
                return krediTutari / vade;

            double payda = Math.Pow(1 + aylikFaizOrani, vade) - 1;
            if (payda == 0)
                return krediTutari / vade;

            return krediTutari * (aylikFaizOrani * Math.Pow(1 + aylikFaizOrani, vade)) / payda;
        }
    }

    public class VadeHesaplamaRequest
    {
        public string KrediTipi { get; set; } = string.Empty;
        public double AylikGelir { get; set; }
        public double KrediTutari { get; set; }
    }

    public class VadeHesaplamaResponse
    {
        public string KrediTipi { get; set; } = string.Empty;
        public int OptimalVade { get; set; }
        public double AylikTaksit { get; set; }
        public double ToplamOdeme { get; set; }
        public double FaizOrani { get; set; }
        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = string.Empty;
    }
}

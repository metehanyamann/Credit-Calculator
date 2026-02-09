using KrediHesaplama.Domain.Models;
using KrediHesaplama.Infrastructure.Interfaces;

namespace KrediHesaplama.Application
{
    public class KonutKrediService : IKonutKrediService
    {
        public double HesaplaFaizOrani(int vade)
        {
            return vade <= 60 ? 2.99 : 2.89;
        }

        public double HesaplaAylikOdeme(double tutar, double faizOrani, int vade)
        {
            double aylikFaiz = faizOrani / 100;
            double taksit = (tutar * aylikFaiz) / (1 - Math.Pow(1 + aylikFaiz, -vade));
            return Math.Round(taksit, 2);
        }

        public double HesaplaToplamOdeme(double aylikOdeme, int vade)
        {
            return Math.Round(aylikOdeme * vade, 2);
        }

        public List<OdemePlaniSatiri> OdemePlaniOlustur(double aylikOdeme, int vade)
        {
            var odemePlani = new List<OdemePlaniSatiri>();
            for (int ay = 1; ay <= vade; ay++)
            {
                odemePlani.Add(new OdemePlaniSatiri
                {
                    Ay = ay,
                    Tutar = aylikOdeme
                });
            }
            return odemePlani;
        }

        public bool GecerliMi(double tutar, int vade)
        {
            return tutar >= 100_000 && tutar <= 8_500_000 && vade >= 3 && vade <= 120;
        }
    }
}

using KrediHesaplama.Domain.Models;
using KrediHesaplama.Infrastructure.Interfaces;

namespace KrediHesaplama.Application
{
    public class TasitKrediService : ITasitKrediService
    {
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

        public int GetMaxVade(double tutar)
        {
            if (tutar <= 400_000) return 48;
            else if (tutar <= 800_000) return 36;
            else if (tutar <= 1_200_000) return 24;
            else return 12;
        }
    }
}

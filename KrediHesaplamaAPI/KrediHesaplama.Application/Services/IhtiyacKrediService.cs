using KrediHesaplama.Application.Interfaces;
using KrediHesaplama.Domain.Models;
using KrediHesaplama.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrediHesaplama.Application.Services
{
    public class IhtiyacKrediService : IIhtiyacKrediService
    {
        public double AylikOdemeHesap(double tutar, double faizOrani, int vade)
        {
            double aylikFaiz = faizOrani / 100;
            double taksit = (tutar * aylikFaiz) / (1 - Math.Pow(1 + aylikFaiz, -vade));
            return Math.Round(taksit, 2);
        }

        public bool GecerliMi(double tutar, int vade)
        {
            return tutar >= 3000 && tutar <= 250_000 && vade > 3 && vade < 36;
        }

        public List<OdemePlaniSatiri> OdemePlaniOlustur(double aylikOdeme, int vade)
        {
            var odemePlani = new List<OdemePlaniSatiri>();
            for (int ay =1; ay <= vade;ay++)
            {
                odemePlani.Add(new OdemePlaniSatiri
                {
                    Ay = ay,
                    Tutar = aylikOdeme

                });
            }
            return odemePlani;
        }

        public double ToplamOdemeHesap (double aylikOdeme, int vade)
        {
            return Math.Round(aylikOdeme * vade, 2);

        }
    }
}

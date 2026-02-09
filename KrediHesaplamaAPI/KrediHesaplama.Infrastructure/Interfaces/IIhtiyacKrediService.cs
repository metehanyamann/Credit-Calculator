using KrediHesaplama.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrediHesaplama.Infrastructure.Interfaces
{
    public interface IIhtiyacKrediService
    {
        double ToplamOdemeHesap(double aylikOdeme, int vade);
        public double AylikOdemeHesap(double tutar, double faizOrani, int vade);

        List<OdemePlaniSatiri> OdemePlaniOlustur(double aylikOdeme, int vade);

        bool GecerliMi(double tutar, int vade);
    }
}

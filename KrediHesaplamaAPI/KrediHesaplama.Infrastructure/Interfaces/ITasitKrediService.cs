using KrediHesaplama.Domain.Models;
using KrediHesaplamaAPI.Models;

namespace KrediHesaplama.Infrastructure.Interfaces
{
    public interface ITasitKrediService
    {
        double HesaplaAylikOdeme(double tutar, double faizOrani, int vade);
        double HesaplaToplamOdeme(double aylikOdeme, int vade);
        List<OdemePlaniSatiri> OdemePlaniOlustur(double aylikOdeme, int vade);
        int GetMaxVade(double tutar); // tutara göre 48/36/24/12
    }
}

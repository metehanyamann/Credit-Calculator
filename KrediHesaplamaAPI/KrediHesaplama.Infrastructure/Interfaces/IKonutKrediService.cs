using KrediHesaplama.Domain.Models;

namespace KrediHesaplama.Infrastructure.Interfaces
{
    public interface IKonutKrediService
    {
        double HesaplaFaizOrani(int vade);
        double HesaplaAylikOdeme(double tutar, double faizOrani, int vade);
        double HesaplaToplamOdeme(double aylikOdeme, int vade);
        List<OdemePlaniSatiri> OdemePlaniOlustur(double aylikOdeme, int vade);
        bool GecerliMi(double tutar, int vade);
    }
}

namespace KrediHesaplama.Domain.Models
{
    public class OdemePlaniSatiri
    {
        public int Ay { get; set; }
        public double Tutar { get; set; }
        public double Anapara { get; set; }
        public double Faiz { get; set; }
        public double KalanAnaPara { get; set; }
    }
}

namespace KrediHesaplamaAPI.Models
{
    public class KrediUrunu
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public double FaizOrani { get; set; }
        public double MinTutar { get; set; }
        public double MaxTutar { get; set; }
        public int MinVade { get; set; }
        public int MaxVade { get; set; }
    }
}

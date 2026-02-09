using Microsoft.EntityFrameworkCore;    
namespace KrediHesaplamaAPI.Models
{
    public class AppDbContext : DbContext 
    {
        public  AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<KrediUrunu> KrediUrunleri { get; set; }

        public DbSet<Hesaplama> Hesaplamalar { get; set; }

    }
}

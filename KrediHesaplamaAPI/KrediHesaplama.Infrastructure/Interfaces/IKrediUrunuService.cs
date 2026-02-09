using KrediHesaplamaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrediHesaplama.Application.Interfaces
{
    public interface IKrediUrunuService
    {
        Task<List<KrediUrunu>> GetAllAsync();
        Task<KrediUrunu> GetByIdAsync(int id);
        Task AddAsync(KrediUrunu urun);
        Task UpdateAsync(KrediUrunu urun);
        Task DeleteAsync(int id);
    }
}

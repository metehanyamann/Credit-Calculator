using KrediHesaplamaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrediHesaplama.Infrastructure.Interfaces
{
    public interface IHesaplamaService
    {
        Task<List<Hesaplama?>> GetAllAsync();
        Task<Hesaplama> GetByIdAsync(int id);
        Task<int> AddAsync(Hesaplama hesaplama); // id'yi döndürecek


    }
}

using Microsoft.AspNetCore.Mvc;
using KrediHesaplama.Infrastructure.Interfaces;
using KrediHesaplamaAPI.Models;
using System.Text.Json;

namespace KrediHesaplamaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HesaplamaController : ControllerBase
    {
        private readonly IHesaplamaService _hesaplamaService;

        public HesaplamaController(IHesaplamaService hesaplamaService)
        {
            _hesaplamaService = hesaplamaService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Hesaplama model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _hesaplamaService.AddAsync(model);
                // Hesaplama kaydedildi, şimdi sonucu ve ödeme planını dön
                var hesaplama = await _hesaplamaService.GetByIdAsync(id);
                if (hesaplama == null)
                    return NotFound();

                var odemePlani = hesaplama.OdemePlaniJson != null
                    ? JsonSerializer.Deserialize<object>(hesaplama.OdemePlaniJson)
                    : null;

                return Ok(new
                {
                    krediTipi = hesaplama.KrediTipi,
                    tutar = hesaplama.Tutar,
                    vade = hesaplama.Vade,
                    faiz = hesaplama.Faiz,
                    aylikOdeme = hesaplama.AylikOdeme,
                    toplamOdeme = hesaplama.ToplamOdeme,
                    odemePlani = odemePlani
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { hata = ex.Message });
            }
        }
    }
}

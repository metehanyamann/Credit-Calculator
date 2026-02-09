using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KrediHesaplamaAPI.Models;

namespace KrediHesaplamaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KrediUrunuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KrediUrunuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/KrediUrunu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KrediUrunu>>> GetKrediUrunleri()
        {
            return await _context.KrediUrunleri.ToListAsync();
        }

        // GET: api/KrediUrunu/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KrediUrunu>> GetKrediUrunu(int id)
        {
            var urun = await _context.KrediUrunleri.FindAsync(id);
            if (urun == null)
                return NotFound();
            return urun;
        }

        // POST: api/KrediUrunu
        [HttpPost]
        public async Task<ActionResult<KrediUrunu>> PostKrediUrunu(KrediUrunu urun)
        {
            _context.KrediUrunleri.Add(urun);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKrediUrunu), new { id = urun.Id }, urun);
        }

        // PUT: api/KrediUrunu/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKrediUrunu(int id, KrediUrunu urun)
        {
            if (id != urun.Id)
                return BadRequest();

            _context.Entry(urun).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.KrediUrunleri.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/KrediUrunu/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKrediUrunu(int id)
        {
            var urun = await _context.KrediUrunleri.FindAsync(id);
            if (urun == null)
                return NotFound();

            _context.KrediUrunleri.Remove(urun);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}

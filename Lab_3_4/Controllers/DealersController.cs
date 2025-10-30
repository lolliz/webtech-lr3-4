using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab_3_4.Data;
using Lab_3_4.Models;

namespace Lab_3_4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DealersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dealers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dealer>>> GetDealers()
        {
            return await _context.Dealers.ToListAsync();
        }

        // GET: api/dealers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dealer>> GetDealer(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();
            return dealer;
        }

        // POST: api/dealers
        [HttpPost]
        public async Task<ActionResult<Dealer>> PostDealer(Dealer dealer)
        {
            _context.Dealers.Add(dealer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDealer), new { id = dealer.ID }, dealer);
        }

        // PUT: api/dealers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDealer(int id, Dealer dealer)
        {
            if (id != dealer.ID) return BadRequest();
            _context.Entry(dealer).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) when (!DealerExists(id))
            { return NotFound(); }
            return NoContent();
        }

        // DELETE: api/dealers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDealer(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return NotFound();
            _context.Dealers.Remove(dealer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool DealerExists(int id) => _context.Dealers.Any(e => e.ID == id);
    }
}

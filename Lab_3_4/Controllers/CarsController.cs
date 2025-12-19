using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab_3_4.Data;
using Lab_3_4.Models;

namespace Lab_3_4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars.Include(c => c.Dealer).ToListAsync();
        }

        // GET: api/cars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Dealer)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            if (!await DealerExists(car.DealerID))
            {
                return BadRequest("Дилер не найден.");
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.ID }, car);
        }

        // PUT: api/cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.ID)
            {
                return BadRequest();
            }

            if (!await DealerExists(car.DealerID))
            {
                return BadRequest("Дилер не найден.");
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CarExists(int id)
        {
            return await _context.Cars.AnyAsync(e => e.ID == id);
        }

        private async Task<bool> DealerExists(int id)
        {
            return await _context.Dealers.AnyAsync(e => e.ID == id);
        }
    }
}
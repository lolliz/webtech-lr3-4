using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab_3_4.Data;
using Lab_3_4.Models;
using Lab_3_4.Events;
using Lab_3_4.Services;

namespace Lab_3_4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMqService _rabbitMqService;

        public CarsController(AppDbContext context, IRabbitMqService rabbitMqService)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
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
            if (car == null) return NotFound();
            return car;
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            if (!await DealerExists(car.DealerID)) return BadRequest("Дилер не найден.");

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            // Отправляем событие CREATE
            var carEvent = new CarEvent
            {
                EventType = "CREATE",
                Car = new CarData
                {
                    Firm = car.Firm,
                    Model = car.Model,
                    Year = car.Year,
                    Power = car.Power,
                    Color = car.Color,
                    Price = car.Price
                }
            };
            _rabbitMqService.Publish(carEvent);

            return CreatedAtAction(nameof(GetCar), new { id = car.ID }, car);
        }

        // PUT: api/cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.ID) return BadRequest();
            if (!await DealerExists(car.DealerID)) return BadRequest("Дилер не найден.");

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CarExists(id))
                    return NotFound();
                else
                    throw;
            }

            // Отправляем событие UPDATE
            var carEvent = new CarEvent
            {
                EventType = "UPDATE",
                Car = new CarData
                {
                    Firm = car.Firm,
                    Model = car.Model,
                    Year = car.Year,
                    Power = car.Power,
                    Color = car.Color,
                    Price = car.Price
                }
            };
            _rabbitMqService.Publish(carEvent);

            return NoContent();
        }

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            // Сохраняем данные перед удалением
            var carEvent = new CarEvent
            {
                EventType = "DELETE",
                Car = new CarData
                {
                    Firm = car.Firm,
                    Model = car.Model,
                    Year = car.Year,
                    Power = car.Power,
                    Color = car.Color,
                    Price = car.Price
                }
            };

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            // Отправляем событие DELETE
            _rabbitMqService.Publish(carEvent);

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
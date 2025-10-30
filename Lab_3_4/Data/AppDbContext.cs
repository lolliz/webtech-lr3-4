using Microsoft.EntityFrameworkCore;
using Lab_3_4.Models;

namespace Lab_3_4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Таблицы, с которыми работает API
        public DbSet<Car> Cars { get; set; }
        public DbSet<Dealer> Dealers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи: один дилер — многим автомобилям
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Dealer)           // У Car есть один Dealer
                .WithMany(d => d.Cars)           // У Dealer — список Car
                .HasForeignKey(c => c.DealerID)  // Через поле DealerID
                .OnDelete(DeleteBehavior.Cascade); // При удалении дилера — удаляются и его машины

            base.OnModelCreating(modelBuilder);
        }
    }
}
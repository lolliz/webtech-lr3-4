using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_3_4.Models
{
    public class Car
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Firm { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Power { get; set; }

        [Required]
        [MaxLength(255)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Price { get; set; }

        // Внешний ключ
        public int DealerID { get; set; }

        // Навигационное свойство (связь с дилером)
        public virtual Dealer? Dealer { get; set; }
    }
}
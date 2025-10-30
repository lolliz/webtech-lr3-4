using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_3_4.Models
{
    public class Dealer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Area { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal Rating { get; set; }

        // Навигационное свойство (список автомобилей у дилера)
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
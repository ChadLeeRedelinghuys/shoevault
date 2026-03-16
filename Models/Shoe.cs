using System.ComponentModel.DataAnnotations;

namespace ShoeVault.Models
{
    public class Shoe
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Weight { get; set; } // grams

        public int Drop { get; set; } // mm

        public int StackHeight { get; set; } // mm

        public string? Outsole { get; set; }

        public string? BestFor { get; set; }

        public ICollection<ShoeImage>? Images { get; set; } = new List<ShoeImage>();

    }
}

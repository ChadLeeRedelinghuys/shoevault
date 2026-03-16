using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeVault.Models
{
    public class ShoeImage
    {
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; } = string.Empty;

        // Foreign key
        public int ShoeId { get; set; }

        [ForeignKey("ShoeId")]
        public Shoe? Shoe { get; set; }
    }
}

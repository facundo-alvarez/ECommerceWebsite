using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationCore.Entities
{
    public class Product
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Ingredients { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Price { get; set; }

        public bool IsOnSale { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SalePrice { get; set; }

        public string? Image { get; set; }

        public Category? Category { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public bool InStock { get; set; }

        public string? Tags { get; set; }
    }
}

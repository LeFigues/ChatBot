using System.ComponentModel.DataAnnotations;

namespace ufl_erp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Stock>? Stocks { get; set; }
        public ICollection<Detail>? Details { get; set; }

        public bool IsDeleted { get; set; }
    }
}

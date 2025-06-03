using System.ComponentModel.DataAnnotations;

namespace ufl_erp.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}

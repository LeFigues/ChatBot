using System.ComponentModel.DataAnnotations;

namespace ufl_erp.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}

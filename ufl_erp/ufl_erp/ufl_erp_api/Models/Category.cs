using System.ComponentModel.DataAnnotations;

namespace ufl_erp_api.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Brand>? Brands { get; set; }
    }
}

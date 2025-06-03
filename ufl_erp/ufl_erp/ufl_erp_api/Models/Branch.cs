using System.ComponentModel.DataAnnotations;

namespace ufl_erp_api.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public string? Phone { get; set; }

        public ICollection<Stock>? Stocks { get; set; }
        public ICollection<Sale>? Sales { get; set; }
    }
}

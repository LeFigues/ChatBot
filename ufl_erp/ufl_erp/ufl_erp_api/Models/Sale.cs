using System.ComponentModel.DataAnnotations;

namespace ufl_erp_api.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public int Status { get; set; }
        public string? NIT { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
        public int PersonId { get; set; }
        public Person? Person { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public DateTime? LastUpdateAt { get; set; }
        public ICollection<Detail>? Details { get; set; }
        public bool IsDeleted { get; set; }
    }
}

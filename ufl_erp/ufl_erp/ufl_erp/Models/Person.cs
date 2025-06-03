using System.ComponentModel.DataAnnotations;

namespace ufl_erp.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CI { get; set; }
        public string Phone { get; set; }
        public DateOnly Birthdate { get; set; }
        
        public int? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Sale>? Sales { get; set; }
    }
}

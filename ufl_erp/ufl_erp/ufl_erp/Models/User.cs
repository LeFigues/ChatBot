using System.ComponentModel.DataAnnotations;

namespace ufl_erp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }


        public ICollection<Sale>? Sales { get; set; }
    }
}

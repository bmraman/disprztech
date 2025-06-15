using System.ComponentModel.DataAnnotations.Schema;

namespace disprztech.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; } 
        public Guid UserId { get; set; } = Guid.NewGuid(); 
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }   
    }
}

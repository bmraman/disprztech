using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace disprztech.Models
{
    [Table("Users")]   
    public class User
    {
        public int Id { get; set; }
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? CompanyName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserType { get; set; } = UserTypeEnum.Default.ToString();
        public string Location { get; set; }
        public string Designation { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        
    }

    public enum UserTypeEnum
    {
        [Display(Name = "Default")]
        Default,
        [Display(Name = "Corporate")]
        Corporate,
        [Display(Name = "Consultant")]
        Consultant,
        [Display(Name = "Freelancer")]
        Freelancer
    }
}

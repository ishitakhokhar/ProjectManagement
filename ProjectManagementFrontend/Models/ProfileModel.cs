using System.ComponentModel.DataAnnotations;

namespace ProjectManagementFrontend.Models
{
    public class ProfileModel
    {
       
            public int UserId { get; set; }

            [Required]
            public string FullName { get; set; } = "";

            [Required, EmailAddress]
            public string Email { get; set; } = "";

            [DataType(DataType.Password)]
            public string? NewPassword { get; set; } // optional
        
    }
}

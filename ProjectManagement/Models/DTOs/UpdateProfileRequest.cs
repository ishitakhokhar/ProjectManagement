namespace ProjectManagement.Models.DTOs
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? NewPassword { get; set; } 
    }
}

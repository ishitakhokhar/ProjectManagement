namespace ProjectManagement.Models.DTOs
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;  // plain password
        //public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }

}

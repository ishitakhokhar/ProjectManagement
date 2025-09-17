namespace ProjectManagementFrontend.Models
{
    public class RegisterUserModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // ✅ must match backend property
        public int RoleID { get; set; }
    }


}

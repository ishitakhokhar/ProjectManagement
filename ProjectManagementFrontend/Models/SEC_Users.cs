using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementFrontend.Models
{
    public class SEC_Users
    {
        public int UserId { get; set; }

        // ✅ Must match backend DTO property names exactly
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // plaintext from form
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserDropDownModel
    {
        [JsonPropertyName("userID")]
        public int UserID { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;
    }
}

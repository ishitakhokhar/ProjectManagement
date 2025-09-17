using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementFrontend.Models
{
    public class PRJ_IssuesModel
    {
        public int IssueId { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }

        [Required]
        public DateTime RaisedOn { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int StatusId { get; set; }
        public string? StatusName { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }

        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }

        public IFormFile? File1 { get; set; }
        public IFormFile? File2 { get; set; }

        // File paths for existing attachments (from backend)
        public string? Attachment1 { get; set; }
        public string? Attachment2 { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        // dropdowns (frontend only)
        public List<SelectListItem> ProjectList { get; set; } = new();
        public List<SelectListItem> StatusList { get; set; } = new();
        public List<SelectListItem> UserList { get; set; } = new();
    }

    public class DropdownModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

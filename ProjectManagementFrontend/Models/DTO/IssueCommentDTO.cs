using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectManagementFrontend.Models.DTO
{
    public class IssueCommentDTO
    {
        // Always needed

        public int CommentId { get; set; }
        public int IssueId { get; set; }
        public string CommentText { get; set; }
        public bool IsShow { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Only returned from GET
        public string IssueTitle { get; set; }
        public string CreatedByName { get; set; }

        public List<SelectListItem> IssueList { get; set; }
        public List<SelectListItem> UserList { get; set; }

        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();

    }
}

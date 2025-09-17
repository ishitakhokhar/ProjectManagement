using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProjectManagementFrontend.Models
{
    public class PRJ_IssueCommentsModel
    {
        public int CommentId { get; set; }
        public int IssueId { get; set; }
        public string CommentText { get; set; }
        public bool IsShow { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        //public string IssueTitle { get; set; }
        //public string CreatedByName { get; set; }

        public List<SelectListItem> IssueList { get; set; }
        public List<SelectListItem> UserList { get; set; }
    }

    public class ProjectIssueDropDown
    {
        [JsonProperty("issueId")]
        public int IssueId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}

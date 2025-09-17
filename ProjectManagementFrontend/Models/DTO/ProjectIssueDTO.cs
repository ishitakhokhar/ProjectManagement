using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProjectManagementFrontend.Models.DTO
{
    public class ProjectIssueDTO
    {
        [JsonProperty("issueId")]
        public int IssueId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("statusName")]
        public string StatusName { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("createdByName")]
        public string CreatedByName { get; set; }

        [JsonProperty("assignedToName")]
        public string AssignedToName { get; set; }

        [JsonProperty("raisedOn")]
        public DateTime RaisedOn { get; set; }

        [JsonProperty("dueDate")]
        public DateTime? DueDate { get; set; }

        // Add these for file paths
        [JsonProperty("attachment1")]
        public string? Attachment1 { get; set; }

        [JsonProperty("attachment2")]
        public string? Attachment2 { get; set; }

        // Dropdowns if needed (usually not in DTO)
        public List<SelectListItem>? ProjectList { get; set; }
        public List<SelectListItem>? StatusList { get; set; }
        public List<SelectListItem>? UserList { get; set; }
        public List<SelectListItem>? IssueList { get; set; }
    }

}

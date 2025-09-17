namespace ProjectManagement.Models.DTOs
{
    public class ProjectIssueDTO
    {
        public int IssueId { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string StatusName { get; set; }
        public string ProjectName { get; set; }
        public string CreatedByName { get; set; }
        public string AssignedToName { get; set; }
        public DateTime RaisedOn { get; set; }
        public DateTime? DueDate { get; set; }

        public string? Attachment1 { get; set; }
        public string? Attachment2 { get; set; }

    }
}

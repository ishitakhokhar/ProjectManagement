namespace ProjectManagement.Models.DTOs
{
    public class ProjectIssueUploadDTO
    {
        public int IssueId { get; set; } // optional for updates
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime RaisedOn { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }

        // File uploads
        public IFormFile? Attachment1 { get; set; }
        public IFormFile? Attachment2 { get; set; }
    }
}

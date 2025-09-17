namespace ProjectManagement.Models.DTOs
{
    public class IssueCommentDTO
    {
        public int CommentId { get; set; }
        public int IssueId { get; set; }
        public string IssueTitle { get; set; }
        public string CommentText { get; set; }
        public bool IsShow { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

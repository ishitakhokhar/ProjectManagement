using Microsoft.AspNetCore.Mvc.Rendering;

    public class PRJ_ProjectMemberModel
    {
        public int ProjectMemberId { get; set; }

        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }

        public int UserId { get; set; }
        public string? FullName { get; set; }

    public string? RoleInProject { get; set; }
 

    public List<SelectListItem> ProjectList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();


}








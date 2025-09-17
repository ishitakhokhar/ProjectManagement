using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models;

public partial class SecUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public virtual ICollection<PrjIssue>? PrjIssueAssignedToNavigations { get; set; } = new List<PrjIssue>();
    [JsonIgnore]
    public virtual ICollection<PrjIssueComment>? PrjIssueComments { get; set; } = new List<PrjIssueComment>();
    [JsonIgnore]
    public virtual ICollection<PrjIssue>? PrjIssueCreatedByNavigations { get; set; } = new List<PrjIssue>();
    [JsonIgnore]
    public virtual ICollection<PrjProjectMember>? PrjProjectMembers { get; set; } = new List<PrjProjectMember>();
    [JsonIgnore]
    public virtual ICollection<PrjProject>? PrjProjects { get; set; } = new List<PrjProject>();
    [JsonIgnore]
    public virtual SecRole? Role { get; set; } = null!;
}

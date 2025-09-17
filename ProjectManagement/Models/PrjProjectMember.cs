using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace ProjectManagement.Models;

public partial class PrjProjectMember
{
    public int ProjectMemberId { get; set; }
    public int ProjectId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public string? ProjectName { get; set; } = null!;
    public int UserId { get; set; }
    [NotMapped]
    [JsonIgnore]
    public string? UserName { get; set; } = null!;
    public string RoleInProject { get; set; } = null!;
    [JsonIgnore]
    public virtual PrjProject? Project { get; set; } = null!;
    [JsonIgnore]
    public virtual SecUser? User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace ProjectManagement.Models;

public partial class PrjIssue
{
    public int IssueId { get; set; }
    public int ProjectId { get; set; }
    public DateTime RaisedOn { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public string Priority { get; set; } = null!;
    public int CreatedBy { get; set; }
    public int? AssignedTo { get; set; }

    public string? Attachment1 { get; set; }
    public string? Attachment2 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }

    // ---- NotMapped fields for upload ----
    [NotMapped]
    public IFormFile? File1 { get; set; }

    [NotMapped]
    public IFormFile? File2 { get; set; }

    [JsonIgnore]
    public virtual SecUser? AssignedToNavigation { get; set; }
    [JsonIgnore]

    public virtual SecUser? CreatedByNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<PrjIssueComment>? PrjIssueComments { get; set; } = new List<PrjIssueComment>();
    [JsonIgnore]
    public virtual PrjProject? Project { get; set; } = null!;
    [JsonIgnore]
    public virtual MstStatus? Status { get; set; } = null!;
}

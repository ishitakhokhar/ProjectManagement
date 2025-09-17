using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models;

public partial class MstStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<PrjIssue>? PrjIssues { get; set; } = new List<PrjIssue>();
}

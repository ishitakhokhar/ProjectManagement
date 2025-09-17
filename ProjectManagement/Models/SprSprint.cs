using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models;

public partial class SprSprint
{
    public int SprintId { get; set; }

    public int ProjectId { get; set; }

    public string SprintName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsCompleted { get; set; }

    public int? TotalTasks { get; set; }



    [JsonIgnore]
    public virtual PrjProject? Project { get; set; } = null!;
}

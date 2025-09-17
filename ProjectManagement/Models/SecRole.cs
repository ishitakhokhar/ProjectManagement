using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectManagement.Models;

public partial class SecRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<SecUser>? SecUsers { get; set; } = new List<SecUser>();
}

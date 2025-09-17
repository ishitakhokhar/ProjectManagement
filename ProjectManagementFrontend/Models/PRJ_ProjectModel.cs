using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProjectManagementFrontend.Models
{
    public class PRJ_ProjectModel
    {
        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("projectName")]
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string ProjectName { get; set; } = null!;

        [JsonProperty("projectDescription")]
        public string? ProjectDescription { get; set; }

        [JsonProperty("clientName")]
        public string? ClientName { get; set; }

        [JsonProperty("projectManagerName")]
        public string? ProjectManagerName { get; set; }

        [JsonProperty("startDate")]
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("projectStatus")]
        public string? ProjectStatus { get; set; }

        [JsonProperty("visibility")]
        public string? Visibility { get; set; }

        [JsonProperty("createdBy")]
        public int CreatedBy { get; set; }

        [JsonProperty("createdByName")]
        public string? CreatedByName { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

      

        public IEnumerable<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ProjectList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();
    }
}

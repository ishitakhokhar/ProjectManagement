using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectManagementFrontend.Models
{
    public class SPR_Sprint
    {
        public int SprintId { get; set; }

        [Required(ErrorMessage = "Please select a project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Sprint Name is required")]
        public string SprintName { get; set; } = null!;

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Total Tasks cannot be negative")]
        public int? TotalTasks { get; set; }

        public List<SelectListItem> ProjectList { get; set; } = new List<SelectListItem>();
    }
}

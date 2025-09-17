using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public DashboardController(ProjectManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard counts based on role.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="role">Role: admin, pm, user</param>
        /// <returns>Counts of projects, sprints, issues, comments</returns>
        [HttpGet("counts")]
        public async Task<ActionResult<DashboardCountsDTO>> GetCounts(
         [FromHeader] int userId,
         [FromHeader] string role)
        {
            if (userId <= 0 || string.IsNullOrEmpty(role))
                return BadRequest("UserId and Role headers are required.");

            var dto = new DashboardCountsDTO();

            switch (role.ToLower())
            {
                case "admin":
                    dto.TotalProjects = await _context.PrjProjects.CountAsync();
                    dto.TotalSprints = await _context.SprSprints.CountAsync();
                    dto.TotalIssues = await _context.PrjIssues.CountAsync();
                    dto.TotalComments = await _context.PrjIssueComments.CountAsync();
                    dto.TotalUsers = await _context.SecUsers.CountAsync();
                    dto.TotalRoles = await _context.SecRoles.CountAsync();
                    break;

                case "projectmanager":
                case "pm":
                    // Get Project IDs where PM is a member
                    var pmProjectIds = await _context.PrjProjectMembers
                        .Where(pm => pm.UserId == userId)
                        .Select(pm => pm.ProjectId)
                        .ToListAsync();

                    // Also include projects they created
                    var createdProjectIds = await _context.PrjProjects
                        .Where(p => p.CreatedBy == userId)
                        .Select(p => p.ProjectId)
                        .ToListAsync();

                    // Merge both lists
                    var allPmProjectIds = pmProjectIds
                        .Union(createdProjectIds)
                        .Distinct()
                        .ToList();

                    // Total projects
                    dto.TotalProjects = allPmProjectIds.Count;

                    // Total sprints in those projects
                    dto.TotalSprints = await _context.SprSprints
                        .CountAsync(s => allPmProjectIds.Contains(s.ProjectId));

                    // Total issues in those projects
                    dto.TotalIssues = await _context.PrjIssues
                        .CountAsync(i => allPmProjectIds.Contains(i.ProjectId));

                    // Total comments on those issues
                    dto.TotalComments = await _context.PrjIssueComments
                        .CountAsync(c => _context.PrjIssues
                            .Where(i => allPmProjectIds.Contains(i.ProjectId))
                            .Select(i => i.IssueId)
                            .Contains(c.IssueId));

                    break;

                case "user":
                    // Projects where the user is a member
                    var userProjectIds = await _context.PrjProjectMembers
                        .Where(pm => pm.UserId == userId)
                        .Select(pm => pm.ProjectId)
                        .Distinct()
                        .ToListAsync();

                    // Total projects
                    dto.TotalProjects = userProjectIds.Count;

                    // Total sprints in those projects
                    dto.TotalSprints = await _context.SprSprints
                        .CountAsync(s => userProjectIds.Contains(s.ProjectId));

                    // Total issues either assigned to OR created by this user
                    dto.TotalIssues = await _context.PrjIssues
                        .CountAsync(i => i.AssignedTo == userId || i.CreatedBy == userId);

                    // Total comments created by this user
                    dto.TotalComments = await _context.PrjIssueComments
                        .CountAsync(c => c.CreatedBy == userId);
                    break;


                default:
                    return Forbid("Invalid role");
            }

            return Ok(dto);
        }


    }
}

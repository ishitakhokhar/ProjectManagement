using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        #region Configuration Fields
        private readonly ProjectManagementContext context;
        public SprintController(ProjectManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllSprints
        [HttpGet]
        public async Task<ActionResult<List<object>>> GetSprints()
        {
            var sprints = await context.SprSprints
                .Join(
                    context.PrjProjects,
                    s => s.ProjectId,
                    p => p.ProjectId,
                    (s, p) => new
                    {
                        s.SprintId,
                        s.ProjectId,
                        ProjectName = p.ProjectName,
                        s.SprintName,
                        s.StartDate,
                        s.EndDate,
                        s.IsCompleted,
                        s.TotalTasks,
                      
                    }
                )
                .ToListAsync();

            return Ok(sprints);
        }
        #endregion

        #region GetAllSprintsById
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSprintsById(int id)
        {
            var sprint = await context.SprSprints
                .Where(s => s.SprintId == id)
                .Join(context.PrjProjects,
                      s => s.ProjectId,
                      p => p.ProjectId,
                      (s, p) => new
                      {
                          s.SprintId,
                          s.ProjectId,
                          ProjectName = p.ProjectName,
                          s.SprintName,
                          s.StartDate,
                          s.EndDate,
                          s.IsCompleted,
                          s.TotalTasks,
                        
                      })
                .FirstOrDefaultAsync();

            if (sprint == null) return NotFound();
            return Ok(sprint);
        }
        #endregion

        #region CreateSprints
        [HttpPost]
        public async Task<ActionResult<object>> CreateSprints(SprSprint sprSprint)
        {
            if (string.IsNullOrEmpty(sprSprint.SprintName))
                return BadRequest("SprintName is required.");

            await context.SprSprints.AddAsync(sprSprint);
            await context.SaveChangesAsync();

            var result = await context.SprSprints
                .Where(s => s.SprintId == sprSprint.SprintId)
                .Join(context.PrjProjects,
                      s => s.ProjectId,
                      p => p.ProjectId,
                      (s, p) => new
                      {
                          s.SprintId,
                          s.ProjectId,
                          ProjectName = p.ProjectName,
                          s.SprintName,
                          s.StartDate,
                          s.EndDate,
                          s.IsCompleted,
                          s.TotalTasks,
                      
                      })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
        #endregion

        #region UpdateSprints
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateSprints(int id, SprSprint sprSprint)
        {
            var existing = await context.SprSprints.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Update fields
            existing.SprintName = sprSprint.SprintName;
            existing.ProjectId = sprSprint.ProjectId;
            existing.StartDate = sprSprint.StartDate;
            existing.EndDate = sprSprint.EndDate;
            existing.IsCompleted = sprSprint.IsCompleted;
            existing.TotalTasks = sprSprint.TotalTasks;

            await context.SaveChangesAsync();

            var result = await context.SprSprints
                .Where(s => s.SprintId == existing.SprintId)
                .Join(context.PrjProjects,
                      s => s.ProjectId,
                      p => p.ProjectId,
                      (s, p) => new
                      {
                          s.SprintId,
                          s.ProjectId,
                          ProjectName = p.ProjectName,
                          s.SprintName,
                          s.StartDate,
                          s.EndDate,
                          s.IsCompleted,
                          s.TotalTasks,
                       
                      })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
        #endregion

        #region DeleteSprints
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteSprints(int id)
        {
            var sprint = await context.SprSprints.FindAsync(id);
            if (sprint == null)
                return NotFound();

            context.SprSprints.Remove(sprint);
            await context.SaveChangesAsync();

            var result = new
            {
                sprint.SprintId,
                sprint.ProjectId,
                ProjectName = context.PrjProjects
                    .Where(p => p.ProjectId == sprint.ProjectId)
                    .Select(p => p.ProjectName)
                    .FirstOrDefault(),
                sprint.SprintName,
                sprint.StartDate,
                sprint.EndDate,
                sprint.IsCompleted,
                sprint.TotalTasks,
               
            };

            return Ok(result);
        }
        #endregion

        #region GetByUser
        [HttpGet("My/{userId}")]
        public async Task<IActionResult> GetSprintsByUser(int userId)
        {
            var sprints = await context.SprSprints
                .Where(s => s.Project.PrjProjectMembers.Any(pm => pm.UserId == userId))
                .Select(s => new
                {
                    s.SprintId,
                    s.ProjectId,
                    ProjectName = s.Project.ProjectName,
                    s.SprintName,
                    s.StartDate,
                    s.EndDate,
                    s.IsCompleted,
                    s.TotalTasks,
              
                })
                .ToListAsync();

            return Ok(sprints);
        }
        #endregion
    }
}

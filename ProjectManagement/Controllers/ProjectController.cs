using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Models.DTOs;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        #region Configuration Fields
        private readonly ProjectManagementContext context;
        public ProjectController(ProjectManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllProject
        [HttpGet]
        public async Task<ActionResult> GetProjects(
       [FromQuery] string search = "",
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
        {
            var query = context.PrjProjects.AsQueryable();

            // Filtering by search term (project name)
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.ProjectName.Contains(search));

            // Total count
            var totalCount = await query.CountAsync();

            // Pagination
            var projects = await query
                .OrderBy(p => p.ProjectId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return wrapped response
            var response = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Data = projects
            };

            return Ok(response);
        }

        #endregion


        #region GetProjectsById
        [HttpGet("{id}")]
        public async Task<ActionResult<PrjProject>> GetProjectsById(int id)
        {
            var project = await context.PrjProjects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        #endregion

        #region CreateProject
        [HttpPost]
        public async Task<ActionResult<PrjProject>> CreateProjects([FromBody] PrjProject prjProject)
        {
            try
            {
                if (prjProject == null)
                {
                    return BadRequest("Project data is required");
                }

                // Validate FK: CreatedBy must exist
                var creatorExists = await context.SecUsers.AnyAsync(u => u.UserId == prjProject.CreatedBy);
                if (!creatorExists)
                {
                    return BadRequest($"Invalid CreatedBy id: {prjProject.CreatedBy}. User does not exist.");
                }

                // Validate SQL datetime range (SQL 'datetime' min is 1753-01-01)
                var sqlMin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                if (prjProject.StartDate < sqlMin)
                {
                    return BadRequest($"StartDate must be on/after {sqlMin:yyyy-MM-dd}.");
                }
                if (prjProject.EndDate.HasValue && prjProject.EndDate.Value < sqlMin)
                {
                    return BadRequest($"EndDate must be on/after {sqlMin:yyyy-MM-dd}.");
                }

                // Set creation timestamp
                prjProject.CreatedAt = DateTime.UtcNow;
                prjProject.UpdatedAt = DateTime.UtcNow;

                await context.PrjProjects.AddAsync(prjProject);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProjectsById), new { id = prjProject.ProjectId }, prjProject);
            }
            catch (Exception ex)
            {
                var detailed = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Internal server error: {detailed}");
            }
        }
        #endregion

        #region UpdateProject
        [HttpPut("{id}")]
        public async Task<ActionResult<PrjProject>> UpdateProjects(int id, PrjProject prjProject)
        {
            try
            {
                if (id != prjProject.ProjectId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingProject = await context.PrjProjects.FindAsync(id);
                if (existingProject == null)
                {
                    return NotFound("Project not found");
                }

                // Validate SQL datetime range
                var sqlMin = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                if (prjProject.StartDate < sqlMin)
                {
                    return BadRequest($"StartDate must be on/after {sqlMin:yyyy-MM-dd}.");
                }
                if (prjProject.EndDate.HasValue && prjProject.EndDate.Value < sqlMin)
                {
                    return BadRequest($"EndDate must be on/after {sqlMin:yyyy-MM-dd}.");
                }

                // Update properties
                existingProject.ProjectName = prjProject.ProjectName;
                existingProject.ProjectDescription = prjProject.ProjectDescription;
                existingProject.ClientName = prjProject.ClientName;
                existingProject.ProjectManagerName = prjProject.ProjectManagerName;
                existingProject.StartDate = prjProject.StartDate;
                existingProject.EndDate = prjProject.EndDate;
                existingProject.ProjectStatus = prjProject.ProjectStatus;
                existingProject.Visibility = prjProject.Visibility;
                existingProject.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return Ok(existingProject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion


        #region DeleteProject
        [HttpDelete("{id}")]
        public async Task<ActionResult<PrjProject>> DeleteProjects(int id)
        {
            var project = await context.PrjProjects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            context.PrjProjects.Remove(project);
            await context.SaveChangesAsync();
            return Ok(project);

        }
        #endregion

        #region GetProjectDropdown
        [HttpGet("Dropdown")]
        public async Task<ActionResult<List<ProjectDropDownDTO>>> GetProjectDropdown()
        {
            var projects = await context.PrjProjects
                .Select(p => new ProjectDropDownDTO
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName
                })
                .ToListAsync();

            return Ok(projects);
        }
        #endregion

    }
}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Models.DTOs;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectIssueController : ControllerBase
    {
        private readonly ProjectManagementContext _context;

        public ProjectIssueController(ProjectManagementContext context)
        {
            _context = context;
        }

        #region Get All Issues
        [HttpGet]
        public async Task<IActionResult> GetAllIssues()
        {
            var issues = await _context.PrjIssues
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.CreatedByNavigation)
                .Include(i => i.AssignedToNavigation)
                .Select(i => new ProjectIssueDTO
                {
                    IssueId = i.IssueId,
                    Title = i.Title,
                    Priority = i.Priority,
                    ProjectName = i.Project.ProjectName,
                    StatusName = i.Status.StatusName,
                    CreatedByName = i.CreatedByNavigation.FullName,
                    AssignedToName = i.AssignedToNavigation != null ? i.AssignedToNavigation.FullName : "Unassigned",
                    RaisedOn = i.RaisedOn,
                    DueDate = i.DueDate,
                    Attachment1 = i.Attachment1,
                    Attachment2 = i.Attachment2
                })
                .ToListAsync();

            return Ok(issues);
        }
        #endregion

        #region Get Issue By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectIssueDTO>> GetIssueById(int id)
        {
            var issue = await _context.PrjIssues
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.CreatedByNavigation)
                .Include(i => i.AssignedToNavigation)
                .Where(i => i.IssueId == id)
                .Select(i => new ProjectIssueDTO
                {
                    IssueId = i.IssueId,
                    Title = i.Title,
                    Priority = i.Priority,
                    ProjectName = i.Project.ProjectName,
                    StatusName = i.Status.StatusName,
                    CreatedByName = i.CreatedByNavigation.FullName,
                    AssignedToName = i.AssignedToNavigation != null ? i.AssignedToNavigation.FullName : "Unassigned",
                    RaisedOn = i.RaisedOn,
                    DueDate = i.DueDate,
                    Attachment1 = i.Attachment1,
                    Attachment2 = i.Attachment2

                })
                .FirstOrDefaultAsync();

            if (issue == null) return NotFound();
            return Ok(issue);
        }
        #endregion

        #region Create Issue
        [HttpPost]
        public async Task<ActionResult<ProjectIssueDTO>> CreateIssue([FromForm] ProjectIssueUploadDTO issueDto)
        {
            var prjIssue = new PrjIssue
            {
                ProjectId = issueDto.ProjectId,
                Title = issueDto.Title,
                Priority = issueDto.Priority,
                StatusId = issueDto.StatusId,
                CreatedBy = issueDto.CreatedBy,
                AssignedTo = issueDto.AssignedTo,
                RaisedOn = issueDto.RaisedOn,
                DueDate = issueDto.DueDate,
                Description = issueDto.Description
            };

            // Save attachments
            await SaveAttachmentsAsync(prjIssue, issueDto);

            await _context.PrjIssues.AddAsync(prjIssue);
            await _context.SaveChangesAsync();

            // Return DTO
            var createdIssueDto = new ProjectIssueDTO
            {
                IssueId = prjIssue.IssueId,
                Title = prjIssue.Title,
                Priority = prjIssue.Priority,
                ProjectName = (await _context.PrjProjects.FindAsync(prjIssue.ProjectId))?.ProjectName,
                StatusName = (await _context.MstStatuses.FindAsync(prjIssue.StatusId))?.StatusName,
                CreatedByName = (await _context.SecUsers.FindAsync(prjIssue.CreatedBy))?.FullName,
                AssignedToName = prjIssue.AssignedTo.HasValue ? (await _context.SecUsers.FindAsync(prjIssue.AssignedTo.Value))?.FullName : "Unassigned",
                RaisedOn = prjIssue.RaisedOn,
                DueDate = prjIssue.DueDate,
                Attachment1 = prjIssue.Attachment1,
                Attachment2 = prjIssue.Attachment2
            };

            return Ok(createdIssueDto);
        }

        #endregion

        #region Update Issue
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectIssueDTO>> UpdateIssue(int id, [FromForm] ProjectIssueUploadDTO issueDto)
        {
            var prjIssue = await _context.PrjIssues.FindAsync(id);
            if (prjIssue == null) return NotFound();

            prjIssue.ProjectId = issueDto.ProjectId;
            prjIssue.Title = issueDto.Title;
            prjIssue.Priority = issueDto.Priority;
            prjIssue.StatusId = issueDto.StatusId;
            prjIssue.AssignedTo = issueDto.AssignedTo;
            prjIssue.RaisedOn = issueDto.RaisedOn;
            prjIssue.DueDate = issueDto.DueDate;
            prjIssue.Description = issueDto.Description;

            // Only update attachments if new files are provided
            await SaveAttachmentsAsync(prjIssue, issueDto);

            _context.Entry(prjIssue).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return updated DTO
            var updatedIssueDto = new ProjectIssueDTO
            {
                IssueId = prjIssue.IssueId,
                Title = prjIssue.Title,
                Priority = prjIssue.Priority,
                ProjectName = (await _context.PrjProjects.FindAsync(prjIssue.ProjectId))?.ProjectName,
                StatusName = (await _context.MstStatuses.FindAsync(prjIssue.StatusId))?.StatusName,
                CreatedByName = (await _context.SecUsers.FindAsync(prjIssue.CreatedBy))?.FullName,
                AssignedToName = prjIssue.AssignedTo.HasValue ? (await _context.SecUsers.FindAsync(prjIssue.AssignedTo.Value))?.FullName : "Unassigned",
                RaisedOn = prjIssue.RaisedOn,
                DueDate = prjIssue.DueDate,
                Attachment1 = prjIssue.Attachment1,
                Attachment2 = prjIssue.Attachment2
            };

            return Ok(updatedIssueDto);
        }

        #endregion

        #region Delete Issue
        [HttpDelete("{id}")]
        public async Task<ActionResult<PrjIssue>> DeleteIssue(int id)
        {
            var issue = await _context.PrjIssues.FindAsync(id);
            if (issue == null) return NotFound();

            _context.PrjIssues.Remove(issue);
            await _context.SaveChangesAsync();
            return Ok(issue);
        }
        #endregion


   

        #region Dropdowns
        [HttpGet("ProjectsDropdown")]
        public async Task<IActionResult> GetProjectsDropdown()
        {
            var projects = await _context.PrjProjects
                .Select(p => new { Id = p.ProjectId, Name = p.ProjectName })
                .ToListAsync();
            return Ok(projects);
        }

      

        [HttpGet("UsersDropdown")]
        public async Task<IActionResult> GetUsersDropdown()
        {
            var users = await _context.SecUsers
                .Select(u => new { Id = u.UserId, Name = u.FullName })
                .ToListAsync();
            return Ok(users);
        }
        #endregion
        #region GetIssueDropdown
        [HttpGet("DropDown")]
        public async Task<ActionResult<IEnumerable<ProjectIssueDropDown>>> GetIssueDropDown()
        {
            var issues = await _context.PrjIssues
                .Select(i => new ProjectIssueDropDown
                {
                    IssueId = i.IssueId,
                    Title = i.Title
                })
                .ToListAsync();

            return Ok(issues);
        }
        #endregion
        #region Get My Issues
        [HttpGet("My/{userId}")]
        public async Task<IActionResult> GetMyIssues(int userId)
        {
            var issues = await _context.PrjIssues
                .Include(i => i.Project)
                .Include(i => i.Status)
                .Include(i => i.CreatedByNavigation)
                .Include(i => i.AssignedToNavigation)
                .Where(i => i.AssignedTo == userId)
                .Select(i => new ProjectIssueDTO
                {
                    IssueId = i.IssueId,
                    Title = i.Title,
                    Priority = i.Priority,
                    ProjectName = i.Project.ProjectName,
                    StatusName = i.Status.StatusName,
                    CreatedByName = i.CreatedByNavigation.FullName,
                    AssignedToName = i.AssignedToNavigation != null ? i.AssignedToNavigation.FullName : "Unassigned",
                    RaisedOn = i.RaisedOn,
                    DueDate = i.DueDate,
                    Attachment1 = i.Attachment1,
                    Attachment2 = i.Attachment2
                })
                .ToListAsync();

            return Ok(issues);
        }
        #endregion

        private async Task SaveAttachmentsAsync(PrjIssue issue, ProjectIssueUploadDTO dto)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Attachment1
            if (dto.Attachment1 != null && dto.Attachment1.Length > 0)
            {
                string fileName1 = $"{Guid.NewGuid()}_{dto.Attachment1.FileName}";
                string filePath1 = Path.Combine(uploadPath, fileName1);

                using var stream = new FileStream(filePath1, FileMode.Create);
                await dto.Attachment1.CopyToAsync(stream);

                // Store relative path only
                issue.Attachment1 = $"/uploads/{fileName1}";
            }

            // Attachment2
            if (dto.Attachment2 != null && dto.Attachment2.Length > 0)
            {
                string fileName2 = $"{Guid.NewGuid()}_{dto.Attachment2.FileName}";
                string filePath2 = Path.Combine(uploadPath, fileName2);

                using var stream = new FileStream(filePath2, FileMode.Create);
                await dto.Attachment2.CopyToAsync(stream);

                // Store relative path only
                issue.Attachment2 = $"/uploads/{fileName2}";
            }
        }





    }
}

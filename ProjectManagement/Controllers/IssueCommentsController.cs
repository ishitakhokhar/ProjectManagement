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
    public class IssueCommentsController : ControllerBase
    {
        #region Configuration Fields
        private readonly ProjectManagementContext context;
        public IssueCommentsController(ProjectManagementContext context)
        {
            this.context = context;
        }
        #endregion
        [HttpGet]
        public async Task<ActionResult<List<IssueCommentDTO>>> GetIssueComments()
        {
            var icomments = await context.PrjIssueComments
                .Include(c => c.Issue)
                .Include(c => c.CreatedByNavigation)
                .ToListAsync();

            var result = icomments.Select(c => new IssueCommentDTO
            {
                CommentId = c.CommentId,
                IssueId = c.IssueId,
                CommentText = c.CommentText,
                IsShow = c.IsShow,
                CreatedAt = c.CreatedAt,
                IssueTitle = c.Issue?.Title,
                CreatedBy = c.CreatedBy,
                CreatedByName = c.CreatedByNavigation?.FullName
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IssueCommentDTO>> GetIssueCommentById(int id)
        {
            var icomment = await context.PrjIssueComments
                .Include(c => c.Issue)
                .Include(c => c.CreatedByNavigation)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (icomment == null) return NotFound();

            return Ok(new IssueCommentDTO
            {
                CommentId = icomment.CommentId,
                IssueId = icomment.IssueId,
                CommentText = icomment.CommentText,
                IsShow = icomment.IsShow,
                CreatedAt = icomment.CreatedAt,
                IssueTitle = icomment.Issue?.Title,
                CreatedBy = icomment.CreatedBy,
                CreatedByName = icomment.CreatedByNavigation?.FullName
            });
        }

        [HttpPost]
        public async Task<ActionResult<IssueCommentDTO>> CreateIssueComment(IssueCommentDTO dto)
        {
            var entity = new PrjIssueComment
            {
                IssueId = dto.IssueId,
                CommentText = dto.CommentText,
                IsShow = dto.IsShow,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await context.PrjIssueComments.AddAsync(entity);
            await context.SaveChangesAsync();

            dto.CommentId = entity.CommentId;
            dto.CreatedAt = entity.CreatedAt;

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IssueCommentDTO>> UpdateIssueComment(int id, IssueCommentDTO dto)
        {
            if (id != dto.CommentId) return BadRequest();

            var entity = await context.PrjIssueComments.FindAsync(id);
            if (entity == null) return NotFound();

            entity.CommentText = dto.CommentText;
            entity.IsShow = dto.IsShow;

            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();

            dto.CreatedAt = entity.CreatedAt;

            return Ok(dto);
        }


        #region DeleteIssueComments
        [HttpDelete("{id}")]
        public async Task<ActionResult<PrjIssueComment>> DeleteIssueComments(int id)
        {
            var icomment = await context.PrjIssueComments.FindAsync(id);
            if (icomment == null)
            {
                return NotFound();
            }

            context.PrjIssueComments.Remove(icomment);
            await context.SaveChangesAsync();
            return Ok(icomment);

        }
        #endregion

        [HttpGet("My/{userId}")]
        public async Task<IActionResult> GetMyComments(int userId)
        {
            var comments = await context.PrjIssueComments
                .Where(c => c.CreatedBy == userId)
                .ToListAsync();

            return Ok(comments);
        }


        [HttpGet("AssignedToMe/{userId}")]
        public async Task<IActionResult> GetCommentsAssignedToMe(int userId)
        {
            var comments = await context.PrjIssueComments
                .Include(c => c.Issue)
                .Include(c => c.CreatedByNavigation)
                .Where(c => c.Issue.AssignedTo == userId) // 🔹 only issues assigned to this user
                .Select(c => new IssueCommentDTO
                {
                    CommentId = c.CommentId,
                    IssueId = c.IssueId,
                    CommentText = c.CommentText,
                    IsShow = c.IsShow,
                    CreatedAt = c.CreatedAt,
                    IssueTitle = c.Issue.Title,
                    CreatedBy = c.CreatedBy,
                    CreatedByName = c.CreatedByNavigation.FullName
                })
                .ToListAsync();

            return Ok(comments);
        }

    }
}

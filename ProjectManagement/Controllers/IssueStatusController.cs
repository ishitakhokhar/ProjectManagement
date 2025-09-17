using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueStatusController : ControllerBase
    {
        #region Configuration Fields
        private readonly ProjectManagementContext context;
        public IssueStatusController(ProjectManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllIssueStatus
        [HttpGet]
        public async Task<ActionResult<List<MstStatus>>> GetIssueStatus()
        {
            var istatus = await context.MstStatuses.ToListAsync();
            return Ok(istatus);
        }
        #endregion


        #region GetIssueStatusById
        [HttpGet("{id}")]
        public async Task<ActionResult<MstStatus>> GetIssueStatusById(int id)
        {
            var istatus = await context.MstStatuses.FindAsync(id);
            if (istatus == null)
            {
                return NotFound();
            }
            return Ok(istatus);
        }
        #endregion

        #region CreateIssueStaus
        [HttpPost]
        public async Task<ActionResult<MstStatus>> CreateStatus(MstStatus mstStatus)
        {
            await context.MstStatuses.AddAsync(mstStatus);
            await context.SaveChangesAsync();
            return Ok(mstStatus);

        }
        #endregion

        #region UpdateIssueStatus
        [HttpPut("{id}")]
        public async Task<ActionResult<PrjProject>> UpdateIssueStatus(int id, MstStatus mstStatus)
        {
            if (id != mstStatus.StatusId)
            {
                return BadRequest();
            }
            context.Entry(mstStatus).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(mstStatus);

        }
        #endregion


        #region DeleteIssueStatus
        [HttpDelete("{id}")]
        public async Task<ActionResult<MstStatus>> DeleteIssueStatus(int id)
        {
            var istatus = await context.MstStatuses.FindAsync(id);
            if (istatus == null)
            {
                return NotFound();
            }

            context.MstStatuses.Remove(istatus);
            await context.SaveChangesAsync();
            return Ok(istatus);

        }
        #endregion

        [HttpGet("StatusDropdown")]
        public async Task<IActionResult> GetStatusDropdown()
        {
            var statuses = await context.MstStatuses
                .Select(s => new { s.StatusId, s.StatusName })
                .ToListAsync();
            return Ok(statuses);
        }
    }
}

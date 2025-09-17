using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Configuration Fields
        private readonly ProjectManagementContext context;
        public RoleController(ProjectManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllRoles
        [HttpGet]
        public async Task<ActionResult<List<SecRole>>> GetRoles()
        {
            var role = await context.SecRoles.ToListAsync();
            return Ok(role);
        }
        #endregion


        #region GetAllRolesById
        [HttpGet("{id}")]
        public async Task<ActionResult<SecRole>> GetRolesById(int id)
        {
            var role = await context.SecRoles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }
        #endregion

        #region CreateRole
        [HttpPost]
        public async Task<ActionResult<SecRole>> CreateRoles(SecRole secrole)
        {
            await context.SecRoles.AddAsync(secrole);
            await context.SaveChangesAsync();
            return Ok(secrole);

        }
        #endregion

        #region UpdateRole
        [HttpPut("{id}")]
        public async Task<ActionResult<SecRole>> UpdateRoles(int id, SecRole secrole)
        {
            if (id != secrole.RoleId)
            {
                return BadRequest();
            }
            context.Entry(secrole).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(secrole);

        }
        #endregion


        #region DeleteRole
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoles(int id)
        {
            var secrole = await context.SecRoles.FindAsync(id);

            if (secrole == null)
            {
                return NotFound(new { message = "Role not found." });
            }

            try
            {
                context.SecRoles.Remove(secrole);
                await context.SaveChangesAsync();

                return Ok(new { message = "Role deleted successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                // Optionally log the exception here
                return BadRequest(new { message = "Role could not be deleted because it is assigned to one or more users or projects." });
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the role." });
            }
        }
        #endregion

    }
}





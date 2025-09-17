using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

[Route("api/[controller]")]
[ApiController]
public class ProjectMemberController : ControllerBase
{
    private readonly ProjectManagementContext _context;

    public ProjectMemberController(ProjectManagementContext context)
    {
        _context = context;
    }

    // GET: api/ProjectMember
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var members = await _context.PrjProjectMembers
       .Include(pm => pm.Project)
       .Include(pm => pm.User)
       .ToListAsync();

        foreach (var member in members)
        {
            member.ProjectName = member.Project?.ProjectName ?? "";
            member.UserName = member.User?.FullName ?? "";
        }

        return Ok(members);
    }


    // GET: api/ProjectMember/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pm = await _context.PrjProjectMembers
            .Include(p => p.Project)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ProjectMemberId == id);

        if (pm == null)
            return NotFound();

        var result = new PrjProjectMember
        {
            ProjectMemberId = pm.ProjectMemberId,
            ProjectId = pm.ProjectId,
            UserId = pm.UserId,
            RoleInProject = pm.RoleInProject,
            ProjectName = pm.Project.ProjectName,
            UserName = pm.User.FullName
        };

        return Ok(result);
    }

    // POST: api/ProjectMember
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrjProjectMember model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new PrjProjectMember
        {
            ProjectId = model.ProjectId,
            UserId = model.UserId,
            RoleInProject = model.RoleInProject
        };

        _context.PrjProjectMembers.Add(entity);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // PUT: api/ProjectMember/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PrjProjectMember model)
    {
        if (id != model.ProjectMemberId)
            return BadRequest("ID mismatch");

        var entity = await _context.PrjProjectMembers.FindAsync(id);
        if (entity == null)
            return NotFound();

        entity.ProjectId = model.ProjectId;
        entity.UserId = model.UserId;
        entity.RoleInProject = model.RoleInProject;

        _context.PrjProjectMembers.Update(entity);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/ProjectMember/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.PrjProjectMembers.FindAsync(id);
        if (entity == null)
            return NotFound();

        _context.PrjProjectMembers.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

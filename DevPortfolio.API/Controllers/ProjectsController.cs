using Microsoft.AspNetCore.Mvc;
using DevPortfolio.API.Data;
using DevPortfolio.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }


    // ==========================================
    // GET ALL PROJECTS
    // PUBLIC
    // ==========================================

    // GET: api/Projects
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _context.Projects
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(projects);
    }


    // ==========================================
    // GET PROJECT BY ID
    // PUBLIC
    // ==========================================

    // GET: api/Projects/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        return Ok(project);
    }


    // ==========================================
    // CREATE PROJECT
    // ADMIN ONLY
    // ==========================================

    // POST: api/Projects
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Project project)
    {
        project.CreatedAt = DateTime.Now;

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Project created successfully!",
            project
        });
    }


    // ==========================================
    // UPDATE PROJECT
    // ADMIN ONLY
    // ==========================================

    // PUT: api/Projects/1
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        Project request)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        project.Title = request.Title;
        project.Description = request.Description;
        project.Technologies = request.Technologies;
        project.ImageUrl = request.ImageUrl;
        project.ProjectUrl = request.ProjectUrl;
        project.GithubUrl = request.GithubUrl;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Project updated successfully!",
            project
        });
    }


    // ==========================================
    // DELETE PROJECT
    // ADMIN ONLY
    // ==========================================

    // DELETE: api/Projects/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Project deleted successfully!"
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly NoteDbContext _context;

        public ProjectController(NoteDbContext context)
        {
            _context = context;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _context.Projects
            .Include(projects => projects.Notes)
            .ThenInclude(note => note.NoteAttributes)
            .ThenInclude(noteAttr => noteAttr.Attribute)
            .ToListAsync();

            var projectDtos = projects.Select(project => new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Notes = project.Notes?.Select(note => new NoteDto
                {
                    NoteId = note.NoteId,
                    NoteText = note.NoteText,
                    CreatedAt = note.CreatedAt,
                    ProjectId = note.ProjectId,
                    NoteAttributeNames = note.NoteAttributes.Select(attr => attr.Attribute.AttributeName).ToList(),
                }).ToList(),
            });

            return Ok(projectDtos);
        }

        // GET: api/Project/5
        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(project => project.Notes)
                .ThenInclude(note => note.NoteAttributes)
                .ThenInclude(noteAttr => noteAttr.Attribute) 
                .FirstOrDefaultAsync(project => project.ProjectId == id);

            if (project == null)
            {
                return NotFound("Project was deleted or does not exist.");
            }

            // Project to ProjectDto mapping
            var projectDto = new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Notes = project.Notes?.Select(note => new NoteDto
                {
                    NoteId = note.NoteId,
                    NoteText = note.NoteText,
                    CreatedAt = note.CreatedAt,
                    ProjectId = note.ProjectId,
                    NoteAttributeNames = note.NoteAttributes?.Select(attr => attr.Attribute.AttributeName).ToList(),
                }).ToList()
            };

            return projectDto;
        }


        // PUT: api/Project/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, ProjectUpdateDto projectUpdateDto)
        {
            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return BadRequest();
            }

            existingProject.ProjectName = projectUpdateDto.ProjectName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Project
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectCreateDto projectCreateDto)
        {
            if (projectCreateDto == null)
            {
                return BadRequest("Invalid project data.");
            }

            var project = new Project
            {
                ProjectName = projectCreateDto.ProjectName,
                Notes = new List<Note>()
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var projectResponse = new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
            };

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, projectResponse);
        }

        // DELETE: api/Project/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}

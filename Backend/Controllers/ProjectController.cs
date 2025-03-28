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
        /*
            Get all projects from the database. 
        */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            // Get all projects, include the notes associated with each project. 
            // Include all of the attribute ids associated with each note via the join table. 
            // Then include the names of the attributes via their ids. 
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
                    NoteAttributeNames = note.NoteAttributes
                        .Select(attr => attr.Attribute.AttributeName).ToList(),
                }).ToList(),
            });

            return Ok(projectDtos);
        }

        // GET: api/Project/5
        /*
            Get the project associated with the passed in id. 
        */
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            // Get the project, include the notes associated with each project. 
            // Include all of the attribute ids associated with each note via the join table. 
            // Then include the names of the attributes via their ids. 
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
                    NoteAttributeNames = note.NoteAttributes?
                        .Select(attr => attr.Attribute.AttributeName).ToList(),
                }).ToList()
            };

            return projectDto;
        }


        // PUT: api/Project/5
        /*
           Update the project based on the id passed in. 
        */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, ProjectUpdateDto projectUpdateDto)
        {
            // Check to see that the project exists. 
            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return BadRequest();
            }

            // Currently only able to edit the name of the project.
            // TODO: deleteing notes functionality.
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
        /*
            Create a new project. 
        */
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectCreateDto projectCreateDto)
        {
            // If the input fields are not filled in, do not create the project.
            if (projectCreateDto == null)
            {
                return BadRequest("Invalid project data.");
            }

            // Create a new Project object and set the name to the name from the form.
            // Create an array to hold Note, set to empty to add notes later. 
            var project = new Project
            {
                ProjectName = projectCreateDto.ProjectName,
                Notes = new List<Note>()
            };

            // Add the project to the DB.
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Response body after creation.
            var projectResponse = new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
            };

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, projectResponse);
        }

        [HttpGet("getProjectNoteCounts")]
        /*
            Gets the count of notes per project. 
        */
        public async Task<ActionResult> GetProjectNoteCounts()
        {
            // Group notes by projectId
            var projectNoteCounts = await _context.Notes
                .GroupBy(note => note.ProjectId)
                .Select(group => new 
                {
                    ProjectId = group.Key,
                    Count = group.Count()
                }).ToListAsync();

            // Group notes that do not have a projectId
            var unassignedNoteCount = await _context.Notes
                .Where(note => note.ProjectId == null)
                .CountAsync();
            
            return Ok(new {
                ProjectNoteCounts = projectNoteCounts,
                UnassignedNoteCount = unassignedNoteCount
            });
        }

        [HttpGet("getAttributeCounts")]
        /*
            Get the count of each attribute used. 
        */
        public async Task<ActionResult> GetAttributeCounts()
        {
            // Group the NoteAttributeJoin By AttributeId and count the occurrences
            var attributeNoteCounts = await _context.NoteAttributes
                .GroupBy(noteAttr => noteAttr.AttributeId)
                .Select(group => new 
                {
                    AttributeId = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            // Join the result with the Attributeentity to get the names
            var attributeDetails = await _context.Attributes
                .Where(attr => attributeNoteCounts
                .Select(attr => attr.AttributeId)
                .Contains(attr.AttributeId))
                .ToListAsync();

            // Combine the grouped counts with the attribute names
            var result = attributeNoteCounts
            .Join(attributeDetails, noteCount => noteCount.AttributeId, 
                attribute => attribute.AttributeId, (noteCount, attribute) => new 
            {
                AttributeId = noteCount.AttributeId,
                AttributeName = attribute.AttributeName,
                Count = noteCount.Count
            }).ToList();

            // Count notes without any attributes
            var unassignedNoteCount = await _context.Notes
                .Where(note => !note.NoteAttributes
                .Any()).CountAsync();

            return Ok(new
            {
                AttributeNoteCounts = result,
                UnassignedAttributeCount = unassignedNoteCount
            });
        }


        // DELETE: api/Project/5
        /*
            Delete the project based on the passed in id. 
            In the DB Context, it is set up so if a project is deleted the notes
            associated with that project are not also deleted. 
        */
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

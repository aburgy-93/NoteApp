using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using NuGet.Packaging;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly NoteDbContext _context;

        public NotesController(NoteDbContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            var notes = await _context.Notes.Select(note => new NoteDto {
                NoteId = note.NoteId,
                CreatedAt = note.CreatedAt,
                NoteText = note.NoteText,
                ProjectId = note.ProjectId,
                NoteAttributeNames = note.NoteAttributes.Select(attr => attr.Attribute.AttributeName).ToList(),
            }).ToListAsync();

            return Ok(notes);
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var note = await _context.Notes
                .Include(note => note.Project)
                .Include(note => note.NoteAttributes)
                    .ThenInclude(noteAttr => noteAttr.Attribute)
                .FirstOrDefaultAsync(note => note.NoteId == id);

            if (note == null)
            {
                return NotFound();
            }

             var noteDto = new NoteDto {
                NoteId = note.NoteId,
                CreatedAt = note.CreatedAt,
                NoteText = note.NoteText,
                ProjectId = note.ProjectId, 
                NoteAttributeNames = note.NoteAttributes
                    .Where(na => na.Attribute != null)
                    .Select(na => na.Attribute.AttributeName)
                    .ToList()
             };

            return Ok(noteDto);
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, NoteUpdateDto noteUpdateDto)
        {
            var existingNote = await _context.Notes
                .Include(note => note.NoteAttributes)
                .FirstOrDefaultAsync(note => note.NoteId == id);

            if (existingNote == null)
            {
                return BadRequest("Note does not exist.");
            }

            // Do not update NoteText if it has not changed
            if (noteUpdateDto.NoteText != null && noteUpdateDto.NoteText != "string" 
                && existingNote.NoteText != noteUpdateDto.NoteText)
            {
                existingNote.NoteText = noteUpdateDto.NoteText;
            }

            // Update ProjectId only if it has changed
            if (noteUpdateDto.ProjectId.HasValue && noteUpdateDto.ProjectId.Value 
                != 0 && existingNote.ProjectId != noteUpdateDto.ProjectId)
            {
                // Check that project exists
                var projectExists = await _context.Projects.AnyAsync(project => project.ProjectId == noteUpdateDto.ProjectId);
                if(!projectExists)
                {
                    return BadRequest("Project does not exist");
                }

                // Update ProjectId if it has changed.
                existingNote.ProjectId = noteUpdateDto.ProjectId;
            }



            // Handle NoteAttributes only if it has changed
            if (noteUpdateDto.NoteAttributes != null)
            {
                // Add new attirbutes that aren't already in the existing list
                foreach (var attributeId in noteUpdateDto.NoteAttributes) {
                    // Only add new attributes that are'nt already in the existing list
                    if (!existingNote.NoteAttributes.Any(noteAttr => noteAttr.AttributeId == attributeId))
                    {
                        var newAttributeJoin = new NoteAttributeJoin
                        {
                            NoteId = existingNote.NoteId,
                            AttributeId = attributeId
                        };
                        existingNote.NoteAttributes.Add(newAttributeJoin);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
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

        // POST: api/Notes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost]
        public async Task<ActionResult<Note>> PostNote(NoteCreateDto noteCreateDto)
        {
            if (noteCreateDto == null)
            {
                return BadRequest("Invalid note data.");
            }

            // Use DTO to create new note
            var note = new Note
            {
                NoteText = noteCreateDto.NoteText,
                CreatedAt = DateTime.UtcNow,
                ProjectId = noteCreateDto.ProjectId == 0 ? null : noteCreateDto.ProjectId,
                NoteAttributes = new List<NoteAttributeJoin>()
            };

            // Add the note to the context and save it to get the note's generated Id
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Now create the NoteAttributeJoins for each attributeId
            if (noteCreateDto.NoteAttributes != null)
            {
                // Remove any invalid attributeId (0 is invalid)
                noteCreateDto.NoteAttributes = noteCreateDto.NoteAttributes.Where(id => id != 0).ToList();
                
                foreach (var attributeId in noteCreateDto.NoteAttributes)
                {
                    // Check if the Attribute with this ID exists in the DB
                    var attribute = await _context.Attributes.FindAsync(attributeId);
                    if(attribute == null) 
                    {
                        return BadRequest($"Attribute with ID {attributeId} does not exist.");
                    }

                    // If the attribute exists, create the NoteAttributeJoin
                    var noteAttributeJoin = new NoteAttributeJoin
                    {
                        NoteId = note.NoteId,
                        AttributeId = attributeId
                    };

                    _context.NoteAttributes.Add(noteAttributeJoin);
                }

                await _context.SaveChangesAsync();
            }

            var responseNote = new 
            {
                note.NoteId,
                note.CreatedAt,
                note.NoteText,
                note.ProjectId,
                noteAttributes = note.NoteAttributes
                    .Select(noteAttr => noteAttr.Attribute.AttributeName).ToList()
            };

            return CreatedAtAction("GetNote", new { id = note.NoteId }, responseNote);
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.NoteId == id);
        }
    }
}

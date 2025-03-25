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
            return await _context.Notes.ToListAsync();
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, NoteUpdateDto noteUpdateDto)
        {
            var existingNote = await _context.Notes.FindAsync(id);
            if (existingNote == null)
            {
                return BadRequest();
            }

            existingNote.NoteText = noteUpdateDto.NoteText;

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
        public async Task<ActionResult<Note>> PostNote(NoteDto noteDto)
        {
            if (noteDto == null)
            {
                return BadRequest("Invalid note data.");
            }

            // Use DTO to create new note
            var note = new Note 
            {
                NoteText = noteDto.NoteText,
                CreatedAt = DateTime.UtcNow,
                ProjectId = noteDto.ProjectId == 0 ? null : noteDto.ProjectId,
                NoteAttributes = new List<NoteAttributeJoin>()
            };

            // Add the note to the context and save it to get the note's generated Id
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            // Now create the NoteAttributeJoins for each attributeId
            if (noteDto.NoteAttributes != null)
            {
                // Remove any invalid attributeId (0 is invalid)
                noteDto.NoteAttributes = noteDto.NoteAttributes.Where(id => id != 0).ToList();
                
                foreach (var attributeId in noteDto.NoteAttributes)
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

            return CreatedAtAction("GetNote", new { id = note.NoteId }, note);
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

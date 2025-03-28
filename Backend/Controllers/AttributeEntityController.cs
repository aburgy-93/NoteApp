using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttributeEntityController : ControllerBase
    {
        private readonly NoteDbContext _context;

        public AttributeEntityController(NoteDbContext context)
        {
            _context = context;
        }

        // GET: api/AttributeEntity
        /*
            Return all of the attributes in the context. 
        */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttributeEntity>>> GetAttributes()
        {
            return await _context.Attributes.ToListAsync();
        }

        // GET: api/AttributeEntity/5
        /*
            Return an attribute via an entered id. 
        */
        [HttpGet("{id}")]
        public async Task<ActionResult<AttributeEntity>> GetAttributeEntity(int id)
        {
            var attributeEntity = await _context.Attributes.FindAsync(id);

            if (attributeEntity == null)
            {
                return NotFound();
            }

            return attributeEntity;
        }

        // PUT: api/AttributeEntity/5
         /*
            Update an attribute corresponding to the entered id. 
        */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributeEntity(int id, AttributeDto attributeDto)
        {
            var existingAttribute = await _context.Attributes.FindAsync(id);
            if (existingAttribute == null)
            {
                return BadRequest("Could not find attribute.");
            }

            existingAttribute.AttributeName = attributeDto.AttributeName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttributeEntityExists(id))
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

        // POST: api/AttributeEntity
        /*
            Create a new attribute and add it to the database. 
        */
        [HttpPost]
        public async Task<ActionResult<AttributeEntity>> PostAttributeEntity(AttributeDto attributeDto)
        {
            if (attributeDto == null) {
                return BadRequest("Attribute cannot be null.");
            }
            var newAttribute = new AttributeEntity
            {
                AttributeName = attributeDto.AttributeName
            };

            _context.Attributes.Add(newAttribute);
            await _context.SaveChangesAsync();

            var attributeResponse = new AttributeDto 
            {
                AttributeId = newAttribute.AttributeId,
                AttributeName = newAttribute.AttributeName
            };

            return CreatedAtAction("GetAttributeEntity", new { id = newAttribute.AttributeId }, attributeResponse);
        }

        // DELETE: api/AttributeEntity/5
        /*
            Delete an attribute based on the id passed in. 
        */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributeEntity(int id)
        {
            var attributeEntity = await _context.Attributes.FindAsync(id);
            if (attributeEntity == null)
            {
                return NotFound();
            }

            _context.Attributes.Remove(attributeEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttributeEntityExists(int id)
        {
            return _context.Attributes.Any(e => e.AttributeId == id);
        }
    }
}

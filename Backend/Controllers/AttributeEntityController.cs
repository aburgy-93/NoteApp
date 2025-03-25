using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttributeEntity>>> GetAttributes()
        {
            return await _context.Attributes.ToListAsync();
        }

        // GET: api/AttributeEntity/5
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributeEntity(int id, AttributeEntity attributeEntity)
        {
            if (id != attributeEntity.AttributeId)
            {
                return BadRequest();
            }

            _context.Entry(attributeEntity).State = EntityState.Modified;

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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AttributeEntity>> PostAttributeEntity(AttributeEntity attributeEntity)
        {
            _context.Attributes.Add(attributeEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttributeEntity", new { id = attributeEntity.AttributeId }, attributeEntity);
        }

        // DELETE: api/AttributeEntity/5
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

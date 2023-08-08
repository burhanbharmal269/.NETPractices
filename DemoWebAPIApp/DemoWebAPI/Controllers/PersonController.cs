using DemoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonDbContext _context;

        public PersonController(PersonDbContext context)
        {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            if(_context.Person == null)
            {
                return NotFound();
            }
            return await _context.Person.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            if (_context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person.FindAsync(id);
            
            if(person == null)
            {
                return NotFound();
            }
            return person;
        }

        [HttpPost("Post")]
        public async Task<ActionResult<Person>> Post(Person person)
        {
            _context.Person.Add(person);
            await _context.SaveChangesAsync();
            return Ok(person);
        }

        [HttpPut]
        public async Task<ActionResult<Person>> Put(int id, Person person)
        {
            if(id != person.Id)
            {
                return BadRequest();
            }
             _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!PersonAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(person);
        }
        private bool PersonAvailable(int id)
        {
            return (_context.Person?.Any(x => x.Id == id)).GetValueOrDefault(); 
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if(_context.Person == null)
            {
                return NotFound();
            }
            var person = await _context.Person.FindAsync(id);

            if(person == null)
            {
                return NotFound();
            }
            _context.Remove(person);
            await _context.SaveChangesAsync();
            return Ok();

        }



    }
}

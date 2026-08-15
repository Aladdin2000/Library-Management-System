using Library_Management_System.Api;
using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublishersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherReadDto>>> GetPublishers()
        {
            var publishers = await _context.Publishers
                .Select(p => new PublisherReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    Website = p.Website
                })
                .ToListAsync();

            return Ok(publishers);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherReadDto>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                return NotFound();

            return Ok(new PublisherReadDto
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Address = publisher.Address,
                Website = publisher.Website
            });
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<ActionResult> CreatePublisher(PublisherCreateDto dto)
        {
            var publisher = new Publisher
            {
                Name = dto.Name,
                Address = dto.Address,
                Website = dto.Website
            };

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, null);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePublisher(int id, PublisherCreateDto dto)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                return NotFound();

            publisher.Name = dto.Name;
            publisher.Address = dto.Address;
            publisher.Website = dto.Website;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
                return NotFound();

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
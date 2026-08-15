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
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorReadDto>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorReadDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Bio = a.Bio
                })
                .ToListAsync();

            return Ok(authors);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorReadDto>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            return Ok(new AuthorReadDto
            {
                Id = author.Id,
                FullName = author.FullName,
                Bio = author.Bio
            });
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<ActionResult> CreateAuthor(AuthorCreateDto dto)
        {
            var author = new Author
            {
                FullName = dto.FullName,
                Bio = dto.Bio
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, null);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuthor(int id, AuthorCreateDto dto)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            author.FullName = dto.FullName;
            author.Bio = dto.Bio;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
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
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberReadDto>>> GetMembers()
        {
            var members = await _context.Members
                .Select(m => new MemberReadDto
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    Address = m.Address,
                    MembershipDate = m.MembershipDate,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(members);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberReadDto>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            return Ok(new MemberReadDto
            {
                Id = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                Address = member.Address,
                MembershipDate = member.MembershipDate,
                IsActive = member.IsActive
            });
        }

        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [HttpPost]
        public async Task<ActionResult> CreateMember(MemberCreateDto dto)
        {
            var member = new Member
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, null);
        }

        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMember(int id, MemberCreateDto dto)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            member.FullName = dto.FullName;
            member.Email = dto.Email;
            member.PhoneNumber = dto.PhoneNumber;
            member.Address = dto.Address;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
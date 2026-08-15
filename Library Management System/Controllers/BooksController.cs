using Library_Management_System.Api;
using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // يعني الـ base route هيبقى api/books
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // بنستخدم Dependency Injection عشان ناخد نسخة من الـ DbContext جاهزة
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        // متاح لأي حد مسجل دخول (من غير تحديد Role) — مفيش [Authorize] بـ Roles هنا
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadDto>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Select(b => new BookReadDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Edition = b.Edition,
                    Summary = b.Summary,
                    CoverImageUrl = b.CoverImageUrl,
                    Language = b.Language,
                    PublicationYear = b.PublicationYear,
                    Status = b.Status.ToString(),
                    PublisherName = b.Publisher!.Name,
                    CategoryName = b.Category!.Name,
                    Authors = b.BookAuthors.Select(ba => ba.Author!.FullName).ToList()
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/books/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadDto>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound(); // بيرجع 404 لو مش موجود

            var dto = new BookReadDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Edition = book.Edition,
                Summary = book.Summary,
                CoverImageUrl = book.CoverImageUrl,
                Language = book.Language,
                PublicationYear = book.PublicationYear,
                Status = book.Status.ToString(),
                PublisherName = book.Publisher!.Name,
                CategoryName = book.Category!.Name,
                Authors = book.BookAuthors.Select(ba => ba.Author!.FullName).ToList()
            };

            return Ok(dto);
        }

        // POST: api/books
        // بس Administrator و Librarian يقدروا يضيفوا كتب (مش Staff)
        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<ActionResult> CreateBook(BookCreateDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                Edition = dto.Edition,
                Summary = dto.Summary,
                CoverImageUrl = dto.CoverImageUrl,
                Language = dto.Language,
                PublicationYear = dto.PublicationYear,
                PublisherId = dto.PublisherId,
                CategoryId = dto.CategoryId,
                Status = BookStatus.In // كتاب جديد يبدأ دايمًا "In"
            };

            // بنربط كل AuthorId جالنا بالـ BookAuthors (جدول الربط)
            foreach (var authorId in dto.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // بيرجع 201 Created مع رابط للمصدر الجديد
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, null);
        }

        // PUT: api/books/5
        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, BookCreateDto dto)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            book.Title = dto.Title;
            book.ISBN = dto.ISBN;
            book.Edition = dto.Edition;
            book.Summary = dto.Summary;
            book.CoverImageUrl = dto.CoverImageUrl;
            book.Language = dto.Language;
            book.PublicationYear = dto.PublicationYear;
            book.PublisherId = dto.PublisherId;
            book.CategoryId = dto.CategoryId;

            // بنشيل كل المؤلفين القدام ونضيف الجداد (أبسط طريقة)
            book.BookAuthors.Clear();
            foreach (var authorId in dto.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { BookId = id, AuthorId = authorId });
            }

            await _context.SaveChangesAsync();
            return NoContent(); // 204: نجح التعديل بس مفيش محتوى نرجعه
        }

        // DELETE: api/books/5
        // بس Administrator يقدر يحذف كتاب
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/books/search?name=..&author=..&category=..
// كل الـ parameters اختيارية، تقدر تبعت واحد بس أو أكتر من واحد مع بعض
[Authorize]
[HttpGet("search")]
public async Task<ActionResult<IEnumerable<BookReadDto>>> SearchBooks(
    [FromQuery] string? name,
    [FromQuery] string? author,
    [FromQuery] string? category)
{
    var query = _context.Books
        .Include(b => b.Publisher)
        .Include(b => b.Category)
        .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
        .AsQueryable(); // بنبدأ بـ query قابل للتعديل قبل ما ننفذه فعليًا

    // بنضيف شرط لو الـ parameter مبعوت بس (مش null أو فاضي)
    if (!string.IsNullOrWhiteSpace(name))
        query = query.Where(b => b.Title.Contains(name));

    if (!string.IsNullOrWhiteSpace(author))
        query = query.Where(b => b.BookAuthors.Any(ba => ba.Author!.FullName.Contains(author)));

    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(b => b.Category!.Name.Contains(category));

    var books = await query
        .Select(b => new BookReadDto
        {
            Id = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            Edition = b.Edition,
            Summary = b.Summary,
            CoverImageUrl = b.CoverImageUrl,
            Language = b.Language,
            PublicationYear = b.PublicationYear,
            Status = b.Status.ToString(),
            PublisherName = b.Publisher!.Name,
            CategoryName = b.Category!.Name,
            Authors = b.BookAuthors.Select(ba => ba.Author!.FullName).ToList()
        })
        .ToListAsync();

    return Ok(books);
}

// GET: api/books/status/In  أو  api/books/status/Out
[Authorize]
[HttpGet("status/{status}")]
public async Task<ActionResult<IEnumerable<BookReadDto>>> GetBooksByStatus(string status)
{
    // بنحاول نحول النص المبعوت (زي "In" أو "Out") لقيمة من الـ enum BookStatus
    if (!Enum.TryParse<BookStatus>(status, true, out var bookStatus))
        return BadRequest("Invalid status. Use 'In' or 'Out'.");

    var books = await _context.Books
        .Where(b => b.Status == bookStatus)
        .Include(b => b.Publisher)
        .Include(b => b.Category)
        .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
        .Select(b => new BookReadDto
        {
            Id = b.Id,
            Title = b.Title,
            ISBN = b.ISBN,
            Edition = b.Edition,
            Summary = b.Summary,
            CoverImageUrl = b.CoverImageUrl,
            Language = b.Language,
            PublicationYear = b.PublicationYear,
            Status = b.Status.ToString(),
            PublisherName = b.Publisher!.Name,
            CategoryName = b.Category!.Name,
            Authors = b.BookAuthors.Select(ba => ba.Author!.FullName).ToList()
        })
        .ToListAsync();

    return Ok(books);
}
    }
}
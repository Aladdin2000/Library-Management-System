using Library_Management_System.Api;
using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BorrowingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/borrowing
        // بيرجع كل عمليات الاستعارة (المرجعة وغير المرجعة)
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowingReadDto>>> GetAll()
        {
            var transactions = await _context.BorrowingTransactions
                .Include(t => t.Book)
                .Include(t => t.Member)
                .Include(t => t.HandledByUser)
                .Select(t => new BorrowingReadDto
                {
                    Id = t.Id,
                    BookId = t.BookId,
                    BookTitle = t.Book!.Title,
                    MemberId = t.MemberId,
                    MemberName = t.Member!.FullName,
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    ReturnDate = t.ReturnDate,
                    HandledByUserName = t.HandledByUser != null ? t.HandledByUser.FullName : null
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // GET: api/borrowing/active
        // بيرجع بس الكتب اللي لسه برة (متسجلش رجوعها)، مفيدة لمعرفة مين ماخد إيه دلوقتي
        [Authorize]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BorrowingReadDto>>> GetActive()
        {
            var transactions = await _context.BorrowingTransactions
                .Where(t => t.ReturnDate == null) // لسه مرجعتش
                .Include(t => t.Book)
                .Include(t => t.Member)
                .Include(t => t.HandledByUser)
                .Select(t => new BorrowingReadDto
                {
                    Id = t.Id,
                    BookId = t.BookId,
                    BookTitle = t.Book!.Title,
                    MemberId = t.MemberId,
                    MemberName = t.Member!.FullName,
                    BorrowDate = t.BorrowDate,
                    DueDate = t.DueDate,
                    ReturnDate = t.ReturnDate,
                    HandledByUserName = t.HandledByUser != null ? t.HandledByUser.FullName : null
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // POST: api/borrowing/borrow
        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [HttpPost("borrow")]
        public async Task<ActionResult> Borrow(BorrowCreateDto dto)
        {
            var book = await _context.Books.FindAsync(dto.BookId);
            if (book == null)
                return NotFound("Book not found.");

            // أهم تحقق هنا: لازم الكتاب يكون "In" قبل ما تقدر تستعيره
            if (book.Status == BookStatus.Out)
                return BadRequest("This book is already borrowed.");

            var member = await _context.Members.FindAsync(dto.MemberId);
            if (member == null)
                return NotFound("Member not found.");

            if (!member.IsActive)
                return BadRequest("This member's account is not active.");

            // بنجيب الـ Id بتاع اليوزر المسجل دخول دلوقتي من داخل الـ JWT Token نفسه
            // ClaimTypes.NameIdentifier بيرجع الـ Sub claim اللي حطينها وقت عمل الـ Token (user.Id)
            // كان: var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // دلوقتي لازم نحوله لـ int
            var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentUserId = currentUserIdString != null ? int.Parse(currentUserIdString) : null;

            var transaction = new BorrowingTransaction
            {
                BookId = dto.BookId,
                MemberId = dto.MemberId,
                BorrowDate = DateTime.UtcNow,
                DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(14),
                HandledByUserId = currentUserId
            };
            // بنحدث حالة الكتاب لـ "Out"
            book.Status = BookStatus.Out;

            _context.BorrowingTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book borrowed successfully.", transactionId = transaction.Id });
        }

        // POST: api/borrowing/return/5
        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [HttpPost("return/{transactionId}")]
        public async Task<ActionResult> Return(int transactionId)
        {
            var transaction = await _context.BorrowingTransactions
                .Include(t => t.Book)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return NotFound("Borrowing transaction not found.");

            if (transaction.ReturnDate != null)
                return BadRequest("This book was already returned.");

            transaction.ReturnDate = DateTime.UtcNow;

            // بنرجع حالة الكتاب لـ "In"
            if (transaction.Book != null)
                transaction.Book.Status = BookStatus.In;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Book returned successfully." });
        }
    }
}
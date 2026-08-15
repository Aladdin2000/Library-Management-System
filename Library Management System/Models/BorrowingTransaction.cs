namespace Library_Management_System.Models
{
    public class BorrowingTransaction
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int MemberId { get; set; }
        public Member? Member { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int? HandledByUserId { get; set; }
        public ApplicationUser? HandledByUser { get; set; }
    }
}
namespace Library_Management_System.Models
{
    public enum BookStatus
    {
        In,   
        Out   
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public int? PublicationYear { get; set; }

        public BookStatus Status { get; set; } = BookStatus.In;

        public int PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
    }
}
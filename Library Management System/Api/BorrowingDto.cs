namespace Library_Management_System.Api
{
    public class BorrowCreateDto
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class BorrowingReadDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? HandledByUserName { get; set; }
    }
}
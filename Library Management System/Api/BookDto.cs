namespace Library_Management_System.Api
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public int? PublicationYear { get; set; }

        public int PublisherId { get; set; }
        public int CategoryId { get; set; }

        public List<int> AuthorIds { get; set; } = new();
    }

    public class BookReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public int? PublicationYear { get; set; }
        public string Status { get; set; } = string.Empty;

        public string PublisherName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public List<string> Authors { get; set; } = new();
    }
}
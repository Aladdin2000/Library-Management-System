namespace Library_Management_System.Api
{
    public class PublisherCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }
    }

    public class PublisherReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }
    }
}
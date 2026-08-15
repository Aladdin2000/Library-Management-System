namespace Library_Management_System.Api
{
    public class AuthorCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
    }

    public class AuthorReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
    }
}
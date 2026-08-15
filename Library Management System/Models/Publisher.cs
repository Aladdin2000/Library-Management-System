namespace Library_Management_System.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Website { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
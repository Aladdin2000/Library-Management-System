namespace Library_Management_System.Models
{
    
        public class Author
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Bio { get; set; }

            public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        }
    }
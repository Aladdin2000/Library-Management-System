namespace Library_Management_System.Api
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public int? ParentCategoryId { get; set; }
    }

    public class CategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
    }
}
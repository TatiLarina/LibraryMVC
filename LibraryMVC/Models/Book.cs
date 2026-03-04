namespace LibraryMVC.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Genre { get; set; }
        public int? Pages { get; set; }
        public string? ISBN { get; set; }
        public DateTime? DateAdded { get; set; }
        public string? SchemaXml { get; set; }
    }
}

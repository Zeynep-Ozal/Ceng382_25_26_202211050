namespace RazorPage.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; } // used for actions only
        public string? ClassName { get; set; }
        public int StudentCount { get; set; }
        public string? Description { get; set; }
    }
}

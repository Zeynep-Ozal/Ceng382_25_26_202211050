using System.ComponentModel.DataAnnotations;  
namespace RazorPage.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 1;

        public int Id { get;set; } 

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required.")]
        [Range(1, 500, ErrorMessage = "Student Count must be between 1 and 500.")]
        public int StudentCount { get; set; }

        public string Description { get; set; }

        public ClassInformationModel()
        {
            Id = _idCounter++; // Her yeni nesnede ID artır
        }
    }
}

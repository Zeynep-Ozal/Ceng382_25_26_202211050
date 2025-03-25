using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Collections.Generic;
using System.Linq;  // To use LINQ methods

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        // Static list to simulate a database
        private static List<ClassInformationModel> _classList = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; }

        public List<ClassInformationModel> ClassList => _classList; 

        public void OnGet()
        {
            // Ensure NewClass is initialized when the page loads
            NewClass = NewClass ?? new ClassInformationModel();
        }

        // Add new class
        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (NewClass.Id == 0)
            {
                NewClass.Id = _classList.Count > 0 ? _classList.Max(c => c.Id) + 1 : 1;
            }
            _classList.Add(NewClass);
            return RedirectToPage(); 
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = _classList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                _classList.Remove(classToRemove);
            }
            return RedirectToPage(); 
        }

        public IActionResult OnGetEdit(int id)
        {
            var classToEdit = _classList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = classToEdit; // Populate NewClass for editing
            }
            return Page();  
        }

        // Save the edited class
        public IActionResult OnPostEdit()
        {
            var classToEdit = _classList.FirstOrDefault(c => c.Id == NewClass.Id);
            if (classToEdit != null)
            {
                classToEdit.ClassName = NewClass.ClassName;
                classToEdit.StudentCount = NewClass.StudentCount;
                classToEdit.Description = NewClass.Description;
            }
            return RedirectToPage();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;

namespace RazorPage.Pages
{
   public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassesData = new();

        [BindProperty]
        public required ClassInformationModel NewClass { get; set; }

        public List<ClassInformationModel> Classes => ClassesData;

        [BindProperty]
        public int EditIndex { get; set; }

        [BindProperty]
        public int DeleteIndex { get; set; }

        public bool IsEditMode => TempData["EditIndex"] != null;

        public void OnGet()
        {
            if (TempData["EditIndex"] != null)
            {
                int idx = (int)TempData["EditIndex"];
                EditIndex = idx; 
                NewClass = new ClassInformationModel
                {
                    ClassName = ClassesData[idx].ClassName,
                    Description = ClassesData[idx].Description,
                    StudentCount = ClassesData[idx].StudentCount
                };
            }
        }

        public IActionResult OnPost()
        {
            if (EditIndex >= 0 && EditIndex < ClassesData.Count)
            {
                ClassesData[EditIndex].ClassName = NewClass.ClassName;
                ClassesData[EditIndex].Description = NewClass.Description;
                ClassesData[EditIndex].StudentCount = NewClass.StudentCount;
            }
            else
            {
                NewClass.Id = ClassesData.Count + 1;
                ClassesData.Add(NewClass);
            }

            TempData.Remove("EditIndex"); 
            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            if (DeleteIndex >= 0 && DeleteIndex < ClassesData.Count)
            {
                ClassesData.RemoveAt(DeleteIndex);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            TempData["EditIndex"] = EditIndex;
            return RedirectToPage();
        }
    }
}
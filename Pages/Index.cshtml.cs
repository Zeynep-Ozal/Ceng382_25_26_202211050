using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Collections.Generic;
using System.Linq;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> AllClasses = GenerateSampleData();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        [BindProperty]
        public int EditIndex { get; set; }

        public List<ClassInformationTable> FilteredClasses { get; set; } = new();

        public bool IsEditMode => TempData["EditIndex"] != null;

        public void OnGet()
        {
            var query = AllClasses.AsQueryable();

            if (!string.IsNullOrEmpty(Filter))
            {
                query = query.Where(c => c.ClassName != null && c.ClassName.Contains(Filter, System.StringComparison.OrdinalIgnoreCase));
            }

            TotalPages = (int)System.Math.Ceiling(query.Count() / (double)PageSize);

            FilteredClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();

            if (TempData["EditIndex"] != null)
            {
                int idx = (int)TempData["EditIndex"];
                EditIndex = idx; 
                NewClass = new ClassInformationModel
                {
                    ClassName = AllClasses[idx].ClassName,
                    Description = AllClasses[idx].Description,
                    StudentCount = AllClasses[idx].StudentCount
                };
            }
        }

        public IActionResult OnPost()
        {
            if (EditIndex >= 0 && EditIndex < AllClasses.Count)
            {
                AllClasses[EditIndex].ClassName = NewClass.ClassName;
                AllClasses[EditIndex].Description = NewClass.Description;
                AllClasses[EditIndex].StudentCount = NewClass.StudentCount;
            }
            else
            {
                NewClass.Id = AllClasses.Count + 1;
                AllClasses.Add(NewClass);
            }

            TempData.Remove("EditIndex"); 
            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostDelete(int deleteId)
        {
            var item = AllClasses.FirstOrDefault(c => c.Id == deleteId);
            if (item != null)
            {
                AllClasses.Remove(item);
            }
            return RedirectToPage(new { Filter, PageNumber });
        }

        public IActionResult OnPostEdit()
        {
           
            TempData["EditIndex"] = EditIndex;
            return RedirectToPage();
        
        }

        private static List<ClassInformationModel> GenerateSampleData()
        {
            var list = new List<ClassInformationModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = 20 + (i % 10),
                    Description = $"This is a description for class {i}"
                });
            }
            return list;
        }
    }
}
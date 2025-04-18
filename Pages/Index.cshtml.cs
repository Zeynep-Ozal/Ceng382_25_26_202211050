using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;   


namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {

        public string? CurrentUser { get; set; }

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

        [BindProperty]
        public string? SelectedColumns { get; set; }


      public IActionResult OnGet()
{
    var query = AllClasses.AsQueryable();

    if (!string.IsNullOrEmpty(Filter))
    {
        query = query.Where(c => c.ClassName != null && c.ClassName.Contains(Filter, System.StringComparison.OrdinalIgnoreCase));
    }

    TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);

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

    // Authentication kontrolü
    var sessionUsername = HttpContext.Session.GetString("username");
    var sessionToken = HttpContext.Session.GetString("token");
    var sessionId = HttpContext.Session.GetString("session_id");

    var cookieUsername = Request.Cookies["username"];
    var cookieToken = Request.Cookies["token"];
    var cookieSessionId = Request.Cookies["session_id"];

    bool isAuthenticated =
        !string.IsNullOrEmpty(sessionUsername) &&
        sessionUsername == cookieUsername &&
        sessionToken == cookieToken &&
        sessionId == cookieSessionId;

    if (!isAuthenticated)
    {
        TempData["ErrorMessage"] = "You must log in to access this page.";
        return RedirectToPage("/Login");
    }

    CurrentUser = sessionUsername;

    cookieUsername = Request.Cookies["username"];
    cookieToken = Request.Cookies["token"];
    cookieSessionId = Request.Cookies["session_id"];

    Console.WriteLine($"username: {cookieUsername}");
    Console.WriteLine($"token: {cookieToken}");
    Console.WriteLine($" session_id: {cookieSessionId}");

    return Page();
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

        

        public IActionResult OnPostExport()
        {
            try
            {
                var selectedCols = SelectedColumns?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .ToArray() ?? Array.Empty<string>();
                if (selectedCols.Length == 0)
                {
                selectedCols = typeof(ClassInformationModel)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name)
                    .ToArray();
                }
                var filtered = AllClasses
                    .Where(c => string.IsNullOrEmpty(Filter) || c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase))
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                var dataToExport = filtered.Select(item =>
                {
                    var filteredItem = new Dictionary<string, object>();

                    foreach (var column in selectedCols)
                    {
                        var prop = item.GetType().GetProperty(column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null)
                        {
                            filteredItem[column] = prop.GetValue(item, null) ?? "";
                        }
                    }

                    return filteredItem;
                }).ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(dataToExport, options);
                return File(Encoding.UTF8.GetBytes(json), "application/json", "classes_export.json");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Export failed: {ex.Message}";
                return RedirectToPage();
            }
        }

    }
    }

    

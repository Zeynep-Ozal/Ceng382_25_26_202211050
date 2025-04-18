using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPage.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace RazorPage.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string _jsonPath;

        public LoginModel(IWebHostEnvironment env)
        {
            _jsonPath = Path.Combine(env.WebRootPath, "data", "users.json");
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            if (!System.IO.File.Exists(_jsonPath))
            {
                ErrorMessage = "User data not found.";
                return Page();
            }

            var json = System.IO.File.ReadAllText(_jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json) ?? new();

            var user = users.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                HttpOnly = true,
            };
            Console.WriteLine("printing cookie..");

            Response.Cookies.Append("username", user.Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, options);

            Console.WriteLine("...printed the cookie");


            return RedirectToPage("/Index");
        }
    }
}
//session ve ccokie nasl çalışuypr
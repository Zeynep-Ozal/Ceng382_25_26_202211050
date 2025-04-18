var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();


app.UseRouting();

app.UseAuthorization();

app.MapGet("/", async context =>
{ 
        context.Response.Redirect("/Index");
});


app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

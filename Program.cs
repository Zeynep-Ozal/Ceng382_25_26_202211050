var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(); // 👈 Session servisini ekle

builder.Services.AddRazorPages();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseSession(); // 👈 Session'ı aktif et

app.UseAuthorization();

app.MapGet("/", async context =>
{
    var session = context.Session;

    if (session.GetString("username") != null &&
        session.GetString("token") != null &&
        session.GetString("session_id") != null)
    {
        context.Response.Redirect("/Index");
    }
    else
    {
        context.Response.Redirect("/Login"); 
    }
});


app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

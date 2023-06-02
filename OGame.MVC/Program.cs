using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OGame.MVC.Data;
using System;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database") ??
                       throw new InvalidOperationException("Connection string 'Database' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "game",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToAreaController("Index", "Home", "Game");

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/@vite/client") ||
        context.Request.Path.Value.StartsWith("/src/"))
    {
        if (context.Request.Path.Value.StartsWith("/src/index.css"))
        {
            Debugger.Break();
        }
        
        var client = new HttpClient();
        var serverResponse = await client.GetAsync("http://localhost:3000" + context.Request.Path);
        if (serverResponse.IsSuccessStatusCode)
        {
            var content = await serverResponse.Content.ReadAsByteArrayAsync();

            context.Response.Headers.ContentType = "application/javascript";
            await context.Response.Body.WriteAsync(content);
            return;
        }
    }

    await next.Invoke();
});

app.UseSpa(spa =>
{
    if (app.Environment.IsDevelopment())
    {
        spa.Options.SourcePath = "wwwroot";
    }
});

app.MapRazorPages();

app.Run();

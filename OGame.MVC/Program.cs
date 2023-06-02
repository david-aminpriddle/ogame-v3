using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OGame.MVC.Data;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.WebSockets;
using Yarp.ReverseProxy.Forwarder;

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
builder.Services.AddHttpForwarder();

builder.Services.AddWebSockets(options => {});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}


var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
{
    UseProxy = false,
    AllowAutoRedirect = false,
    AutomaticDecompression = DecompressionMethods.None,
    UseCookies = false,
    ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
    ConnectTimeout = TimeSpan.FromSeconds(15),
});
var transformer = HttpTransformer.Default; // or HttpTransformer.Default;
var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

var httpForwarder = app.Services.GetRequiredService<IHttpForwarder>();

app.UseWebSockets();
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/@vite/client") ||
            context.Request.Path.Value.StartsWith("/src/") ||
            context.Request.Path.Value.StartsWith("/node_modules/")
           )
        {
            var client = new HttpClient();
            var serverResponse = await client.GetAsync("http://localhost:3000" + context.Request.Path);

            if (serverResponse.IsSuccessStatusCode)
            {
                var content = await serverResponse.Content.ReadAsByteArrayAsync();

                foreach (var responseHeader in serverResponse.Headers)
                {
                    context.Response.Headers.Add(responseHeader.Key, responseHeader.Value.ToArray());
                }

                context.Response.Headers.ContentType = "application/javascript";
                await context.Response.Body.WriteAsync(content);
                return;
            }
        }
        else if (context.WebSockets.IsWebSocketRequest)
        {
            var error = await httpForwarder.SendAsync(context, "ws://localhost:3000/",
                httpClient, requestConfig, transformer);
            // Check if the operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = context.GetForwarderErrorFeature();
                var exception = errorFeature.Exception;

                if (exception is not null)
                {
                    throw exception;
                }
            }
        }

        await next.Invoke();
    });
}
else
{
    app.UseStaticFiles();
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "game",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToAreaController("Index", "Home", "Game");

app.UseSpa(spa =>
{
    if (app.Environment.IsDevelopment())
    {
        spa.Options.SourcePath = "wwwroot";
    }
});

app.MapRazorPages();

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OGame.MVC.Data;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.AspNetCore.WebSockets;

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
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapReverseProxy();

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

        await next.Invoke();
    });
}
else
{
    app.UseStaticFiles();
}

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var client = new ClientWebSocket();
        var targetUri = new Uri("ws://localhost:3000" + context.Request.Path);

        await client.ConnectAsync(targetUri, context.RequestAborted);

        var serverSocket = await context.WebSockets.AcceptWebSocketAsync();

        var buffer = new byte[1024 * 4];
        var clientSegment = new ArraySegment<byte>(buffer);
        var serverSegment = new ArraySegment<byte>(buffer);

        var clientReceiveTask = client.ReceiveAsync(clientSegment, context.RequestAborted);
        var serverReceiveTask = serverSocket.ReceiveAsync(serverSegment, context.RequestAborted);

        while (client.State == WebSocketState.Open && serverSocket.State == WebSocketState.Open)
        {
            var completedTask = await Task.WhenAny(clientReceiveTask, serverReceiveTask);

            Console.WriteLine("completedTask: " + completedTask);

            if (completedTask == clientReceiveTask)
            {
                await serverSocket.SendAsync(new ArraySegment<byte>(buffer, 0, clientReceiveTask.Result.Count), clientReceiveTask.Result.MessageType, clientReceiveTask.Result.EndOfMessage, context.RequestAborted);
                clientReceiveTask = client.ReceiveAsync(clientSegment, context.RequestAborted);
            }
            else // serverReceiveTask
            {
                await client.SendAsync(new ArraySegment<byte>(buffer, 0, serverReceiveTask.Result.Count), serverReceiveTask.Result.MessageType, serverReceiveTask.Result.EndOfMessage, context.RequestAborted);
                serverReceiveTask = serverSocket.ReceiveAsync(serverSegment, context.RequestAborted);
            }
        }

        await serverSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", context.RequestAborted);
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", context.RequestAborted);
    }
    else
    {
        await next();
    }
});

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

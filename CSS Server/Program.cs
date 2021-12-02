using CSS_Server.JsonProvider;
using CSS_Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddSingleton<CameraJsonProvider>();
builder.Services.AddSingleton<CameraManager>();

//// Add services to the container.
////builder.Services.AddRazorPages();
//builder.Services.AddControllers()

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

//add websocket config
var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};

app.UseWebSockets(webSocketOptions);


//app.MapControllers();

app.UseRouting();

////app.UseAuthorization();

////app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
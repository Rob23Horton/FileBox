using FileBox.Client.Pages;
using FileBox.Client.Services;
using FileBox.Components;
using FileBox.Services;
using MySqlConnector;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

//Server Services
builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

//Client Services
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<FileBox.Client.Services.FolderService>();
builder.Services.AddScoped<FileBox.Client.Services.FileService>();
builder.Services.AddScoped<FileBox.Client.Services.TagService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FileBox.Client._Imports).Assembly);

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");


app.Run();

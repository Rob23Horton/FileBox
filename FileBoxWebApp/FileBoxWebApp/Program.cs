using FileBox.Interfaces;
using FileBox.Repositories;
using DatabaseConnector.Services;
using FileBoxWebApp.Client.Pages;
using FileBoxWebApp.Client.Services;
using FileBoxWebApp.Components;
using FileBoxWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

//Adds services that is used by the server
builder.Services.AddSingleton<IDatabaseConnector>(FileBox.FileBox.StartUp(null));
builder.Services.AddSingleton<IFileSaveService, FileSaveService>();

//Adds services that is used by client
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IDataTransferService, DataTransferService>();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

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

app.MapControllers();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(FileBoxWebApp.Client._Imports).Assembly);

app.Run();

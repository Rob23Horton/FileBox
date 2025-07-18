using DatabaseConnector.Models;
using DatabaseConnector.Services;
using FileBox.Interfaces;
using FileBox.Repositories;
using FileBoxWebApp.Client.Pages;
using FileBoxWebApp.Client.Services;
using FileBoxWebApp.Components;
using FileBoxWebApp.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

//Adds services that is used by the server
builder.Services.AddSingleton<IDatabaseConnector, DatabaseConnector.Services.DatabaseConnector>(x => FileBox.FileBox.StartUp(null));
builder.Services.AddSingleton<PathService>();
builder.Services.AddSingleton<IFileSaveService, FileSaveService>();
builder.Services.AddSingleton<FolderService>();
builder.Services.AddSingleton<FileService>();

//Adds services that is used by client
builder.Services.AddScoped<HttpClient>();
builder.Services.AddSingleton<PerpetualSettingService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IDataTransferService, DataTransferService>();


builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	//app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.MapControllers();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(FileBoxWebApp.Client._Imports).Assembly);

app.Run();

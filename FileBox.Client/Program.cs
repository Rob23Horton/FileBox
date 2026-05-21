using FileBox.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(x => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<FolderService>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<TagService>();

await builder.Build().RunAsync();

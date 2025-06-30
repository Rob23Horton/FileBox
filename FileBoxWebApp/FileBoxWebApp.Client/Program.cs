using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FileBoxWebApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(x => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IDataTransferService, DataTransferService>();

await builder.Build().RunAsync();

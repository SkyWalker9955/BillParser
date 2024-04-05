using BillParser.Client;
using BillParser.Client.Code;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddTransient<IBillParserService, BillParserService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync(); 

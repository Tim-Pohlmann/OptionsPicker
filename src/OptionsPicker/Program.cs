using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OptionsPicker;
using OptionsPicker.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register application services
builder.Services.AddSingleton<IOptionCollection, OptionCollection>();
builder.Services.AddSingleton<IStateManager, StateManager>();
builder.Services.AddSingleton<ISelectionService, SelectionService>();

await builder.Build().RunAsync();

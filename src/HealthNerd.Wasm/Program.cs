using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OfficeOpenXml;
using HealthNerd.Wasm.Services;

// Required for XML parsing of Apple Health export
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// EPPlus non-commercial license (same as CLI project)
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HealthNerd.Wasm.App>("#app");

builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();

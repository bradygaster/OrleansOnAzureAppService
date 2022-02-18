using Orleans;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>(EnvironmentVariables.AzureStorageConnectionString);
    siloBuilder
        .HostSiloInAzure(builder.Configuration)
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false);
});

builder.Services.AddServicesForSelfHostedDashboard();

var app = builder.Build();
app.UseOrleansDashboard();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
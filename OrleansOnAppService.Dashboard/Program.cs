using Orleans;
using Orleans.Hosting;
using OrleansOnAppService.Grains;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>(EnvironmentVariables.AzureStorageConnectionString);
    siloBuilder
        .HostSiloInAzure(builder.Configuration)
        .AddAzureTableGrainStorage(name: "visitorsStore", options => options.ConfigureTableServiceClient(storageConnectionString))
        .AddAzureTableGrainStorage(name: "activeVisitorsStore", options => options.ConfigureTableServiceClient(storageConnectionString))
        .ConfigureApplicationParts(applicationParts => applicationParts.AddApplicationPart(typeof(VisitorGrain).Assembly).WithReferences())
        .UseDashboard(dashboardOptions => dashboardOptions.HostSelf = false);
});

builder.Services.AddServicesForSelfHostedDashboard();

var app = builder.Build();
app.UseOrleansDashboard();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
using Orleans;
using Orleans.Hosting;
using OrleansOnAppService.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>(EnvironmentVariables.AzureStorageConnectionString);
    siloBuilder
        .HostSiloInAzure(builder.Configuration)
        .AddAzureTableGrainStorage(name: "visitorsStore", options => options.ConfigureTableServiceClient(storageConnectionString))
        .AddAzureTableGrainStorage(name: "activeVisitorsStore", options => options.ConfigureTableServiceClient(storageConnectionString));
});

var app = builder.Build();
app.UseRouting();

app.MapGet("/", async (IGrainFactory grainFactory) =>
{
    var grain = grainFactory.GetGrain<IActiveVisitorsGrain>(Guid.Empty);
    return await grain.GetVisitors();
});

app.Run();

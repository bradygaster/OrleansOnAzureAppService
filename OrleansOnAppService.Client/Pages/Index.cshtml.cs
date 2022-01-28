using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrleansOnAppService.Abstractions;

namespace OrleansOnAppService.Clients.FrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Orleans.IClusterClient _orleansClient;
        public Visitor[] Visitors { get; set; }

        public IndexModel(ILogger<IndexModel> logger, Orleans.IClusterClient orleansClient)
        {
            _logger = logger;
            _orleansClient = orleansClient;
        }

        public async Task OnGet()
        {
            var grain = _orleansClient.GetGrain<IActiveVisitorsGrain>(Guid.Empty);
            var activeVisitorList = await grain.GetVisitors();
            Visitors = activeVisitorList.ToArray();
        }
    }
}
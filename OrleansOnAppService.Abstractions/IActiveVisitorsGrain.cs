using Orleans;

namespace OrleansOnAppService.Abstractions
{
    public interface IActiveVisitorsGrain : IGrainWithGuidKey
    {
        Task<List<Visitor>> GetVisitors();
        Task AddVisitor(Visitor visitor);
    }
}

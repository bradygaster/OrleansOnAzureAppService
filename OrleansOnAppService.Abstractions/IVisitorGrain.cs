using Orleans;

namespace OrleansOnAppService.Abstractions
{
    public interface IVisitorGrain : IGrainWithStringKey
    {
        Task SetVisitor(Visitor visitor);
        Task<Visitor> GetVisitor(string key);
    }
}

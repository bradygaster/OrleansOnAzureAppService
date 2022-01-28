using Orleans;
using Orleans.Runtime;
using OrleansOnAppService.Abstractions;

namespace OrleansOnAppService.Grains
{
    public class VisitorGrain : Grain, IVisitorGrain
    {
        private readonly IPersistentState<Visitor> _visitor;
        private readonly IGrainFactory _grainFactory;
        private readonly IActiveVisitorsGrain _activeVisitorsGrain;

        public VisitorGrain([PersistentState("visitors", "visitorsStore")] IPersistentState<Visitor> visitor,
            IGrainFactory grainFactory)
        {
            _visitor = visitor;
            _grainFactory = grainFactory;
            _activeVisitorsGrain = _grainFactory.GetGrain<IActiveVisitorsGrain>(Guid.Empty);
        }

        public async Task<Visitor> GetVisitor(string key)
        {
            var activeVisitors = await _activeVisitorsGrain.GetVisitors();
            if (activeVisitors.Any(x => x.SessionKey == key))
            {
                return activeVisitors.First(x => x.SessionKey == key);
            }

            return null;
        }

        public async Task SetVisitor(Visitor visitor)
        {
            _visitor.State = visitor;
            await _visitor.WriteStateAsync();
            await _activeVisitorsGrain.AddVisitor(visitor);
        }
    }
}

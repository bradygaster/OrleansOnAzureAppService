using Orleans;
using Orleans.Runtime;
using OrleansOnAppService.Abstractions;

namespace OrleansOnAppService.Grains
{
    public class ActiveVisitorsGrain : Grain, IActiveVisitorsGrain
    {
        private readonly IPersistentState<List<Visitor>> _activeVisitors;

        public ActiveVisitorsGrain([PersistentState("activeVisitors", "activeVisitorsStore")] IPersistentState<List<Visitor>> activeVisitors)
        {
            _activeVisitors = activeVisitors;
        }

        public async Task AddVisitor(Visitor visitor)
        {
            if (_activeVisitors.State != null && !_activeVisitors.State.Any(x => x.SessionKey == visitor.SessionKey))
            {
                _activeVisitors.State.Add(visitor);
            }
            else
            {
                _activeVisitors.State.First(x => x.SessionKey == visitor.SessionKey).CurrentPage = visitor.CurrentPage;
                _activeVisitors.State.First(x => x.SessionKey == visitor.SessionKey).LastSeen = DateTime.Now;
            }

            _activeVisitors.State = _activeVisitors.State.OrderByDescending(x => x.LastSeen).ToList();
            await _activeVisitors.WriteStateAsync();
        }

        public Task<List<Visitor>> GetVisitors()
        {
            return Task.FromResult(_activeVisitors.State);
        }
    }
}

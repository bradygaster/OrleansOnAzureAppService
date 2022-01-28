namespace OrleansOnAppService.Abstractions
{
    public class Visitor
    {
        public string SessionKey { get; set; } = string.Empty;
        public string RemoteAddress { get; set; } = string.Empty;
        public string CurrentPage { get; set; } = string.Empty;
        public DateTime Arrived { get; set; } = DateTime.Now;
        public DateTime LastSeen { get; set; } = DateTime.Now;
    }
}

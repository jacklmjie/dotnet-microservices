namespace User.API.Infrastructure
{
    public class CapOptions
    {
        public string MySql { get; set; }
        public string RabbitMQ { get; set; }
        public string DiscoveryServerHostName { get; set; }
        public int DiscoveryServerPort { get; set; }
        public string CurrentNodeHostName { get; set; }
        public int CurrentNodePort { get; set; }
        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public string MatchPath { get; set; }
    }
}

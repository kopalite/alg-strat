using System;

namespace AlgorithmStrategies
{
    public class StrategySettings : IStrategySettings
    {
        public int ForId { get; set; }

        public int NextId { get; set; }

        public int Priority { get; set; }

        public TerminationType TerminationType { get; set; }
    }
    
}

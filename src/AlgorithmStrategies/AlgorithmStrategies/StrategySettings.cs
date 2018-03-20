using System;

namespace AlgorithmStrategies
{
    public interface IStrategySettings
    {
        int Type { get; }

        int NextId { get; }

        int Priority { get; }

        TerminationType TerminationType { get; }
    }

    public class StrategySettings : IStrategySettings
    {
        public int Type { get; set; }

        public int NextId { get; set; }

        public int Priority { get; set; }

        public TerminationType TerminationType { get; set; }

        public StrategySettings Clone()
        {
            return MemberwiseClone() as StrategySettings;
        }
    }

    public interface IConfigSettings : IStrategySettings
    {
        int ForId { get; }
    }

    public class ConfigSettings : StrategySettings
    {
        public int ForId { get; set; }
    }
}

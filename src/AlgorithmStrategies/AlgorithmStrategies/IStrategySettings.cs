using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmStrategies
{
    public interface IStrategySettings
    {
        int ForId { get; set; }

        int NextId { get; set; }

        int Priority { get; set; }

        TerminationType TerminationType { get; set; }
    }
}

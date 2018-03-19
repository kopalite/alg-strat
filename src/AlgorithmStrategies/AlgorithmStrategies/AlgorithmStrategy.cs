using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategy<TModel>
    {
        int Type { get; }

        string Name { get; }

        int Id { get; }

        int NextId { get; }

        int Priority { get; }

        bool IsTerminating { get; }

        bool IsCandidate(TModel model);

        Result Evaluate(TModel model);
    }

    public class AlgorithmStrategyBase<TModel> : IAlgorithmStrategy<TModel>
    {
        public virtual int Type { get; }

        public string Name { get; }

        public virtual int Id { get; }

        public virtual int NextId { get; }

        public virtual int Priority { get; }

        public virtual bool IsTerminating { get; }

        public virtual bool IsCandidate(TModel model)
        {
            return false;
        }

        public virtual Result Evaluate(TModel model)
        {
            return Result.Inconclusive;
        }
    }
}

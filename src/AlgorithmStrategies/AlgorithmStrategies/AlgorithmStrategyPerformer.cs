using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategyPerformer<TModel>
    {
        Result ByPriority(TModel model, int strategyType, int strategyId);

        Result ByAlgorithm(TModel model, int strategyType, int strategyId);
    }

    public class AlgorithmStrategyPerformer<TModel> : IAlgorithmStrategyPerformer<TModel>
    {
        private readonly IEnumerable<IAlgorithmStrategy<TModel>> _strategies;

        public AlgorithmStrategyPerformer(IEnumerable<IAlgorithmStrategy<TModel>> strategies)
        {
            _strategies = strategies;
        }

        public Result ByPriority(TModel model, int strategyType, int strategyId)
        {
            var result = Result.Inconclusive;
            var strategies = _strategies.Where(x => x.Type == strategyType).OrderByDescending(x => x.Priority).ToList();
            IAlgorithmStrategy<TModel> strategy = null;

            do
            {
                strategy = strategies.FirstOrDefault(x => x.Id == strategyId) ?? strategies.FirstOrDefault();
                if (strategy != null)
                {
                    if (strategy.IsCandidate(model))
                    {
                        result = strategy.Evaluate(model);
                    }
                    strategies.Remove(strategy);
                    strategyId = strategy.NextId;
                }
            }
            while (result != Result.Success && strategy != null && !strategy.IsTerminating);

            return result;
        }

        public Result ByAlgorithm(TModel model, int strategyType, int strategyId)
        {
            throw new NotImplementedException();
        }
    }
}

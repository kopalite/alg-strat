using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmStrategies
{
    public sealed class StrategyExecutor<TModel, TResult> : IStrategyExecutor<TModel, TResult>
    {
        private readonly IEnumerable<IAlgorithmStrategy<TModel, TResult>> _strategies;

        public StrategyExecutor(IEnumerable<IAlgorithmStrategy<TModel, TResult>> strategies)
        {
            _strategies = strategies;
        }

        public StrategyResult<TResult> Execute(TModel model)
        {
            var strategies = _strategies.Where(x => x.IsActive).ToList();
            return ByPriority(model, strategies, 0);
        }

        public StrategyResult<TResult> Execute(TModel model, int strategyId)
        {
            var strategies = _strategies.Where(x => x.IsActive).ToList();
            return ByPriority(model, strategies, strategyId);
        }

        private StrategyResult<TResult> ByPriority(TModel model, List<IAlgorithmStrategy<TModel, TResult>> strategies, int strategyId)
        {
            strategies = strategies.OrderBy(x => x.Priority).ToList();

            var result = StrategyResult<TResult>.Inconclusive();
            IAlgorithmStrategy<TModel, TResult> strategy = null;

            do
            {
                strategy = strategies.FirstOrDefault(x => x.Id == strategyId) ?? strategies.FirstOrDefault();
                if (strategy != null)
                { 
                    if (strategy.IsCandidate(model))
                    {
                        result.Sync(strategy.Execute(model));
                    }

                    result.Log(strategy.Name);
                    strategies.RemoveAll(s => s.Priority <= strategy.Priority);
                    strategyId = strategy.NextId;
                }
            }
            while (strategy != null && !strategy.IsTermination(result));

            return result;
        }
    }
}

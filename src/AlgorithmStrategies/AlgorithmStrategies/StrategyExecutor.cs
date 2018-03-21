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

        public AggregateResult<TResult> Execute(TModel model)
        {
            var strategies = _strategies.Where(x => x.IsActive).ToList();
            return Execute(model, strategies, 0);
        }

        public AggregateResult<TResult> Execute(TModel model, int strategyId)
        {
            var strategies = _strategies.Where(x => x.IsActive).ToList();
            return Execute(model, strategies, strategyId);
        }

        private AggregateResult<TResult> Execute(TModel model, List<IAlgorithmStrategy<TModel, TResult>> strategies, int strategyId)
        {
            strategies = strategies.OrderBy(x => x.Priority).ToList();

            var finalResult = new AggregateResult<TResult>(ResultType.Inconclusive);
            IAlgorithmStrategy<TModel, TResult> strategy = null;

            do
            {
                strategy = strategies.FirstOrDefault(x => x.Id == strategyId) ?? strategies.FirstOrDefault();
                if (strategy != null)
                {
                    var currentResult = StrategyResult<TResult>.Inconclusive();
                    if (strategy.IsCandidate(model))
                    {
                        currentResult = strategy.Execute(model);
                    }
                    currentResult.Strategy = strategy.Name;
                    finalResult.Sync(currentResult);
                    finalResult.Log(currentResult);

                    strategies.RemoveAll(s => s.Priority <= strategy.Priority);
                    strategyId = strategy.NextId;
                }
            }
            while (strategy != null && !strategy.IsTermination(finalResult));

            return finalResult;
        }
    }
}

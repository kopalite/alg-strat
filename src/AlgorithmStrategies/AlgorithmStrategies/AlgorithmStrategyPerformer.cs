﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategyPerformer<TModel, TResult>
    {
        StrategyResult<TResult> ByPriority(TModel model, int strategyType);

        StrategyResult<TResult> ByPriority(TModel model, int strategyType, int strategyId);
    }

    public class AlgorithmStrategyPerformer<TModel, TResult> : IAlgorithmStrategyPerformer<TModel, TResult>
    {
        private readonly IEnumerable<IAlgorithmStrategy<TModel, TResult>> _strategies;

        public AlgorithmStrategyPerformer(IEnumerable<IAlgorithmStrategy<TModel, TResult>> strategies)
        {
            _strategies = strategies;
        }

        public StrategyResult<TResult> ByPriority(TModel model, int strategyType)
        {
            var strategies = _strategies.Where(x => x.Type == strategyType).ToList();
            return ByPriority(model, strategies, 0);
        }

        public StrategyResult<TResult> ByPriority(TModel model, int strategyType, int strategyId)
        {
            var strategies = _strategies.Where(x => x.Type == strategyType).ToList();
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

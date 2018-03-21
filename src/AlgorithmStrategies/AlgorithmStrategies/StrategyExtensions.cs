using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmStrategies
{
    public static class StrategyExtensions
    {
        public static IEnumerable<IAlgorithmStrategy<TModel, TResult>> Create<TModel, TResult>(this IEnumerable<IAlgorithmStrategy<TModel, TResult>> strategies,
                                                                 int id, string name,
                                                                 Expression<Func<TModel, bool>> isCandidate,
                                                                 Expression<Func<TModel, StrategyResult<TResult>>> execute)
        {
            var retVal = new List<IAlgorithmStrategy<TModel, TResult>>();
            var newStrategy = RuntimeStrategy<TModel, TResult>.Create(id, name, isCandidate, execute);
            retVal.AddRange(newStrategy);
            retVal.AddRange(strategies);
            return retVal;
        }

        public static IEnumerable<IAlgorithmStrategy<TModel, TResult>> Set<TModel, TResult>(this IEnumerable<IAlgorithmStrategy<TModel, TResult>> strategies,
                                                                            IEnumerable<IStrategySettings> settings)
        {
            foreach (var strategy in strategies)
            {
                var setting = settings.FirstOrDefault(s => s.ForId == strategy.Id);
                if (setting != null)
                {
                    strategy.Set(setting);
                }
            }
            return strategies;
        }

        public static IEnumerable<IAlgorithmStrategy<TModel, TResult>> Activate<TModel, TResult>(this IEnumerable<IAlgorithmStrategy<TModel, TResult>> strategies)
        {
            foreach (var strategy in strategies)
            {
                strategy.IsActive = true;
            }
            return strategies;
        }
    }
}

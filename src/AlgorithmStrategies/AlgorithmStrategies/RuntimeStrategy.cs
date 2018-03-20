using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AlgorithmStrategies
{
    public class RuntimeStrategy<TModel, TResult> : StrategyBase<TModel, TResult>
    {
        private readonly Func<TModel, bool> _isCandidate;
        private readonly Func<TModel, StrategyResult<TResult>> _execute;

        private readonly int _id;
        public override int Id { get { return _id; } }

        private readonly string _name;
        public override string Name { get { return _name; } }

        private RuntimeStrategy(int id, string name, 
                                Expression<Func<TModel, bool>> isCandidate,
                                Expression<Func<TModel, StrategyResult<TResult>>> execute)
        {
            _id = id;
            _name = name;
            _isCandidate = isCandidate.Compile();
            _execute = execute.Compile();
        }

        public override bool IsCandidate(TModel model)
        {
            return _isCandidate(model);
        }

        public override StrategyResult<TResult> Execute(TModel model)
        {
            return _execute(model);
        }

        public static IEnumerable<IAlgorithmStrategy<TModel, TResult>> Create(int id, string name,
                                                                             Expression<Func<TModel, bool>> isCandidate,
                                                                             Expression<Func<TModel, StrategyResult<TResult>>> execute)
        {
            return new[] { new RuntimeStrategy<TModel, TResult>(id, name, isCandidate, execute) };
        }
    }

    public static class RuntimeStrategyExtensions
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
                                                                            IEnumerable<ConfigSettings> settings)
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
    }
}

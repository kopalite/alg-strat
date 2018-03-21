using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AlgorithmStrategies
{
    public class RuntimeStrategy<TModel, TResult> : AlgorithmStrategy<TModel, TResult>
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
}

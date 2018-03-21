using System.Collections;
using System.Linq;

namespace AlgorithmStrategies
{
    public class AggregateResult<TResult> : StrategyResult<TResult>
    {
        private Stack _resultsLog;

        public AggregateResult(ResultType resultType) : base(resultType)
        {
            _resultsLog = new Stack();
        }

        public string StrategyHistory
        {
            get
            {
                var results = _resultsLog.ToArray().OfType<StrategyResult<TResult>>();
                var traces = results.Select(x => x.Strategy).Reverse();
                return string.Join(";", traces);
            }
        }

        public string OutcomeHistory
        {
            get
            {
                var results = _resultsLog.ToArray().OfType<StrategyResult<TResult>>();
                var traces = results.Select(x => string.Format("{0}:{1}", x.Strategy, x.ResultType)).Reverse();
                return string.Join(";", traces);
            }
        }

        public string VerboseHistory
        {
            get
            {
                var results = _resultsLog.ToArray().OfType<StrategyResult<TResult>>();
                var traces = results.Select(x => string.Format("{0}:{1}:{2}", x.Strategy, x.ResultType, x.Result)).Reverse();
                return string.Join(";", traces);
            }
        }

        internal void Log(StrategyResult<TResult> result)
        {
            if (result != null)
            {
                _resultsLog.Push(Clone(result));
            }
        }
    }
}

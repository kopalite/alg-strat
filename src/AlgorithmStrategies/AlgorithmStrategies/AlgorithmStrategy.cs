namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategy<TModel, TResult>
    {
        int Type { get; }

        string Name { get; }

        int Id { get; }

        int NextId { get; }

        int Priority { get; }

        TerminationType TerminationType { get; }

        bool IsCandidate(TModel model);
        
        StrategyResult<TResult> Execute(TModel model);

        bool IsTermination(StrategyResult<TResult> result);
    }

    public class AlgorithmStrategyBase<TModel, TResult> : IAlgorithmStrategy<TModel, TResult>
    {
        public virtual int Type { get; }

        public virtual string Name { get; }

        public virtual int Id { get; }

        public virtual int NextId { get; }

        public virtual int Priority { get; }

        public virtual TerminationType TerminationType { get; }

        public virtual bool IsCandidate(TModel model)
        {
            return false;
        }

        public virtual StrategyResult<TResult> Execute(TModel model)
        {
            return StrategyResult<TResult>.Inconclusive();
        }

        public bool IsTermination(StrategyResult<TResult> result)
        {
            return this.TerminationType == TerminationType.TerminateAlways ||
                   this.TerminationType == TerminationType.TerminateOnFailure && result.IsFailure ||
                   this.TerminationType == TerminationType.TerminateOnSuccess && result.IsSuccess;
        }
    }
}

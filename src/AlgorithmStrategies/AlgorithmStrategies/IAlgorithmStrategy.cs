namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategy<TModel, TResult>
    {
        int Id { get; }

        string Name { get; }

        bool IsActive { get; set; }

        int NextId { get; }

        int Priority { get; }

        TerminationType TerminationType { get; }

        void Set(IStrategySettings settings);

        bool IsCandidate(TModel model);

        StrategyResult<TResult> Execute(TModel model);

        bool IsTermination(StrategyResult<TResult> result);
    }
}

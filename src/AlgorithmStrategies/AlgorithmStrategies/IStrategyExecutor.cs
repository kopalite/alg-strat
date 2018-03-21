namespace AlgorithmStrategies
{
    public interface IStrategyExecutor<TModel, TResult>
    {
        AggregateResult<TResult> Execute(TModel model);

        AggregateResult<TResult> Execute(TModel model, int strategyId);
    }
}

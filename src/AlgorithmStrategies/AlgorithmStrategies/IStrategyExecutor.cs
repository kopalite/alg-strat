namespace AlgorithmStrategies
{
    public interface IStrategyExecutor<TModel, TResult>
    {
        StrategyResult<TResult> Execute(TModel model);

        StrategyResult<TResult> Execute(TModel model, int strategyId);
    }
}

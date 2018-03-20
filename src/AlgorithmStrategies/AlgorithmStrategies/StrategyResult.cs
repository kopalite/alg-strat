namespace AlgorithmStrategies
{
    public class StrategyResult<TResult>
    {
        public ResultType ResultType { get; private set; }

        public TResult Result { get; private set; }

        public string History { get; private set; }

        private StrategyResult(ResultType resultType) : this(resultType, default(TResult))
        {
            
        }

        private StrategyResult(ResultType resultType, TResult result)
        {
            ResultType = resultType;
            Result = result;
        }

        public void Sync(StrategyResult<TResult> result)
        {
            ResultType = result.ResultType;
            Result = result.Result;
        }

        public void Log(string strategyName)
        {
            History = ((History ?? "") + ";" + strategyName).Trim(';'); 
        }

        public bool IsSuccess { get { return ResultType == ResultType.Success; } }

        public bool IsFailure { get { return ResultType == ResultType.Failure; } }

        public bool IsInconclusive { get { return ResultType == ResultType.Inconclusive; } }

        public static StrategyResult<TResult> Inconclusive(TResult result = default(TResult))
        {
            return new StrategyResult<TResult>(ResultType.Inconclusive, result); 
        }

        public static StrategyResult<TResult> Success(TResult result = default(TResult))
        {
            return new StrategyResult<TResult>(ResultType.Success, result);
        }

        public static StrategyResult<TResult> Failure(TResult result = default(TResult))
        {
            return new StrategyResult<TResult>(ResultType.Failure, result);
        }

        
    }

}

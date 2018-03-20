namespace AlgorithmStrategies
{
    public class StrategyResult<TResult>
    {
        public TResult Result { get; }

        public ResultType ResultType { get; }

        public string StrategyName { get; internal set; }

        private StrategyResult(ResultType resultType) : this(resultType, default(TResult))
        {
            
        }

        private StrategyResult(ResultType resultType, TResult result)
        {
            ResultType = resultType;
            Result = result;
        }

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

        public override bool Equals(object obj)
        {
            var result = obj as StrategyResult<TResult>;
            if (result == null)
            {
                return false;
            } 
            return result.ResultType == ResultType;
        }

        public override int GetHashCode()
        {
            return -1373938868 + ResultType.GetHashCode();
        }
    }

}

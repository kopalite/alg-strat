namespace AlgorithmStrategies
{
    public class StrategyResult<TResult>
    {
        public TResult Result { get; }

        public ResultType ResultType { get; }

        private StrategyResult(ResultType resultType) : this(resultType, default(TResult))
        {
            
        }

        private StrategyResult(ResultType resultType, TResult result)
        {
            ResultType = resultType;
            Result = result;
        }

        public static StrategyResult<TResult> Inconclusive()
        {
            return new StrategyResult<TResult>(ResultType.Inconclusive); 
        }

        public static StrategyResult<TResult> Success()
        {
            return new StrategyResult<TResult>(ResultType.Success);
        }

        public static StrategyResult<TResult> Failure()
        {
            return new StrategyResult<TResult>(ResultType.Failure);
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

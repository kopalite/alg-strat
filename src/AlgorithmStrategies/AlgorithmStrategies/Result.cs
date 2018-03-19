namespace AlgorithmStrategies
{
    public class Result
    {
        public ResultType ResultType { get; }

        public Result(ResultType resultType)
        {
            ResultType = resultType;
        }

        public static Result Inconclusive
        {
            get { return new Result(ResultType.Inconclusive); }
        }

        public static Result Success
        {
            get { return new Result(ResultType.Success); }
        }

        public static Result Failure
        {
            get { return new Result(ResultType.Failure); }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Result && ((Result)obj).ResultType == ResultType;
        }

        public override int GetHashCode()
        {
            return -1373938868 + ResultType.GetHashCode();
        }
    }

}

namespace LetstalkMockAPI.Models
{
    public class FailureResult : Result
    {
        public FailureResult()
        {
            Status = 0;
            Message = "failure";
        }
    }
}
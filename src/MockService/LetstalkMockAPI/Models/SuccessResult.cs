namespace LetstalkMockAPI.Models
{
    public class SuccessResult : Result
    {
        public SuccessResult()
        {
            Status = 1;
            Message = "success";
        }
    }
}
namespace Identity_Infrastructure.Models.ResponseModel
{
    public class BaseResponse
    {
        public int Status { get; set; }

        public string Message { get; set; } = "NA"!;
    }
}

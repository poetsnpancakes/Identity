namespace Infrastructure.Models.ResponseModel
{
    public class AppResponse<T>:BaseResponse
    {
        public required T Data { get; set; }
    }
}

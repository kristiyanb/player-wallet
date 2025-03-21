namespace PlayerWallet.Services.Models.Results.Base
{
    public interface IResult<T>
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }
    }
}
namespace Application.Core;

public sealed class ServiceResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Error { get; set; } = "Something went wrong..";

    public static ServiceResponse<T> SuccessResponse(T Data) => 
        new() { Success = true, Data = Data, Error = string.Empty };
}

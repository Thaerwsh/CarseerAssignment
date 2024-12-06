namespace Application.Service.Base.Models;

public class ApiResponseWrapper<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
}
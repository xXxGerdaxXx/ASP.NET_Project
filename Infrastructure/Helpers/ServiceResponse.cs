namespace Infrastructure.Helpers;
// this helper allows me to create a standard response for my API endpoints
public class ServiceResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

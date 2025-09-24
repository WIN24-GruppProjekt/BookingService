namespace Application.Models;

public class BookingResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class BookingResult<T> : BookingResult
{
    public T? Result { get; set; }
}
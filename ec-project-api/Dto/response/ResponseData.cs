public class ResponseData<T>
{
    public int Status { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ResponseData() { }

    public ResponseData(int status, bool isSuccess, string? message, T? data)
    {
        Status = status;
        IsSuccess = isSuccess; 
        Message = message;
        Data = data;
    }

    public static ResponseData<T> Success(int status, T data, string? message)
    {
        return new ResponseData<T>(status, true, message, data);
    }

    public static ResponseData<T> Success(int status, T data)
    {
        return new ResponseData<T>(status, true, null, data);
    }

    public static ResponseData<T> Error(int status, string message)
    {
        return new ResponseData<T>(status, false, message, default);
    }
}

namespace BackendApi.Core.General
{
    public class ResponseData<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}

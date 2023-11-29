namespace Trilha.DotNet.Shared.Contracts;

public class ApiResponse<T>
{
    public bool Status { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool LastPage { get; set; }
    public T Data { get; set; } = default!;
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}

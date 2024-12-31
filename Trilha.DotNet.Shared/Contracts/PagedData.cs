namespace Trilha.DotNet.Shared.Contracts;

public class PagedData<T>
{
    public List<T> Data { get; set; }
    public int CurrentPage { get; set; }
    public bool LastPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }

    public PagedData(List<T> data, int currentPage, int totalItems, int pageSize)
    {
        Data = data;
        CurrentPage = currentPage;
        TotalItems = totalItems;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        LastPage = CurrentPage == TotalPages;
    }
}

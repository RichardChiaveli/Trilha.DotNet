namespace Trilha.DotNet.Shared.Extensions;

public static class PaginationExtensions
{
    private static int GetTotalPages(this int totalItems, int pageSize)
    {
        var totalPages = totalItems / pageSize;
        if (totalItems % pageSize != 0)
            totalPages++;

        return totalPages;
    }

    public static IEnumerable<T> Paginate<T>(
        this IQueryable<T> query
        , int pageIndex
        , int pageSize
        , out int totalPages
        , out int totalItems)
    {
        totalItems = query.Count();
        totalPages = totalItems.GetTotalPages(pageSize);

        return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }

    public static IEnumerable<T> Paginate<T>(
        this IList<T> list
        , int pageIndex
        , int pageSize
        , out int totalPages
        , out int totalItems)
    {
        totalItems = list.Count;
        totalPages = totalItems.GetTotalPages(pageSize);

        return list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }

    public static IEnumerable<T> Paginate<T>(
        int pageIndex
        , int pageSize
        , out int totalPages
        , out int totalItems
        , params T[] array)
    {
        totalItems = array.Length;
        totalPages = totalItems.GetTotalPages(pageSize);

        return array.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }
}

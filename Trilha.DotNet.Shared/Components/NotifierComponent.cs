namespace Trilha.DotNet.Shared.Components;

public class NotifierComponent
{
    public List<string> Messages { get; set; } = new();

    private object ShowMessages()
    {
        var result = new
        {
            Status = false,
            Errors = Messages
        };

        Messages.Clear();

        return result;
    }

    public object Result<T>(T? data = default)
    {
        if (Messages.Any())
            return ShowMessages();

        dynamic result = new ExpandoObject();
        result.Status = true;

        if (data != null)
            result.Data = data;

        return result;
    }

    public object Result<T>(IEnumerable<T> data, int pageIndex, int pageSize, int totalPages, int totalItems)
    {
        if (Messages.Any())
            return ShowMessages();

        return new
        {
            Status = true,
            PageIndex = pageIndex,
            PageSize = pageSize,
            LastPage = pageIndex == pageSize,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Data = data
        };
    }
}
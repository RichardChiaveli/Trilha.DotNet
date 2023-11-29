namespace Trilha.DotNet.Shared.Components;

public class NotifierComponent
{
    public List<string> Messages { get; set; } = new();

    public ApiResponse<T> Result<T>(T data)
    {
        if (Messages.Count > 0)
            return new ApiResponse<T>
            {
                Errors = Messages,
                Status = Messages.Count > 0
            };

        var result = new ApiResponse<T>
        {
            Status = true
        };

        if (data != null)
            result.Data = data;

        return result;
    }

    public ApiResponse<List<T>> Result<T>(List<T> data, int pageIndex, int pageSize, int totalPages, int totalItems)
    {
        return Messages.Count > 0
            ? new ApiResponse<List<T>>
            {
                Errors = Messages,
                Status = Messages.Count > 0
            }
            : new ApiResponse<List<T>>
            {
                Status = true,
                LastPage = pageIndex == pageSize,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Data = data
            };
    }
}
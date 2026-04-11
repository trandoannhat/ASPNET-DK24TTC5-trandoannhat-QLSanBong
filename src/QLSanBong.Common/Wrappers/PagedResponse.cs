namespace QLSanBong.Common.Wrappers;

public class PagedResponse<T> : ApiResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }

    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public PagedResponse(T data, int pageNumber, int pageSize, int totalRecords, string action = "List")
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.TotalRecords = totalRecords;
        this.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        this.Success = true;
        this.Message = "Thành công";
        this.Description = action;
        this.Data = data;
    }
}
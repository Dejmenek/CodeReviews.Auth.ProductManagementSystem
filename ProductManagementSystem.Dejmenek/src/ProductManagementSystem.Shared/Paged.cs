namespace ProductManagementSystem.Shared;
public class Paged<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public PageSize PageSize { get; set; } = PageSize.Five;
    public int TotalCount { get; set; }
}

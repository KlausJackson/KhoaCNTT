public class PagedResult<T>
{
    public int Total { get; set; }
    public List<T> Items { get; set; } = new();
    
}
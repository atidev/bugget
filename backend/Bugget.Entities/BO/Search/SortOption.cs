namespace Bugget.Entities.BO.Search;

public class SortOption
{
    public string Field { get; set; } = "created";
    public bool IsDescending { get; set; } = true;

    public static SortOption Parse(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return new SortOption();

        var parts = sort.Split('_');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid sort format");

        return new SortOption
        {
            Field = parts[0],
            IsDescending = parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
        };
    }
}
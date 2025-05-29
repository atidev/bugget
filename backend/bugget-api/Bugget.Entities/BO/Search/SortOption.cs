namespace Bugget.Entities.BO.Search;

public class SortOption
{
    private static readonly string[] AllowedFields = ["created", "updated", "rank"];
    public string Field { get; private init; } = "created";
    public bool IsDescending { get; private init; } = true;

    public static SortOption Parse(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return new SortOption();

        var parts = sort.Split('_');
        if (parts.Length != 2 || (!string.IsNullOrEmpty(parts[0]) && !AllowedFields.Contains(parts[0])))
            throw new ArgumentException("Invalid sort format");

        return new SortOption
        {
            Field = parts[0],
            IsDescending = parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
        };
    }
}
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
        if (parts.Length != 2)
            return new SortOption();

        var field = parts[0];
        var direction = parts[1];

        if (string.IsNullOrWhiteSpace(field) || !AllowedFields.Contains(field))
            return new SortOption();

        return new SortOption
        {
            Field = field,
            IsDescending = direction.Equals("desc", StringComparison.OrdinalIgnoreCase)
        };
    }
}
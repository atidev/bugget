namespace Bugget.Entities.DbModels.Bug
{
    public sealed class BugPatchResultDbModel
    {
        public required int Id { get; init; }
        public required string Receive { get; init; }
        public required string Expect { get; init; }
        public required DateTimeOffset UpdatedAt { get; init; }
        public required int Status { get; init; }
    }
}
namespace Bugget.Entities.BO.ReportBo;

public sealed class ReportUpdate
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? ResponsibleUserId { get; set; }
    public required string UpdaterUserId { get; set; }

    public string[] ParticipantsUserIds
    {
        get
        {
            if (string.IsNullOrEmpty(ResponsibleUserId))
                return new string[] { UpdaterUserId };
            return (new string[] { UpdaterUserId, ResponsibleUserId }).Distinct().ToArray();
        }
    }
}
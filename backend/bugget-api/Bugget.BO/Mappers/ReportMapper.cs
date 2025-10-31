using Bugget.Entities.BO;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.SocketViews;

namespace Bugget.BO.Mappers;

public static class ReportMapper
{
    public static SearchReports ToSearchReports(
        string? query,
        int[]? reportStatuses,
        string? userId,
        string? teamId,
        string? organizationId,
        string? sort,
        uint skip,
        uint take,
        IReadOnlyDictionary<string, IReadOnlyCollection<User>> usersByTeam)
    {
        List<string> resultUserIds = [];
        if (!string.IsNullOrEmpty(teamId))
        {
            if (usersByTeam.TryGetValue(teamId, out var team))
            {
                resultUserIds.AddRange(team.Select(e => e.Id));
            }
        }

        if (!string.IsNullOrEmpty(userId))
        {
            resultUserIds.Add(userId);
        }

        return new SearchReports
        {
            Query = string.IsNullOrEmpty(query) ? null : query,
            ReportStatuses = reportStatuses?.Length > 0 ? reportStatuses : null,
            UserIds = resultUserIds.Count > 0 ? resultUserIds.ToArray() : null,
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort),
            OrganizationId = organizationId
        };
    }

    public static PatchReportSocketView ToSocketView(this ReportPatchDto patchDto, ReportPatchResultDbModel result)
    {
        return new PatchReportSocketView
        {
            Title = patchDto.Title,
            Status = patchDto.Status,
            ResponsibleUserId = patchDto.ResponsibleUserId,
            PastResponsibleUserId = patchDto.ResponsibleUserId == null ? null : result.PastResponsibleUserId,
            UpdatedAt = result.UpdatedAt
        };
    }
}
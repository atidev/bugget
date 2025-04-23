using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.SocketViews;

namespace Bugget.DA.WebSockets;

public interface IReportPageHubClient
{
    Task SendReportPatchAsync(int reportId, PatchReportSocketView view, string? signalRConnectionId);
    Task SendNewReportParticipantAsync(int reportId, string newParticipant);
    Task SendBugCreateAsync(int reportId, BugSummaryDbModel summaryDbModel, string? signalRConnectionId);
    Task SendBugPatchAsync(int reportId, int bugId, BugPatchDto patchDto, string? signalRConnectionId);
}
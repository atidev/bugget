using Bugget.DA.WebSockets;
using Bugget.Entities.Authentication;
using Bugget.Entities.DbModels.Comment;

public class CommentEventsService(
        IReportPageHubClient reportPageHubClient
            )
    {
        public async Task HandleCommentCreateEventAsync(int reportId, UserIdentity user, CommentSummaryDbModel commentSummaryDbModel)
        {
            await Task.WhenAll(
                reportPageHubClient.SendCommentCreateAsync(reportId, commentSummaryDbModel, user.SignalRConnectionId)
        );
        }

        public async Task HandleCommentDeleteEventAsync(int reportId, UserIdentity user, int bugId, int commentId)
        {
            await Task.WhenAll(
                reportPageHubClient.SendCommentDeleteAsync(reportId, bugId, commentId, user.SignalRConnectionId)
            );
        }

        public async Task HandleCommentUpdateEventAsync(int reportId, UserIdentity user, CommentSummaryDbModel commentSummaryDbModel)
        {
            await Task.WhenAll(
                reportPageHubClient.SendCommentUpdateAsync(reportId, commentSummaryDbModel, user.SignalRConnectionId)
            );
        }
    }


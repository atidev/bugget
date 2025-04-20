using Dapper;

namespace Bugget.DA.Postgres
{
    public class ParticipantsDbClient : PostgresClient
    {
        public async Task<string[]?> AddParticipantIfNotExistAsync(int reportId, string userId)
        {
            await using var connection = await DataSource.OpenConnectionAsync();

            var participants = await connection.ExecuteScalarAsync<string[]?>(
                "SELECT public.add_participant_if_not_exist(@report_id, @user_id);",
                new { report_id = reportId, user_id = userId }
            );

            return participants;
        }
    }
}
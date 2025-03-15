using System.Text.Json;
using System.Text.Json.Serialization;
using Bugget.Entities.Config;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Bugget.DA.Postgres;

public class AttachmentDbClient: PostgresClient
{
    public async Task<AttachmentDbModel?> CreateAttachment(AttachmentDbModel attachmentDbModel)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_attachment(@BugId, @Path, @AttachType, @CreatedAt)",
            new
            {
                attachmentDbModel.BugId,
                attachmentDbModel.Path,
                attachmentDbModel.AttachType,
                attachmentDbModel.CreatedAt
            }
        );

        return jsonResult != null
            ? Deserialize<AttachmentDbModel>(jsonResult)
            : null;
    }

    public async Task<AttachmentDbModel?> GetAttachment(int attachmentId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_attachment(@attachmentId)",
            new
            {
                attachmentId
            }
        );
        
        return jsonResult != null
            ? Deserialize<AttachmentDbModel>(jsonResult)
            : null;
    }

    public async Task DeleteAttachment(int attachmentId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        
        await connection.ExecuteScalarAsync<string>(
            "SELECT public.delete_attachment(@attachmentId)",
            new
            {
                attachmentId
            }
        );
    }

    public Task<bool> FilePathExist(string relativePath)
    {
        // todo: check in db
        return Task.FromResult(false);
    }
    
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
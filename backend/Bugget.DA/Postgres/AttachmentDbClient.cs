using Bugget.Entities.DbModels.Attachment;
using Dapper;

namespace Bugget.DA.Postgres;

public class AttachmentDbClient: PostgresClient
{
    public async Task<AttachmentDbModel?> GetBugAttachmentAsync(string? organizationId, int reportId, int bugId, int attachmentId){
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<AttachmentDbModel>(
            "SELECT * FROM public.get_bug_attachment(@organizationId, @reportId, @bugId, @attachmentId)",
            new
            {
                organizationId,
                reportId,
                bugId,
                attachmentId
            }
        );
    }

    public async Task<AttachmentDbModel?> GetCommentAttachmentAsync(string? organizationId, int reportId, int bugId, int commentId, int attachmentId){
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<AttachmentDbModel>(
            "SELECT * FROM public.get_comment_attachment(@organizationId, @reportId, @bugId, @commentId, @attachmentId)",
            new
            {
                organizationId,
                reportId,
                bugId,
                commentId,
                attachmentId
            }
        );
    }

    public async Task<int> GetBugAttachmentsCountAsync(string? organizationId, int reportId, int bugId, int attachType)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var result = await connection.ExecuteScalarAsync<int>(
            "SELECT public.get_bug_attachments_count(@organizationId, @reportId, @bugId, @attachType)",
            new
            {
                organizationId,
                reportId,
                bugId,
                attachType
            }
        );

        return result;
    }

    public async Task<int> GetCommentAttachmentsCountAsync(string? organizationId, int reportId, int bugId, int commentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var result = await connection.ExecuteScalarAsync<int>(
            "SELECT public.get_comment_attachments_count(@organizationId, @reportId, @bugId, @commentId)",
            new
            {
                organizationId,
                reportId,
                bugId,
                commentId
            }
        );

        return result;
    }

    public async Task<AttachmentDbModel> CreateAttachment(AttachmentCreateDbModel attachmentCreateDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<AttachmentDbModel>(
            "SELECT * FROM public.create_attachment_internal(@entity_id, @attach_type, @storage_key, @storage_kind, @creator_user_id, @length_bytes, @file_name, @mime_type, @has_preview, @is_gzip_compressed)",
            new
            {
                entity_id = attachmentCreateDbModel.EntityId,
                attach_type = attachmentCreateDbModel.AttachType,
                storage_key = attachmentCreateDbModel.StorageKey,
                storage_kind = attachmentCreateDbModel.StorageKind,
                creator_user_id = attachmentCreateDbModel.CreatorUserId,
                length_bytes = attachmentCreateDbModel.LengthBytes,
                file_name = attachmentCreateDbModel.FileName,
                mime_type = attachmentCreateDbModel.MimeType,
                has_preview = attachmentCreateDbModel.HasPreview,
                is_gzip_compressed = attachmentCreateDbModel.IsGzipCompressed
            }
        );
    }

    public async Task<AttachmentDbModel?> DeleteBugAttachmentAsync(string? organizationId, int reportId, int bugId, int attachmentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<AttachmentDbModel>(
            "SELECT * FROM public.delete_bug_attachment(@organizationId, @reportId, @bugId, @attachmentId)",
            new
            {
                organizationId,
                reportId,
                bugId,
                attachmentId
            }
        );
    }

    public async Task<AttachmentDbModel?> DeleteCommentAttachmentAsync(string? organizationId, int reportId, int bugId, int commentId, int attachmentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<AttachmentDbModel>(
            "SELECT * FROM public.delete_comment_attachment(@organizationId, @reportId, @bugId, @commentId, @attachmentId)",
            new
            {
                organizationId,
                reportId,
                bugId,
                commentId,
                attachmentId
            }
        );
    }
}

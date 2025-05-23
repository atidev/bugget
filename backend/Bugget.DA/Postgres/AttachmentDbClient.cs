using Bugget.Entities.DbModels.Attachment;
using Dapper;

namespace Bugget.DA.Postgres;

public class AttachmentDbClient : PostgresClient
{
    public async Task<AttachmentDbModel[]> DeleteCommentAttachmentsAsync(int commentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return (await connection.QueryAsync<AttachmentDbModel>(
            "SELECT * FROM public.delete_comment_attachments_internal(@commentId)",
            new
            {
                commentId
            }
        )).ToArray();
    }

    public async Task<AttachmentDbModel> UpdateAttachmentAsync(UpdateAttachmentDbModel updateAttachmentDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<AttachmentDbModel>(
            "SELECT * FROM public.update_attachment_internal(@id, @storage_key, @storage_kind, @length_bytes, @file_name, @mime_type, @has_preview, @is_gzip_compressed)",
            new
            {
                id = updateAttachmentDbModel.Id,
                storage_key = updateAttachmentDbModel.StorageKey,
                storage_kind = updateAttachmentDbModel.StorageKind,
                length_bytes = updateAttachmentDbModel.LengthBytes,
                file_name = updateAttachmentDbModel.FileName,
                mime_type = updateAttachmentDbModel.MimeType,
                has_preview = updateAttachmentDbModel.HasPreview,
                is_gzip_compressed = updateAttachmentDbModel.IsGzipCompressed
            }
        );
    }

    public async Task<AttachmentDbModel?> GetBugAttachmentAsync(string? organizationId, int reportId, int bugId, int attachmentId)
    {
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

    public async Task<AttachmentDbModel?> GetCommentAttachmentAsync(string? organizationId, int reportId, int bugId, int commentId, int attachmentId)
    {
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

    public async Task<int> GetCommentAttachmentsCountAsync(string userId, string? organizationId, int reportId, int bugId, int commentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var result = await connection.ExecuteScalarAsync<int>(
            "SELECT public.get_comment_attachments_count(@userId, @organizationId, @reportId, @bugId, @commentId)",
            new
            {
                userId,
                organizationId,
                reportId,
                bugId,
                commentId
            }
        );

        return result;
    }

    public async Task<AttachmentDbModel> CreateAttachment(CreateAttachmentDbModel attachmentCreateDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<AttachmentDbModel>(
            "SELECT * FROM public.create_attachment_internal(@entity_id, @attach_type, @storage_key, @storage_kind, @creator_user_id, @length_bytes, @file_name, @mime_type)",
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

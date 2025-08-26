DROP FUNCTION IF EXISTS public.list_attachments_internal(_report_id int);

CREATE OR REPLACE FUNCTION public.list_attachments_internal(_report_id int)
    RETURNS TABLE(
        id int,
        attach_type int,
        created_at timestamp with time zone,
        entity_id int,
        storage_key text,
        storage_kind int,
        creator_user_id text,
        length_bytes bigint,
        file_name text,
        mime_type text,
        has_preview boolean,
        is_gzip_compressed boolean,
        bug_id int,
        path text
    )
    AS $$
DECLARE
    _fact_attach_type int = 0;
    _expected_attach_type int = 1;
    _comment_attach_type int = 2;
BEGIN
    RETURN QUERY
    -- Вложения, привязанные к багам
    SELECT
        a.id,
        a.attach_type,
        a.created_at,
        a.entity_id,
        a.storage_key,
        a.storage_kind,
        a.creator_user_id,
        a.length_bytes,
        a.file_name,
        a.mime_type,
        a.has_preview,
        a.is_gzip_compressed,
        a.bug_id,
        a.path
    FROM
        public.attachments a
        JOIN public.bugs b ON (a.entity_id = b.id or a.bug_id = b.id)
    WHERE (a.attach_type = _fact_attach_type
        OR a.attach_type = _expected_attach_type)
        AND b.report_id = _report_id
    UNION ALL
    -- Вложения, привязанные к комментариям
    SELECT
        a.id,
        a.attach_type,
        a.created_at,
        a.entity_id,
        a.storage_key,
        a.storage_kind,
        a.creator_user_id,
        a.length_bytes,
        a.file_name,
        a.mime_type,
        a.has_preview,
        a.is_gzip_compressed,
        a.bug_id,
        a.path
    FROM
        public.attachments a
        JOIN public.comments c ON a.entity_id = c.id
        JOIN public.bugs b ON c.bug_id = b.id
    WHERE
        a.attach_type = _comment_attach_type
        AND b.report_id = _report_id;
END;
$$
LANGUAGE plpgsql
STABLE;

DROP FUNCTION IF EXISTS public.get_bug_attachment(_organization_id text, _report_id int, _bug_id int, _attachment_id int);
CREATE OR REPLACE FUNCTION public.get_bug_attachment(_organization_id text, _report_id int, _bug_id int, _attachment_id int)
    RETURNS TABLE(
        id int,
        attach_type int,
        entity_id int,
        storage_key text,
        storage_kind int,
        creator_user_id text,
        length_bytes bigint,
        file_name text,
        mime_type text,
        has_preview boolean,
        is_gzip_compressed boolean,
        created_at timestamp with time zone,
        bug_id int,
        path text
    )
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        a.id,
        a.attach_type,
        a.entity_id,
        a.storage_key,
        a.storage_kind,
        a.creator_user_id,
        a.length_bytes,
        a.file_name,
        a.mime_type,
        a.has_preview,
        a.is_gzip_compressed,
        a.created_at,
        a.bug_id,
        a.path
    FROM
        public.attachments a
        JOIN public.bugs b ON (a.entity_id = b.id or a.bug_id = b.id)
        JOIN public.reports r ON b.report_id = r.id
    WHERE
        a.id = _attachment_id
        AND b.id = _bug_id
        AND b.report_id = _report_id
        AND(_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
END;
$$
LANGUAGE plpgsql;
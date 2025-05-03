CREATE OR REPLACE PROCEDURE public.check_bug_access(IN _bug_id integer,IN _report_id integer, IN _organization_id text)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS(
        SELECT
            1
        FROM
            public.bugs b
            JOIN public.reports r ON b.report_id = r.id
        WHERE
            b.id = _bug_id
            AND b.report_id = _report_id
            AND(_organization_id IS NULL
                OR r.creator_organization_id = _organization_id)) THEN
    RAISE EXCEPTION 'Bug % not found or access denied', _bug_id
        USING ERRCODE = 'P0404';
    END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE public.check_comment_access(IN _comment_id integer, IN _bug_id integer,IN _report_id integer, IN _organization_id text)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS(
        SELECT
            1
        FROM
            public.comments c
            JOIN public.bugs b ON c.bug_id = b.id
            JOIN public.reports r ON b.report_id = r.id
        WHERE
            c.id = _comment_id
            AND b.id = _bug_id
            AND b.report_id = _report_id
            AND(_organization_id IS NULL
                OR r.creator_organization_id = _organization_id)) THEN
    RAISE EXCEPTION 'Comment % not found or access denied', _comment_id
        USING ERRCODE = 'P0404';
    END IF;
END;
$$;


CREATE OR REPLACE FUNCTION public.get_bug_attachments_count(_organization_id text, _report_id int, _bug_id int, _attach_type int)
    RETURNS int
    AS $$
BEGIN
    -- Проверка доступа
    CALL check_bug_access(_bug_id, _report_id, _organization_id);
    RETURN(
        SELECT
            COUNT(*)
        FROM
            public.attachments a
        WHERE
            a.entity_id = _bug_id
            AND a.attach_type = _attach_type);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION public.get_comment_attachments_count(_organization_id text, _report_id int, _bug_id int, _comment_id int)
    RETURNS int
    AS $$
DECLARE
    _comment_attach_type int = 2;
BEGIN
    CALL check_comment_access(_comment_id, _bug_id, _report_id, _organization_id);
    RETURN (
        SELECT
            COUNT(*)
        FROM
            public.attachments a
        WHERE
            a.entity_id = _comment_id
            AND a.attach_type = _comment_attach_type);
END;
$$
LANGUAGE plpgsql;

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
        is_gzip_compressed boolean
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
        a.is_gzip_compressed
    FROM
        public.attachments a
        JOIN public.bugs b ON a.entity_id = b.id
    WHERE
        (a.attach_type = _fact_attach_type
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
        a.is_gzip_compressed
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

CREATE OR REPLACE FUNCTION public.create_attachment_internal(_entity_id int, _attach_type int, _storage_key text, _storage_kind int, _creator_user_id text, _length_bytes bigint, _file_name text, _mime_type text, _has_preview boolean, _is_gzip_compressed boolean)
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
        created_at timestamp with time zone
    )
    AS $$
DECLARE
    new_attachment_id int;
BEGIN
    INSERT INTO public.attachments(entity_id, attach_type, storage_key, storage_kind, creator_user_id, length_bytes, file_name, mime_type, has_preview, is_gzip_compressed, bug_id, path)
        VALUES (_entity_id, _attach_type, _storage_key, _storage_kind, _creator_user_id, _length_bytes, _file_name, _mime_type, _has_preview, _is_gzip_compressed, _entity_id, _storage_key)
    RETURNING
        public.attachments.id INTO new_attachment_id;
    --  Возвращаем данные
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
        a.created_at
    FROM
        public.attachments a
    WHERE
        a.id = new_attachment_id;
END;
$$
LANGUAGE plpgsql;

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
        created_at timestamp with time zone
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
        a.created_at
    FROM
        public.attachments a
        JOIN public.bugs b ON a.entity_id = b.id
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

CREATE OR REPLACE FUNCTION public.get_comment_attachment(_organization_id text, _report_id int, _bug_id int, _comment_id int, _attachment_id int)
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
        created_at timestamp with time zone
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
        a.created_at
    FROM
        public.attachments a
        JOIN public.comments c ON a.entity_id = c.id
        JOIN public.bugs b ON c.bug_id = b.id
        JOIN public.reports r ON b.report_id = r.id
    WHERE
        a.id = _attachment_id
        AND c.id = _comment_id
        AND b.id = _bug_id
        AND b.report_id = _report_id
        AND(_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION public.delete_bug_attachment(_organization_id text, _report_id int, _bug_id int, _attachment_id int)
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
        created_at timestamp with time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY DELETE FROM public.attachments a USING public.bugs b
    JOIN public.reports r ON b.report_id = r.id
    WHERE a.entity_id = b.id
        AND b.id = _bug_id
        AND b.report_id = _report_id
        AND(_organization_id IS NULL
            OR r.creator_organization_id = _organization_id)
        AND a.id = _attachment_id
    RETURNING
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
        a.created_at;
END;
$$;

CREATE OR REPLACE FUNCTION public.delete_comment_attachment(_organization_id text, _report_id int, _bug_id int, _comment_id int, _attachment_id int)
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
        created_at timestamp with time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY DELETE FROM public.attachments a USING public.comments c
    JOIN public.bugs b ON c.bug_id = b.id
    JOIN public.reports r ON b.report_id = r.id
    WHERE a.entity_id = c.id
        AND c.id = _comment_id
        AND b.id = _bug_id
        AND b.report_id = _report_id
        AND(_organization_id IS NULL
            OR r.creator_organization_id = _organization_id)
        AND a.id = _attachment_id
    RETURNING
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
        a.created_at;
END;
$$;


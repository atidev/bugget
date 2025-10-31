CREATE OR REPLACE FUNCTION public.list_reports_base(
    _organization_id   text DEFAULT NULL,
    _user_id           text DEFAULT NULL,
    _team_id           text DEFAULT NULL,
    _report_statuses   int[] DEFAULT NULL
)
RETURNS TABLE(
    id          int,
    created_at  timestamptz,
    updated_at  timestamptz
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        r.id,
        r.created_at,
        r.updated_at
    FROM public.reports r
    WHERE (_organization_id IS NULL OR r.creator_organization_id = _organization_id)
      AND (_team_id        IS NULL OR r.creator_team_id        = _team_id)
      AND (_report_statuses IS NULL OR r.status = ANY(_report_statuses))
      AND (_user_id        IS NULL OR EXISTS (
            SELECT 1
            FROM public.report_participants rp
            WHERE rp.report_id = r.id
              AND rp.user_id   = _user_id
      ));
$$;

CREATE OR REPLACE FUNCTION public.list_reports_ids(
    _skip              int,
    _take              int,
    _organization_id   text DEFAULT NULL,
    _user_id           text DEFAULT NULL,
    _team_id           text DEFAULT NULL,
    _report_statuses   int[] DEFAULT NULL
)
RETURNS TABLE(id int)
LANGUAGE sql
STABLE
AS $$
    SELECT
        b.id
    FROM public.list_reports_base(_organization_id, _user_id, _team_id, _report_statuses) AS b
    ORDER BY
        b.updated_at DESC NULLS FIRST,
        b.created_at DESC
    OFFSET GREATEST(_skip, 0)
    LIMIT  GREATEST(_take, 0);
$$;

CREATE OR REPLACE FUNCTION public.list_reports_count(
    _organization_id   text DEFAULT NULL,
    _user_id           text DEFAULT NULL,
    _team_id           text DEFAULT NULL,
    _report_statuses   int[] DEFAULT NULL
)
RETURNS bigint
LANGUAGE sql
STABLE
AS $$
    SELECT COUNT(*)
    FROM public.list_reports_base(_organization_id, _user_id, _team_id, _report_statuses);
$$;

CREATE OR REPLACE FUNCTION public.list_participants_internal(_report_ids int[])
    RETURNS TABLE(
        report_id int,
        user_id text)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        p.report_id,
        p.user_id
    FROM
        public.report_participants p
    WHERE
        p.report_id = ANY(_report_ids)
    ORDER BY
        p.report_id,
        p.user_id;
$$;

CREATE OR REPLACE FUNCTION public.list_bugs_internal(_report_ids int[])
    RETURNS TABLE(
        id int,
        report_id int,
        receive text,
        expect text,
        status int,
        creator_user_id text,
        created_at timestamp with time zone,
        updated_at timestamp with time zone)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        b.id,
        b.report_id,
        b.receive,
        b.expect,
        b.status,
        b.creator_user_id,
        b.created_at,
        b.updated_at
    FROM
        public.bugs b
    WHERE
        b.report_id = ANY(_report_ids);
$$;

CREATE OR REPLACE FUNCTION public.list_comments_internal(_report_ids int[])
    RETURNS TABLE(
        id int,
        bug_id int,
        text text,
        creator_user_id text,
        created_at timestamp with time zone,
        updated_at timestamp with time zone)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        c.id,
        c.bug_id,
        c.text,
        c.creator_user_id,
        c.created_at,
        c.updated_at
    FROM
        public.comments c
        JOIN public.bugs b ON c.bug_id = b.id
    WHERE
        b.report_id = ANY(_report_ids);
$$;

CREATE OR REPLACE FUNCTION public.list_attachments_internal(_report_ids int[])
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
    WHERE (a.attach_type = _fact_attach_type
        OR a.attach_type = _expected_attach_type)
        AND b.report_id = ANY (_report_ids)
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
        AND b.report_id = ANY (_report_ids);
END;
$$
LANGUAGE plpgsql
STABLE;

CREATE OR REPLACE FUNCTION public.list_reports_internal(_report_ids int[])
    RETURNS TABLE(
        id int,
        title text,
        status text,
        created_at timestamp with time zone,
        updated_at timestamp with time zone,
        creator_user_id text,
        creator_team_id text,
        responsible_user_id text,
        past_responsible_user_id text)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        r.id,
        r.title,
        r.status,
        r.created_at,
        r.updated_at,
        r.creator_user_id,
        r.creator_team_id,
        r.responsible_user_id,
        r.past_responsible_user_id
    FROM
        public.reports r
    WHERE
        r.id = ANY(_report_ids)
$$;
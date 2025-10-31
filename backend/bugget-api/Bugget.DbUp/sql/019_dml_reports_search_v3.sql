CREATE OR REPLACE FUNCTION public.search_reports_base(_query text, _statuses int[], _user_ids text[], _organization_id text)
    RETURNS TABLE(
        id int,
        created_at timestamp with time zone,
        updated_at timestamp with time zone,
        rank double precision)
    LANGUAGE plpgsql
    STABLE
    AS $$
DECLARE
    ts_query tsquery := NULL;
BEGIN
    IF _query IS NOT NULL AND LENGTH(TRIM(_query)) > 0 THEN
        ts_query := to_tsquery('russian', regexp_replace(trim(_query), '\s+', ':* & ', 'g') || ':*');
    END IF;
    RETURN QUERY
    SELECT
        r.id,
        r.created_at,
        r.updated_at,
        COALESCE(ts_rank(r.search_vector, ts_query), 0)::double precision * 1.5 + COALESCE((
            SELECT
                MAX(ts_rank(b.search_vector, ts_query))
            FROM public.bugs b
            WHERE
                b.report_id = r.id), 0)::double precision * 1.2 + COALESCE((
            SELECT
                MAX(ts_rank(c.search_vector, ts_query))
            FROM public.bugs b
            JOIN public.comments c ON c.bug_id = b.id
            WHERE
                b.report_id = r.id), 0)::double precision AS rank
    FROM
        public.reports r
    WHERE (_query IS NULL
        OR ts_query IS NULL
        OR r.search_vector @@ ts_query
        OR EXISTS (
            SELECT
                1
            FROM
                public.bugs b
            WHERE
                b.report_id = r.id
                AND b.search_vector @@ ts_query)
            OR EXISTS (
                SELECT
                    1
                FROM
                    public.bugs b
                    JOIN public.comments c ON c.bug_id = b.id
                WHERE
                    b.report_id = r.id
                    AND c.search_vector @@ ts_query)
                OR r.title ILIKE '%' || _query || '%'
                OR EXISTS (
                    SELECT
                        1
                    FROM
                        public.bugs b
                    WHERE
                        b.report_id = r.id
                        AND (b.receive ILIKE '%' || _query || '%'
                            OR b.expect ILIKE '%' || _query || '%'))
                    OR EXISTS (
                        SELECT
                            1
                        FROM
                            public.bugs b
                            JOIN public.comments c ON c.bug_id = b.id
                        WHERE
                            b.report_id = r.id
                            AND c.text ILIKE '%' || _query || '%'))
                AND (_statuses IS NULL
                    OR r.status = ANY (_statuses))
            AND (_organization_id IS NULL
                OR (r.creator_organization_id IS NOT NULL
                    AND r.creator_organization_id = _organization_id))
        AND (_user_ids IS NULL
            OR EXISTS (
                SELECT
                    1
                FROM
                    public.report_participants rp
                WHERE
                    rp.report_id = r.id
                    AND rp.user_id = ANY (_user_ids)));
END;
$$;

CREATE OR REPLACE FUNCTION public.search_reports_count(_query text DEFAULT NULL, _statuses int[] DEFAULT NULL, _user_ids text[] DEFAULT NULL, _organization_id text DEFAULT NULL)
    RETURNS bigint
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        COUNT(*)
    FROM
        public.search_reports_base(_query, _statuses, _user_ids, _organization_id);
$$;

CREATE OR REPLACE FUNCTION public.search_reports_ids(_sort_field text, _sort_desc boolean, _skip int, _take int, _query text DEFAULT NULL, _statuses int[] DEFAULT NULL, _user_ids text[] DEFAULT NULL, _organization_id text DEFAULT NULL)
    RETURNS TABLE(
        id int)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        id
    FROM
        public.search_reports_base(_query, _statuses, _user_ids, _organization_id)
    ORDER BY
        CASE WHEN _sort_field = 'rank'
            AND _sort_desc THEN
            rank
        END DESC NULLS LAST,
        CASE WHEN _sort_field = 'rank'
            AND NOT _sort_desc THEN
            rank
        END ASC NULLS FIRST,
        CASE WHEN _sort_field = 'created'
            AND _sort_desc THEN
            created_at
        END DESC,
        CASE WHEN _sort_field = 'created'
            AND NOT _sort_desc THEN
            created_at
        END ASC,
        CASE WHEN _sort_field = 'updated'
            AND _sort_desc THEN
            updated_at
        END DESC,
        CASE WHEN _sort_field = 'updated'
            AND NOT _sort_desc THEN
            updated_at
        END ASC,
        id DESC OFFSET GREATEST(_skip, 0)
    LIMIT GREATEST(_take, 0);
$$;


-- Create search_reports_v2 procedure
CREATE OR REPLACE FUNCTION public.search_reports_v2(
    _sort_field TEXT,
    _sort_desc BOOLEAN,
    _skip INT,
    _take INT,
    _query TEXT DEFAULT NULL,
    _statuses INT[] DEFAULT NULL,
    _user_id TEXT DEFAULT NULL,
    _team_id TEXT DEFAULT NULL,
    _organization_id TEXT DEFAULT NULL
)
RETURNS JSONB
LANGUAGE plpgsql
AS
$$
DECLARE
    ts_query tsquery := NULL;
    result JSONB;
BEGIN
    IF _query IS NOT NULL AND LENGTH(TRIM(_query)) > 0 THEN
        ts_query := to_tsquery('russian', regexp_replace(trim(_query), '\s+', ':* & ', 'g') || ':*');
    END IF;

    WITH ranked_reports AS (
        SELECT DISTINCT
            r.id,
            r.created_at,
            r.updated_at,
            CASE WHEN _sort_field = 'rank' THEN
            (
                COALESCE(ts_rank(r.search_vector, ts_query), 0) * 1.5 +
                COALESCE((SELECT MAX(ts_rank(b.search_vector, ts_query)) FROM public.bugs b WHERE b.report_id = r.id), 0) * 1.2 +
                COALESCE((SELECT MAX(ts_rank(c.search_vector, ts_query)) FROM public.comments c JOIN public.bugs b ON c.bug_id = b.id WHERE b.report_id = r.id), 0)
            )
            ELSE 0 END AS rank
        FROM public.reports r
        LEFT JOIN public.report_participants rp ON rp.report_id = r.id
        WHERE (_query IS NULL OR ts_query IS NULL OR
               r.search_vector @@ ts_query OR
               EXISTS (SELECT 1 FROM public.bugs b WHERE b.report_id = r.id AND b.search_vector @@ ts_query) OR
               EXISTS (SELECT 1 FROM public.bugs b JOIN public.comments c ON c.bug_id = b.id WHERE b.report_id = r.id AND c.search_vector @@ ts_query) OR
               r.title ILIKE '%' || _query || '%' OR
               EXISTS (SELECT 1 FROM public.bugs b WHERE b.report_id = r.id AND 
                       (b.receive ILIKE '%' || _query || '%' OR b.expect ILIKE '%' || _query || '%')) OR
               EXISTS (SELECT 1 FROM public.bugs b JOIN public.comments c ON c.bug_id = b.id WHERE b.report_id = r.id AND c.text ILIKE '%' || _query || '%'))
          AND (_statuses IS NULL OR r.status = ANY (_statuses))
          AND (_user_id IS NULL OR r.creator_user_id = _user_id OR rp.user_id = _user_id)
          AND (_team_id IS NULL OR r.creator_team_id = _team_id)
          AND (_organization_id IS NULL OR r.creator_organization_id = _organization_id)
    ),
    total AS (
        SELECT COUNT(*) AS cnt FROM ranked_reports
    ),
    sorted_reports AS (
        SELECT id
        FROM ranked_reports
        ORDER BY
            CASE WHEN _sort_field = 'rank' AND _sort_desc THEN rank END DESC NULLS LAST,
            CASE WHEN _sort_field = 'rank' AND NOT _sort_desc THEN rank END ASC NULLS FIRST,
            CASE WHEN _sort_field = 'created' AND _sort_desc THEN created_at END DESC,
            CASE WHEN _sort_field = 'created' AND NOT _sort_desc THEN created_at END ASC,
            CASE WHEN _sort_field = 'updated' AND _sort_desc THEN updated_at END DESC,
            CASE WHEN _sort_field = 'updated' AND NOT _sort_desc THEN updated_at END ASC
        OFFSET _skip LIMIT _take
    ),
    reports_array AS (
        SELECT jsonb_agg(public.get_report(id)) AS reports
        FROM sorted_reports
    )
    SELECT jsonb_build_object(
        'total', (SELECT cnt FROM total),
        'reports', COALESCE((SELECT reports FROM reports_array), '[]'::jsonb)
    ) INTO result;

    RETURN result;
END;
$$; 
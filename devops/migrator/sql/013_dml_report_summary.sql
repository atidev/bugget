CREATE OR REPLACE FUNCTION public.get_report_summary(_report_id integer, _organization_id text DEFAULT NULL)
RETURNS jsonb
LANGUAGE plpgsql
AS $$
DECLARE
    result jsonb;
BEGIN
    SELECT jsonb_build_object(
        'id', r.id,
        'title', r.title,
        'status', r.status,
        'responsible_user_id', r.responsible_user_id,
        'creator_user_id', r.creator_user_id,
        'created_at', r.created_at,
        'updated_at', r.updated_at
    ) INTO result
    FROM reports r
    WHERE r.id = _report_id
    AND (_organization_id IS NULL OR r.creator_organization_id = _organization_id);

    RETURN result;
END;
$$; 
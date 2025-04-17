CREATE OR REPLACE FUNCTION public.get_bug_summary(_report_id integer, _bug_id integer, _organization_id text DEFAULT NULL)
RETURNS jsonb
LANGUAGE plpgsql
AS $$
DECLARE
    result jsonb;
BEGIN
    SELECT jsonb_build_object(
        'id', b.id,
        'report_id', b.report_id,
        'receive', b.receive,
        'expect', b.expect,
        'creator_user_id', b.creator_user_id,
        'created_at', b.created_at,
        'updated_at', b.updated_at,
        'status', b.status
    ) INTO result
    FROM bugs b
    JOIN reports r ON r.id = b.report_id
    WHERE b.report_id = _report_id 
    AND b.id = _bug_id
    AND (_organization_id IS NULL OR r.creator_organization_id = _organization_id);

    RETURN result;
END;
$$; 
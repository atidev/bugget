-- Drop existing get_report procedure
DROP FUNCTION IF EXISTS public.get_report(integer);

-- Create updated get_report procedure with new fields
CREATE OR REPLACE FUNCTION public.get_report(_report_id INT, _organization_id TEXT DEFAULT NULL)
    RETURNS JSONB
    LANGUAGE plpgsql
AS
$$
DECLARE
    result JSONB;
BEGIN
    -- Формируем JSON с Report и его связями
    SELECT jsonb_build_object(
                   'id', r.id,
                   'title', r.title,
                   'status', r.status,
                   'created_at', r.created_at,
                   'updated_at', r.updated_at,
                   'creator_user_id', r.creator_user_id,
                   'creator_team_id', r.creator_team_id,
                   'creator_organization_id', r.creator_organization_id,
                   'responsible_user_id', r.responsible_user_id,
                   'bugs', COALESCE((SELECT jsonb_agg(
                                                    jsonb_build_object(
                                                            'id', b.id,
                                                            'report_id', b.report_id,
                                                            'receive', b.receive,
                                                            'expect', b.expect,
                                                            'status', b.status,
                                                            'creator_user_id', b.creator_user_id,
                                                            'created_at', b.created_at,
                                                            'updated_at', b.updated_at,
                                                            'attachments', COALESCE((SELECT jsonb_agg(
                                                                                                    jsonb_build_object(
                                                                                                            'id', a.id,
                                                                                                            'path', a.path,
                                                                                                            'attach_type', a.attach_type,
                                                                                                            'created_at', a.created_at
                                                                                                    )
                                                                                            )
                                                                                     FROM public.attachments a
                                                                                     WHERE a.bug_id = b.id), '[]'::jsonb),
                                                            'comments', COALESCE((SELECT jsonb_agg(
                                                                                                    jsonb_build_object(
                                                                                                            'id', c.id,
                                                                                                            'text', c.text,
                                                                                                            'creator_user_id', c.creator_user_id,
                                                                                                            'bug_id', c.bug_id,
                                                                                                            'created_at', c.created_at,
                                                                                                            'updated_at', c.updated_at
                                                                                                    )
                                                                                            )
                                                                                     FROM public.comments c
                                                                                     WHERE c.bug_id = b.id), '[]'::jsonb)
                                                    )
                                            )
                                     FROM public.bugs b
                                     WHERE b.report_id = r.id), '[]'::jsonb),
                   'participants_user_ids', COALESCE((SELECT jsonb_agg(user_id)
                                                      FROM public.report_participants
                                                      WHERE report_id = r.id), '[]'::jsonb)
           )
    INTO result
    FROM public.reports r
    WHERE r.id = _report_id
    AND (_organization_id IS NULL OR r.creator_organization_id = _organization_id);

    RETURN result;
END;
$$; 
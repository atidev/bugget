CREATE OR REPLACE FUNCTION public.list_reports(_user_id TEXT)
    RETURNS TABLE
            (
                report JSONB
            )
    LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN QUERY
        SELECT public.get_report(r.id)
        FROM public.reports r
        WHERE (r.status = 0 or r.status = 2)
          AND r.id IN (SELECT report_id
                       FROM public.report_participants
                       WHERE user_id = _user_id);
END;
$$;

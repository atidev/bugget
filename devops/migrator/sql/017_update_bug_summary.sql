CREATE OR REPLACE FUNCTION public.update_bug_summary(_bug_id int, _report_id int, _updater_user_id text, _receive text DEFAULT NULL, _expect text DEFAULT NULL, _status int DEFAULT NULL, _organization_id text DEFAULT NULL)
    RETURNS jsonb LANGUAGE plpgsql
    AS $$
DECLARE
    report_id text;
BEGIN
    -- Проверяем существование отчета и его организацию
    SELECT
        r.id INTO report_id
    FROM
        reports r
    WHERE
        r.id = _report_id
        AND (organization_id IS NULL
            OR r.organization_id = _organization_id);
    IF report_id IS NULL THEN
        RAISE EXCEPTION 'Report not found or access denied'
            USING ERRCODE = 'P0404';
        END IF;
        -- Обновляем bug, если параметры не null
        UPDATE
            public.bugs
        SET
            receive = COALESCE(_receive, receive),
            expect = COALESCE(_expect, expect),
            status = COALESCE(_status, status),
            updated_at = NOW()
        WHERE
            id = _bug_id;
        -- Добавляем updater_user_id в ReportParticipants, если его там нет
        INSERT INTO public.report_participants(report_id, user_id)
            VALUES (_report_id, _updater_user_id)
        ON CONFLICT (report_id, user_id)
            DO NOTHING;
        -- Возвращаем обновленный Bug через get_bug
        RETURN public.get_bug(_bug_id);
END;
$$;


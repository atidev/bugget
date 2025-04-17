DROP FUNCTION IF EXISTS public.create_bug(_report_id INT, _receive TEXT, _expect TEXT, _creator_user_id TEXT, _status INT)
CREATE OR REPLACE FUNCTION public.create_bug(_report_id int, _receive text, _expect text, _creator_user_id text, _status int, _organization_id text DEFAULT NULL)
    RETURNS jsonb LANGUAGE plpgsql
    AS $$
DECLARE
    new_bug_id integer;
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
        -- Создаём новую запись в таблице Bug
        INSERT INTO public.bugs(report_id, receive, expect, status, creator_user_id)
            VALUES (_report_id, _receive, _expect, _status, _creator_user_id)
        RETURNING
            id INTO new_bug_id;
        -- Добавляем creator_user_id в ReportParticipants, если его там нет
        INSERT INTO public.report_participants(report_id, user_id)
            VALUES (_report_id, _creator_user_id)
        ON CONFLICT (report_id, user_id)
            DO NOTHING;
        -- Возвращаем созданный Bug через get_report
        RETURN public.get_bug(new_bug_id);
END;
$$;


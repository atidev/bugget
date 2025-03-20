CREATE OR REPLACE FUNCTION public.create_report(
    _title TEXT,
    _status INT,
    _responsible_user_id TEXT,
    _creator_user_id TEXT,
    _participants TEXT[],
    _bugs_json TEXT
)
    RETURNS JSONB
    LANGUAGE plpgsql
AS
$$
DECLARE
    new_report_id INTEGER;
    bug_id        INTEGER;
    bug_json      JSONB;
BEGIN
    -- Создаём Report
    INSERT INTO public."Report" (title, status, responsible_user_id, creator_user_id)
    VALUES (_title, _status, _responsible_user_id, _creator_user_id)
    RETURNING id INTO new_report_id;

    -- Добавляем участников
    IF _participants IS NOT NULL AND array_length(_participants, 1) > 0 THEN
        INSERT INTO public."ReportParticipants" (report_id, user_id)
        SELECT new_report_id, unnest(_participants);
    END IF;

    -- Добавляем Bugs
    FOR bug_json IN
        SELECT * FROM jsonb_array_elements(_bugs_json::jsonb)
        LOOP
            INSERT INTO public."Bug" (report_id, receive, expect, status, creator_user_id)
            VALUES (new_report_id,
                    (bug_json ->> 'receive')::TEXT,
                    (bug_json ->> 'expect')::TEXT,
                    (bug_json ->> 'status')::INTEGER,
                    _creator_user_id)
            RETURNING id INTO bug_id;
        END LOOP;

    -- Возвращаем полный отчёт через get_report
    RETURN public.get_report(new_report_id);
END;
$$;

CREATE OR REPLACE FUNCTION public.get_report(_report_id INT)
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
                                                                                     FROM public."Attachment" a
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
                                                                                  FROM public."Comment" c
                                                                                  WHERE c.bug_id = b.id), '[]'::jsonb)
                                                    )
                                            )
                                     FROM public."Bug" b
                                     WHERE b.report_id = r.id), '[]'::jsonb),
                   'participants_user_ids', COALESCE((SELECT jsonb_agg(user_id)
                                                      FROM public."ReportParticipants"
                                                      WHERE report_id = r.id), '[]'::jsonb)
           )
    INTO result
    FROM public."Report" r
    WHERE r.id = _report_id;

    RETURN result;
END;
$$;

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
        FROM public."Report" r
        WHERE r.status = 0
          AND r.id IN (SELECT report_id
                       FROM public."ReportParticipants"
                       WHERE user_id = _user_id);
END;
$$;

CREATE OR REPLACE FUNCTION public.update_report(
    _report_id INT,
    _participants TEXT[],
    _title TEXT DEFAULT NULL,
    _status INT DEFAULT NULL,
    _responsible_user_id TEXT DEFAULT NULL
)
    RETURNS JSONB
    LANGUAGE plpgsql
AS
$$
BEGIN
    -- Обновляем report, если параметры не null
    UPDATE public."Report"
    SET title               = COALESCE(_title, title),
        status              = COALESCE(_status, status),
        responsible_user_id = COALESCE(_responsible_user_id, responsible_user_id)
    WHERE id = _report_id;

    -- Добавляем участников, если переданы
    IF array_length(_participants, 1) > 0 THEN
        INSERT INTO public."ReportParticipants" (report_id, user_id)
        SELECT _report_id, unnest(_participants)
        ON CONFLICT (report_id, user_id) DO NOTHING;
    END IF;

    -- Возвращаем обновленный отчет
    RETURN public.get_report(_report_id);
END;
$$;

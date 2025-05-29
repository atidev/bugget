CREATE
OR REPLACE FUNCTION public.get_bug(_bug_id INT)
    RETURNS JSONB
    LANGUAGE plpgsql
AS $$
DECLARE
result JSONB;
BEGIN
    -- Формируем JSON с Bug и его связями
SELECT jsonb_build_object(
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
                                        WHERE a.bug_id = b.id), '[]' ::jsonb),
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
                                     WHERE c.bug_id = b.id), '[]' ::jsonb)
       )
INTO result
FROM public.bugs b
WHERE b.id = _bug_id;

RETURN result;
END;
$$;


CREATE
OR REPLACE FUNCTION public.create_bug(
    _report_id INT,
    _receive TEXT,
    _expect TEXT,
    _creator_user_id TEXT,
    _status INT
)
    RETURNS JSONB
    LANGUAGE plpgsql
AS $$
DECLARE
new_bug_id INTEGER;
BEGIN
    -- Создаём новую запись в таблице Bug
INSERT INTO public.bugs (report_id, receive, expect, status, creator_user_id)
VALUES (_report_id, _receive, _expect, _status, _creator_user_id) RETURNING id
INTO new_bug_id;

-- Добавляем creator_user_id в ReportParticipants, если его там нет
INSERT INTO public.report_participants (report_id, user_id)
VALUES (_report_id, _creator_user_id) ON CONFLICT (report_id, user_id) DO NOTHING;

-- Возвращаем созданный Bug через get_report
RETURN public.get_bug(new_bug_id);
END;
$$;

CREATE
OR REPLACE FUNCTION public.update_bug(
    _bug_id INT,
    _report_id INT,
    _updater_user_id TEXT,
    _receive TEXT DEFAULT NULL,
    _expect TEXT DEFAULT NULL,
    _status INT DEFAULT NULL
)
RETURNS JSONB
LANGUAGE plpgsql
AS $$
BEGIN
    -- Обновляем bug, если параметры не null
UPDATE public.bugs
SET receive    = COALESCE(_receive, receive),
    expect     = COALESCE(_expect, expect),
    status     = COALESCE(_status, status),
    updated_at = NOW()
WHERE id = _bug_id;

-- Добавляем updater_user_id в ReportParticipants, если его там нет
INSERT INTO public.report_participants (report_id, user_id)
VALUES (_report_id, _updater_user_id) ON CONFLICT (report_id, user_id) DO NOTHING;

-- Возвращаем обновленный Bug через get_bug
RETURN public.get_bug(_bug_id);
END;
$$;
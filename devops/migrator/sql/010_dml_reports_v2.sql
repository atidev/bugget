CREATE OR REPLACE FUNCTION public.get_report_summary(_report_id integer, _organization_id text DEFAULT NULL)
    RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    result jsonb;
BEGIN
    SELECT
        jsonb_build_object('id', r.id, 'title', r.title, 'status', r.status, 'responsible_user_id', r.responsible_user_id, 'past_responsible_user_id', r.past_responsible_user_id, 'creator_user_id', r.creator_user_id, 'creator_team_id', r.creator_team_id, 'created_at', r.created_at, 'updated_at', r.updated_at) INTO result
    FROM
        reports r
    WHERE
        r.id = _report_id
        AND (_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
    RETURN result;
END;
$$;

CREATE OR REPLACE FUNCTION public.create_report(_user_id text, _title text, _team_id text DEFAULT NULL, _organization_id text DEFAULT NULL)
    RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    new_report_id integer;
    summary jsonb;
    participants jsonb;
BEGIN
    -- Создаём Report
    INSERT INTO public.reports(responsible_user_id, title, status, creator_user_id, creator_team_id, creator_organization_id, past_responsible_user_id)
        VALUES (_user_id, _title, 0, _user_id, _team_id, _organization_id, _user_id)
    RETURNING
        id INTO new_report_id;
    -- Добавляем участников
    INSERT INTO public.report_participants(report_id, user_id)
        VALUES (new_report_id, _user_id);
    -- Получаем краткое описание
    summary := public.get_report_summary(new_report_id, _organization_id);
    -- Получаем участников
    SELECT
        COALESCE(jsonb_agg(user_id), '[]'::jsonb) INTO participants
    FROM
        public.report_participants AS rp
    WHERE
        rp.report_id = new_report_id;
    -- Собираем финальный JSON
    RETURN summary || jsonb_build_object('participants_user_ids', participants);
END;
$$;

CREATE OR REPLACE FUNCTION public.patch_report(_report_id integer, _user_id text, _organization_id text DEFAULT NULL, _title text DEFAULT NULL, _status integer DEFAULT NULL, _responsible_user_id text DEFAULT NULL)
    RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    report_id integer;
BEGIN
    -- Проверка доступа
    SELECT
        r.id INTO report_id
    FROM
        public.reports r
    WHERE
        r.id = _report_id
        AND (_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Report % not found or access denied', _report_id
            USING ERRCODE = 'P0404';
        END IF;
        -- Обновление
        UPDATE
            public.reports
        SET
            updated_at = now(),
            past_responsible_user_id = CASE WHEN _responsible_user_id IS NOT NULL THEN
                responsible_user_id
            ELSE
                past_responsible_user_id
            END,
            status = COALESCE(_status, status),
            title = COALESCE(_title, title),
            responsible_user_id = COALESCE(_responsible_user_id, responsible_user_id)
        WHERE
            id = _report_id;
        RETURN public.get_report_summary(_report_id, _organization_id);
END;
$$;

CREATE OR REPLACE FUNCTION public.get_report_v2(_report_id int, _organization_id text DEFAULT NULL)
    RETURNS jsonb
    LANGUAGE plpgsql
    AS $$
DECLARE
    result jsonb;
BEGIN
    -- Формируем JSON с Report и его связями
    SELECT
        jsonb_build_object('id', r.id, 'title', r.title, 'status', r.status, 'created_at', r.created_at, 'updated_at', r.updated_at, 'creator_user_id', r.creator_user_id, 'creator_team_id', r.creator_team_id, 'responsible_user_id', r.responsible_user_id, 'past_responsible_user_id', r.past_responsible_user_id, 'bugs', COALESCE((
                SELECT
                    jsonb_agg(jsonb_build_object('id', b.id, 'report_id', b.report_id, 'receive', b.receive, 'expect', b.expect, 'status', b.status, 'creator_user_id', b.creator_user_id, 'created_at', b.created_at, 'updated_at', b.updated_at, 'attachments', COALESCE((
                                SELECT
                                    jsonb_agg(jsonb_build_object('id', a.id, 'path', a.path, 'attach_type', a.attach_type, 'created_at', a.created_at))
                            FROM public.attachments a
                            WHERE
                                a.bug_id = b.id), '[]'::jsonb), 'comments', COALESCE((
                            SELECT
                                jsonb_agg(jsonb_build_object('id', c.id, 'text', c.text, 'creator_user_id', c.creator_user_id, 'bug_id', c.bug_id, 'created_at', c.created_at, 'updated_at', c.updated_at))
                            FROM public.comments c
                        WHERE
                            c.bug_id = b.id), '[]'::jsonb)))
            FROM public.bugs b
        WHERE
            b.report_id = r.id), '[]'::jsonb), 'participants_user_ids', COALESCE((
        SELECT
            jsonb_agg(user_id)
        FROM public.report_participants AS rp
    WHERE
        rp.report_id = r.id), '[]'::jsonb)) INTO result
    FROM
        public.reports r
    WHERE
        r.id = _report_id
        AND (_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
    RETURN result;
END;
$$;

CREATE OR REPLACE FUNCTION public.add_participant_if_not_exist(_report_id integer, _user_id text)
    RETURNS text[] -- возвращаем массив идентификаторов или NULL
    LANGUAGE plpgsql
    AS $$
DECLARE
    inserted_count int;
    participants text[];
BEGIN
    -- пытаемся добавить, при конфликте ничего не делаем
    INSERT INTO public.report_participants(report_id, user_id)
        VALUES (_report_id, _user_id)
    ON CONFLICT (report_id, user_id)
        DO NOTHING;
        
    GET DIAGNOSTICS inserted_count = ROW_COUNT;
    -- если ничего не вставилось — возвращаем NULL
    IF inserted_count = 0 THEN
        RETURN NULL;
    END IF;
    -- собираем всех участников в массив
    SELECT
        array_agg(user_id) INTO participants
    FROM
        public.report_participants
    WHERE
        report_id = _report_id;
    RETURN participants;
END;
$$;

CREATE OR REPLACE FUNCTION public.change_status(_report_id integer, _new_status integer)
    RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE
        public.reports
    SET
        status = _new_status
    WHERE
        id = _report_id;
END;
$$;


CREATE OR REPLACE FUNCTION public.create_report_v2(_user_id text, _title text, _team_id text DEFAULT NULL, _organization_id text DEFAULT NULL)
    RETURNS TABLE(
        id int,
        title text,
        status int,
        created_at timestamp with time zone,
        updated_at timestamp with time zone,
        creator_user_id text,
        creator_team_id text,
        responsible_user_id text,
        past_responsible_user_id text)
    LANGUAGE plpgsql
    AS $$
DECLARE
    new_report_id integer;
BEGIN
    -- Создаём Report
    INSERT INTO public.reports(responsible_user_id, title, status, creator_user_id, creator_team_id, creator_organization_id, past_responsible_user_id)
        VALUES (_user_id, _title, 0, _user_id, _team_id, _organization_id, _user_id)
    RETURNING
        public.reports.id INTO new_report_id;
    -- Добавляем участников
    INSERT INTO public.report_participants(report_id, user_id)
        VALUES (new_report_id, _user_id);
    -- Возвращаем данные в том же формате, что и get_report_v2
    RETURN QUERY
    SELECT
        r.id,
        r.title,
        r.status,
        r.created_at,
        r.updated_at,
        r.creator_user_id,
        r.creator_team_id,
        r.responsible_user_id,
        r.past_responsible_user_id
    FROM
        public.reports AS r
    WHERE
        r.id = new_report_id;
END;
$$;

CREATE OR REPLACE PROCEDURE public.check_report_access(IN _report_id integer, IN _organization_id text)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS(
        SELECT
            1
        FROM
            public.reports r
        WHERE
            r.id = _report_id
            AND(_organization_id IS NULL
                OR r.creator_organization_id = _organization_id)) THEN
    RAISE EXCEPTION 'Report % not found or access denied', _report_id
        USING ERRCODE = 'P0404';
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION public.patch_report(_report_id integer, _organization_id text DEFAULT NULL, _title text DEFAULT NULL, _status integer DEFAULT NULL, _responsible_user_id text DEFAULT NULL)
    RETURNS TABLE(
        id integer,
        title text,
        status integer,
        responsible_user_id text,
        past_responsible_user_id text,
        updated_at timestamp with time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Проверка доступа
    CALL check_report_access(_report_id, _organization_id);
    ------------------------------------------------------------------
    -- Обновление: квалифицируем столбцы таблицы ТОЛЬКО в правой части
    ------------------------------------------------------------------
    UPDATE
        public.reports AS r
    SET
        updated_at = now(),
        past_responsible_user_id = CASE WHEN _responsible_user_id IS NOT NULL THEN
            r.responsible_user_id 
        ELSE
            r.past_responsible_user_id 
        END,
        status = COALESCE(_status, r.status),
        title = COALESCE(_title, r.title),
        responsible_user_id = COALESCE(_responsible_user_id, r.responsible_user_id)
    WHERE
        r.id = _report_id;
        
    ------------------------------------------------------------------
    -- Возврат обновлённой строки
    ------------------------------------------------------------------
    RETURN QUERY
    SELECT
        r.id,
        r.title,
        r.status,
        r.responsible_user_id,
        r.past_responsible_user_id,
        r.updated_at
    FROM
        public.reports AS r
    WHERE
        r.id = _report_id;
END;
$$;

-- 1) Общая информация по отчёту
CREATE OR REPLACE FUNCTION public.get_report_v2(_report_id int, _organization_id text DEFAULT NULL)
    RETURNS TABLE(
        id int,
        title text,
        status text,
        created_at timestamp,
        updated_at timestamp,
        creator_user_id text,
        creator_team_id text,
        responsible_user_id text,
        past_responsible_user_id text)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        r.id,
        r.title,
        r.status,
        r.created_at,
        r.updated_at,
        r.creator_user_id,
        r.creator_team_id,
        r.responsible_user_id,
        r.past_responsible_user_id
    FROM
        public.reports r
    WHERE
        r.id = _report_id
        AND(_organization_id IS NULL
            OR r.creator_organization_id = _organization_id);
$$;

-- 2) Все баги из отчёта
CREATE OR REPLACE FUNCTION public.list_bugs_internal(_report_id int)
    RETURNS TABLE(
        id int,
        report_id int,
        receive text,
        expect text,
        status text,
        creator_user_id text,
        created_at timestamp,
        updated_at timestamp)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        b.id,
        b.report_id,
        b.receive,
        b.expect,
        b.status,
        b.creator_user_id,
        b.created_at,
        b.updated_at
    FROM
        public.bugs b
    WHERE
        b.report_id = _report_id;
$$;

-- 3) Участники отчёта
CREATE OR REPLACE FUNCTION public.list_participants_internal(_report_id int)
    RETURNS TABLE(
        user_id text)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        p.user_id
    FROM
        public.report_participants p
    WHERE
        p.report_id = _report_id;
$$;

-- 4) Комментарии ко всем багам отчёта
CREATE OR REPLACE FUNCTION public.list_comments_internal(_report_id int)
    RETURNS TABLE(
        id int,
        bug_id int,
        text text,
        creator_user_id text,
        created_at timestamp,
        updated_at timestamp)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        c.id,
        c.bug_id,
        c.text,
        c.creator_user_id,
        c.created_at,
        c.updated_at
    FROM
        public.comments c
        JOIN public.bugs b ON c.bug_id = b.id
    WHERE
        b.report_id = _report_id;
$$;

-- 5) Вложения ко всем багам отчёта
CREATE OR REPLACE FUNCTION public.list_attachments_internal(_report_id int)
    RETURNS TABLE(
        id int,
        bug_id int,
        path text,
        attach_type text,
        created_at timestamp)
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        a.id,
        a.bug_id,
        a.path,
        a.attach_type,
        a.created_at
    FROM
        public.attachments a
        JOIN public.bugs b ON a.bug_id = b.id
    WHERE
        b.report_id = _report_id;
$$;

CREATE OR REPLACE FUNCTION public.add_participant_if_not_exist_internal(_report_id integer, _user_id text)
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

CREATE OR REPLACE FUNCTION public.change_status_internal(_report_id integer, _new_status integer)
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

CREATE OR REPLACE FUNCTION public.get_report_access(_report_id int, _organization_id text DEFAULT NULL)
    RETURNS boolean
    LANGUAGE sql
    STABLE
    AS $$
    SELECT
        EXISTS(
            SELECT
                1
            FROM
                public.reports r
            WHERE
                r.id = _report_id
                AND(_organization_id IS NULL
                    OR r.creator_organization_id = _organization_id));
$$;


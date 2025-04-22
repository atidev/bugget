CREATE OR REPLACE FUNCTION public.create_bug_v2(_user_id text, _organization_id text, _report_id int, _receive text, _expect text)
    RETURNS TABLE(
        id int,
        receive text,
        expect text,
        status text,
        creator_user_id text,
        created_at timestamp,
        updated_at timestamp)
    LANGUAGE plpgsql
    AS $$
DECLARE
    new_bug_id integer;
BEGIN
    -- Проверка доступа
    CALL check_report_access(_report_id, _organization_id);

    -- Создаём Bug
    INSERT INTO public.bugs(report_id, receive, expect, creator_user_id, status)
        VALUES (_report_id, _receive, _expect, _user_id, 0)
    RETURNING
        id INTO new_bug_id;
    --  Возвращаем данные
    RETURN QUERY
    SELECT
        b.id,
        b.receive,
        b.expect,
        b.status,
        b.creator_user_id,
        b.created_at,
        b.updated_at
    FROM
        public.bugs b
    WHERE
        b.id = new_bug_id;
END;
$$;

CREATE OR REPLACE FUNCTION public.patch_bug(_bug_id int, _report_id int, _organization_id text, _receive text, _expect text, _status int)
    RETURNS TABLE(
        id int,
        receive text,
        expect text,
        status int,
        updated_at timestamp)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Проверка доступа
    CALL check_report_access(_report_id, _organization_id);

    -- Обновляем Bug
    UPDATE public.bugs
        SET receive = _receive, expect = _expect, status = _status
    WHERE
        id = _bug_id
        AND report_id = _report_id

    --  Возвращаем данные
    RETURN QUERY
    SELECT
        b.id,
        b.receive,
        b.expect,
        b.status,
        b.updated_at
    FROM
        public.bugs b
    WHERE
        b.id = _bug_id;
END;
$$;




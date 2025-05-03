CREATE OR REPLACE FUNCTION public.create_comment_v2(_organization_id text, _user_id text, _report_id integer, _bug_id integer, _text text)
    RETURNS TABLE(
        id integer,
        bug_id integer,
        text text,
        creator_user_id text,
        created_at timestamp with time zone,
        updated_at timestamp with time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    CALL check_bug_access(_bug_id, _report_id, _organization_id);
    RETURN QUERY INSERT INTO public.comments(bug_id, text, creator_user_id)
        VALUES(_bug_id, _text, _user_id)
    RETURNING
        public.comments.id,
        public.comments.bug_id,
        public.comments.text,
        public.comments.creator_user_id,
        public.comments.created_at,
        public.comments.updated_at;
END;
$$;

CREATE OR REPLACE FUNCTION public.delete_comment_v2(_organization_id text, _user_id text, _report_id integer, _bug_id integer, _comment_id integer)
    RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Проверка доступа
    CALL check_comment_access(_comment_id, _bug_id, _report_id, _organization_id);
    -- Удаляем комментарий
    DELETE FROM public.comments
    WHERE public.comments.id = _comment_id
        AND public.comments.creator_user_id = _user_id;
END;
$$;

CREATE OR REPLACE FUNCTION public.update_comment_v2(_organization_id text, _user_id text, _report_id integer, _bug_id integer, _comment_id integer, _text text)
    RETURNS TABLE(
        id integer,
        bug_id integer,
        text text,
        creator_user_id text,
        created_at timestamp with time zone,
        updated_at timestamp with time zone)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- 1) Проверяем, что комментарий существует и вы имеете к нему доступ
    CALL check_comment_access(_comment_id, _bug_id, _report_id, _organization_id);
    -- 2) Обновляем и сразу возвращаем новые значения
    RETURN QUERY UPDATE
        public.comments c
    SET
        text = _text,
        updated_at = now()
    WHERE
        c.id = _comment_id
        AND c.creator_user_id = _user_id
    RETURNING
        c.id,
        c.bug_id,
        c.text,
        c.creator_user_id,
        c.created_at,
        c.updated_at;
END;
$$;


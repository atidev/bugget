CREATE OR REPLACE FUNCTION public.get_comment(_comment_id INT)
RETURNS JSONB
LANGUAGE plpgsql
AS $$
DECLARE
result JSONB;
BEGIN
    -- Формируем JSON с Comment
SELECT jsonb_build_object(
               'id', c.id,
               'text', c.text,
               'creator_user_id', c.creator_user_id,
               'bug_id', c.bug_id,
               'created_at', c.created_at,
               'updated_at', c.updated_at
       ) INTO result
FROM public."Comment" c
WHERE c.id = _comment_id;

RETURN result;
END;
$$;

CREATE OR REPLACE FUNCTION public.create_comment(
    _report_id INT,
    _bug_id INT,
    _text TEXT,
    _creator_user_id TEXT
)
RETURNS JSONB
LANGUAGE plpgsql
AS $$
DECLARE
new_comment_id INTEGER;
BEGIN
    -- Создаём новую запись в таблице Comment
INSERT INTO public."Comment" (bug_id, text, creator_user_id, created_at, updated_at)
VALUES (_bug_id, _text, _creator_user_id, NOW(), NOW())
    RETURNING id INTO new_comment_id;

-- Добавляем creator_user_id в ReportParticipants, если его там нет
INSERT INTO public."ReportParticipants" (report_id, user_id)
VALUES (_report_id, _creator_user_id)
    ON CONFLICT (report_id, user_id) DO NOTHING;

-- Возвращаем созданный комментарий через get_comment
RETURN public.get_comment(new_comment_id);
END;
$$;

CREATE OR REPLACE FUNCTION public.list_comments(
    _report_id INT,
    _bug_id INT
)
RETURNS TABLE(comment JSONB)
LANGUAGE plpgsql
AS $$
BEGIN
RETURN QUERY
SELECT public.get_comment(c.id)
FROM public."Comment" c
WHERE c.bug_id = _bug_id AND EXISTS (
    SELECT 1 FROM public."Bug" b WHERE b.id = _bug_id AND b.report_id = _report_id
);
END;
$$;

CREATE OR REPLACE PROCEDURE public.delete_comment(
    p_user_id TEXT,
    p_report_id INT,
    p_bug_id INT,
    p_comment_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN

    IF EXISTS (
        SELECT 1
        FROM public."Comment" c
        JOIN public."Bug" b ON b.id = c.bug_id
        JOIN public."Report" r ON r.id = b.report_id
        WHERE c.id = p_comment_id
          AND c.bug_id = p_bug_id
          AND r.id = p_report_id
          AND c.creator_user_id = p_user_id 
    )
    THEN
        DELETE FROM public."Comment"
        WHERE id = p_comment_id AND bug_id = p_bug_id;
    END IF;
END;
$$;
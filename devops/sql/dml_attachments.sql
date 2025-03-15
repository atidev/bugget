
CREATE OR REPLACE FUNCTION public.get_attachment(_attachment_id INT)
RETURNS JSONB
LANGUAGE plpgsql
AS $$
DECLARE
result JSONB;
BEGIN
-- Формируем JSON с Comment
SELECT jsonb_build_object(
               'id', c.id,
               'bug_id', c.bug_id,
               'created_at', c.created_at,
               'attach_type', c.attach_type,
               'path', c.path
       ) INTO result
FROM public."Attachment" c
WHERE c.id = _attachment_id;

RETURN result;
END;
$$;

CREATE
OR REPLACE FUNCTION public.create_attachment(
    _bug_id INT,
    _path TEXT,
    _attach_type INT,
    _created_at TIMESTAMP WITH TIME ZONE
)
    RETURNS JSONB
    LANGUAGE plpgsql
AS $$
DECLARE
new_bug_id INTEGER;
BEGIN
-- Создаём новую запись в таблице Attachment
INSERT INTO public."Attachment" (bug_id, path, attach_type, created_at)
VALUES (_bug_id, _path, _attach_type, _created_at) RETURNING id
INTO new_bug_id;

-- Возвращаем созданный Bug через get_report
RETURN public.get_attachment(new_bug_id);
END;
$$;


CREATE OR REPLACE FUNCTION public.delete_attachment(_attachment_id INT)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN

DELETE
FROM public."Attachment" a
WHERE a.id = _attachment_id;

END;
$$;


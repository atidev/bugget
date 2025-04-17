CREATE OR REPLACE FUNCTION public.update_report_summary(
    _report_id INT,
    _participants TEXT[],
    _title TEXT DEFAULT NULL,
    _status INT DEFAULT NULL,
    _responsible_user_id TEXT DEFAULT NULL,
    _organization_id TEXT DEFAULT NULL
)
    RETURNS JSONB
    LANGUAGE plpgsql
AS
$$
DECLARE
    _updated_id INT;
BEGIN
    -- Обновляем report, если найден и принадлежит организации (если передана)
    UPDATE public.reports
    SET title               = COALESCE(_title, title),
        status              = COALESCE(_status, status),
        responsible_user_id = COALESCE(_responsible_user_id, responsible_user_id),
        updated_at          = now()
    WHERE id = _report_id
      AND (_organization_id IS NULL OR creator_organization_id = _organization_id)
    RETURNING id INTO _updated_id;

    -- Если ничего не обновилось — либо не найден, либо нет прав
    IF _updated_id IS NULL THEN
        RAISE EXCEPTION 'Report not found or access denied'
            USING ERRCODE = 'P0404';
    END IF;

    -- Добавляем участников, если переданы
    IF array_length(_participants, 1) > 0 THEN
        INSERT INTO public.report_participants (report_id, user_id)
        SELECT _report_id, unnest(_participants)
        ON CONFLICT (report_id, user_id) DO NOTHING;
    END IF;

    -- Возвращаем обновленный отчет
    RETURN public.get_report_summary(_report_id, _organization_id);
END;
$$;
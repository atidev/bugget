-- Drop existing create_report procedure
DROP FUNCTION IF EXISTS public.create_report(text, integer, text, text, text[], jsonb);

-- Create updated create_report procedure with new fields
CREATE OR REPLACE FUNCTION public.create_report(
    _title text,
    _status integer,
    _responsible_user_id text,
    _creator_user_id text,
    _creator_team_id text,
    _creator_organization_id text,
    _participants text[],
    _bugs_json jsonb
) RETURNS jsonb
LANGUAGE plpgsql
AS $$
DECLARE
    _report_id integer;
    _bug jsonb;
    _bug_id integer;
    _participant text;
    _result jsonb;
BEGIN
    -- Create report
    INSERT INTO public.reports (
        title,
        status,
        responsible_user_id,
        creator_user_id,
        creator_team_id,
        creator_organization_id,
        created_at,
        updated_at
    ) VALUES (
        _title,
        _status,
        _responsible_user_id,
        _creator_user_id,
        _creator_team_id,
        _creator_organization_id,
        NOW(),
        NOW()
    ) RETURNING id INTO _report_id;

    -- Add participants
    FOREACH _participant IN ARRAY _participants
    LOOP
        INSERT INTO public.report_participants (report_id, user_id)
        VALUES (_report_id, _participant);
    END LOOP;

    -- Add bugs
    FOR _bug IN SELECT * FROM jsonb_array_elements(_bugs_json)
    LOOP
        INSERT INTO public.bugs (
            report_id,
            receive,
            expect,
            creator_user_id,
            status,
            created_at,
            updated_at
        ) VALUES (
            _report_id,
            _bug->>'receive',
            _bug->>'expect',
            _bug->>'creator_user_id',
            (_bug->>'status')::integer,
            NOW(),
            NOW()
        ) RETURNING id INTO _bug_id;

        -- Add attachments if any
        IF _bug->'attachments' IS NOT NULL THEN
            INSERT INTO public.attachments (
                bug_id,
                path,
                created_at,
                attach_type
            )
            SELECT
                _bug_id,
                attachment->>'path',
                NOW(),
                (attachment->>'attach_type')::integer
            FROM jsonb_array_elements(_bug->'attachments') AS attachment;
        END IF;
    END LOOP;

    -- Get full report with all relations
    SELECT public.get_report(_report_id) INTO _result;

    RETURN _result;
END;
$$; 
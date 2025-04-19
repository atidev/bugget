ALTER TABLE public.reports
    ADD COLUMN IF NOT EXISTS creator_team_id text DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS creator_organization_id text DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS past_responsible_user_id text DEFAULT NULL;
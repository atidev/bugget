-- Add CreatorTeamId and CreatorOrganizationId columns to reports table
ALTER TABLE public.reports
    ADD COLUMN IF NOT EXISTS creator_team_id text DEFAULT NULL,
    ADD COLUMN IF NOT EXISTS creator_organization_id text DEFAULT NULL;
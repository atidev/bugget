ALTER TABLE public.attachments
  ADD COLUMN IF NOT EXISTS entity_id int DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS storage_key text DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS storage_kind int DEFAULT 0,
  ADD COLUMN IF NOT EXISTS creator_user_id text DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS length_bytes bigint DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS file_name text DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS mime_type text DEFAULT NULL,
  ADD COLUMN IF NOT EXISTS has_preview boolean DEFAULT false,
  ADD COLUMN IF NOT EXISTS is_gzip_compressed boolean DEFAULT false,
  ADD COLUMN IF NOT EXISTS updated_at timestamp with time zone DEFAULT now();

ALTER TABLE public.attachments
  DROP CONSTRAINT IF EXISTS fk_attachments_bugs;


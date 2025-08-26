-- расширение для поиска по триграммам
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- Report
ALTER TABLE public.reports
ADD COLUMN IF NOT EXISTS search_vector tsvector
GENERATED ALWAYS AS (
    to_tsvector('russian', coalesce(title, ''))
) STORED;

CREATE INDEX IF NOT EXISTS report_search_vector_idx ON public.reports USING GIN (search_vector);
CREATE INDEX IF NOT EXISTS report_title_trgm_idx ON public.reports USING GIN (title gin_trgm_ops);

-- Bug
ALTER TABLE public.bugs
ADD COLUMN IF NOT EXISTS search_vector tsvector
GENERATED ALWAYS AS (
    to_tsvector('russian', coalesce(receive, '') || ' ' || coalesce(expect, ''))
) STORED;

CREATE INDEX IF NOT EXISTS bug_search_vector_idx ON public.bugs USING GIN (search_vector);
CREATE INDEX IF NOT EXISTS bug_receive_trgm_idx ON public.bugs USING GIN (receive gin_trgm_ops);
CREATE INDEX IF NOT EXISTS bug_expect_trgm_idx ON public.bugs USING GIN (expect gin_trgm_ops);

-- Comment
ALTER TABLE public.comments
ADD COLUMN IF NOT EXISTS search_vector tsvector
GENERATED ALWAYS AS (
    to_tsvector('russian', coalesce(text, ''))
) STORED;

CREATE INDEX IF NOT EXISTS comment_search_vector_idx ON public.comments USING GIN (search_vector);
CREATE INDEX IF NOT EXISTS comment_text_trgm_idx ON public.comments USING GIN (text gin_trgm_ops);

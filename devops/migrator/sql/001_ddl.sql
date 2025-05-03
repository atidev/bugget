-- Таблица отчётов
CREATE TABLE IF NOT EXISTS public.reports (
    id integer generated always as identity
        primary key,
    title TEXT NOT NULL,
    status INT NOT NULL,
    responsible_user_id text NOT NULL,
    creator_user_id text NOT NULL,
    created_at timestamp with time zone not null default now(),
    updated_at timestamp with time zone not null default now()
);

-- Таблица участников
CREATE TABLE IF NOT EXISTS public.report_participants (
    report_id INT NOT NULL,
    user_id text NOT NULL,
    PRIMARY KEY (report_id, user_id),
    CONSTRAINT fk_reports
        FOREIGN KEY (report_id)
        REFERENCES public.reports(id)
);

-- Таблица багов
CREATE TABLE IF NOT EXISTS public.bugs (
    id integer generated always as identity
        primary key,
    report_id INT NOT NULL,
    receive TEXT NOT NULL,
    expect TEXT NOT NULL,
    created_at timestamp with time zone not null default now(),
    updated_at timestamp with time zone not null default now(),
    creator_user_id text NOT NULL,
    status INT NOT NULL,

    CONSTRAINT fk_bugs_reports
        FOREIGN KEY (report_id)
        REFERENCES public.reports(id)
);

-- Таблица вложений
CREATE TABLE IF NOT EXISTS public.attachments (
    id integer generated always as identity
        primary key,
    bug_id INT NOT NULL,
    path TEXT NOT NULL,
    created_at timestamp with time zone not null default now(),
    attach_type INT NOT NULL,

    CONSTRAINT fk_attachments_bugs
        FOREIGN KEY (bug_id)
        REFERENCES public.bugs(id)
);

-- Таблица комментариев
CREATE TABLE IF NOT EXISTS public.comments (
    id integer generated always as identity
        primary key,
    bug_id INT NOT NULL,
    text TEXT NOT NULL,
    creator_user_id text NOT NULL,
    created_at timestamp with time zone not null default now(),
    updated_at timestamp with time zone not null default now(),

    CONSTRAINT fk_comments_bugs
        FOREIGN KEY (bug_id)
        REFERENCES public.bugs(id)
);
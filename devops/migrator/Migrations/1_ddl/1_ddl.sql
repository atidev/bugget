-- Таблица отчётов
CREATE TABLE IF NOT EXISTS public."Report" (
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
CREATE TABLE IF NOT EXISTS public."ReportParticipants" (
    report_id INT NOT NULL,
    user_id text NOT NULL,
    PRIMARY KEY (report_id, user_id),
    CONSTRAINT fk_report
        FOREIGN KEY (report_id)
        REFERENCES public."Report"(id)
);

-- Таблица "Bug"
CREATE TABLE IF NOT EXISTS public."Bug" (
    id integer generated always as identity
        primary key,
    report_id INT NOT NULL,
    receive TEXT NOT NULL,
    expect TEXT NOT NULL,
    created_at timestamp with time zone not null default now(),
    updated_at timestamp with time zone not null default now(),
    creator_user_id text NOT NULL,
    status INT NOT NULL,

    CONSTRAINT fk_bug_report
        FOREIGN KEY (report_id)
        REFERENCES public."Report"(id)
);

-- Таблица вложений (Attachment)
CREATE TABLE IF NOT EXISTS public."Attachment" (
    id integer generated always as identity
        primary key,
    bug_id INT NOT NULL,
    path TEXT NOT NULL,
    created_at timestamp with time zone not null default now(),
    attach_type INT NOT NULL,

    CONSTRAINT fk_attachment_bug
        FOREIGN KEY (bug_id)
        REFERENCES public."Bug"(id)
);

-- Таблица комментариев (Comment)
CREATE TABLE IF NOT EXISTS public."Comment" (
    id integer generated always as identity
        primary key,
    bug_id INT NOT NULL,
    text TEXT NOT NULL,
    creator_user_id text NOT NULL,
    created_at timestamp with time zone not null default now(),
    updated_at timestamp with time zone not null default now(),


    CONSTRAINT fk_comment_bug
        FOREIGN KEY (bug_id)
        REFERENCES public."Bug"(id)
);
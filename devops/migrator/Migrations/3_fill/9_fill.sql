-- ====== Report #1 ======
INSERT INTO public."Report" (title, status, responsible_user_id, creator_user_id)
VALUES ('Проблема с логином', 0, '1', 'default-user');

-- допустим id = 1
-- Участники:
INSERT INTO public."ReportParticipants" (report_id, user_id) VALUES
(1, '1'),
(1, 'default-user'),
(1, 'any-ldap-id');

-- Баг #1
INSERT INTO public."Bug" (report_id, receive, expect, creator_user_id, status)
VALUES (
  1,
  'Появляется ошибка 401 при вводе корректных данных',
  'Успешный вход в систему',
  'any-ldap-id',
  0
);

-- допустим id = 1
INSERT INTO public."Comment" (bug_id, text, creator_user_id)
VALUES (
  1,
  'У меня тоже возникает ошибка 401 при входе',
  '1'
);

-- Баг #2
INSERT INTO public."Bug" (report_id, receive, expect, creator_user_id, status)
VALUES (
  1,
  'Пользователь остаётся на той же странице',
  'Редирект на дашборд',
  'default-user',
  1
);

-- ====== Report #2 ======
INSERT INTO public."Report" (title, status, responsible_user_id, creator_user_id)
VALUES ('UI не обновляется после создания бага', 1, 'guid', 'int');

-- допустим id = 2
-- Участники:
INSERT INTO public."ReportParticipants" (report_id, user_id) VALUES
(2, 'guid'),
(2, 'int'),
(2, '1'),
(2, 'default-user');

-- Баг #1
INSERT INTO public."Bug" (report_id, receive, expect, creator_user_id, status)
VALUES (
  2,
  'После добавления бага список не обновляется',
  'Баг появляется без перезагрузки',
  'int',
  0
);

-- Баг #2 (без комментариев)
INSERT INTO public."Bug" (report_id, receive, expect, creator_user_id, status)
VALUES (
  2,
  'Баг создаётся, но не отображается badge',
  'Появляется иконка с количеством багов',
  'guid',
  1
);
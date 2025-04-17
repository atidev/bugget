# 🤖 Инструкция для AI-агента: как работать с проектом Bugget

## 🔍 Цель этого файла

Этот документ нужен, чтобы AI-агент сразу понял **контекст**, **структуру проекта** и **как правильно в него вписываться**. Перед выполнением задач читай это как чеклист.

## 📦 Архитектура проекта

### Backend (`/backend`)

На .NET (ASP.NET), структура слоями:
- `Bugget.Entities` — DTO, BO, DbModels, Views
- `Bugget.DA` — доступ к данным (PostgreSQL, Files)
- `Bugget.BO` — бизнес-логика
- `Bugget.Features` — сквозные фичи (например, уведомления)
- `Bugget` — Web API, Hubs, Controllers

Особенности:
- Используется WebSocket (`ReportPageHub.cs`)
- Все DTO и View-объекты лежат в `Entities`
- В папках `Controllers/Obsolete` — устаревшие API, не трогай

### Frontend (`/frontend`)

На React + TypeScript + Effector + DaisyUI

Структура:
- `src/pages/` — страницы (например, `Home`, `Report`, `Search`)
- `src/components/` — общие компоненты (DropDown, Sidebar и пр.)
- `src/store/` — Effector хранилища
- `src/types/` — типы
- `src/api/` — обёртки над API

Особенности:
- Используется Vite
- Никакого CRA
- UI — минимум обёрток, максимум читаемости
- Компоненты самодостаточны, без HOC/контекстов

### DevOps (`/devops`)
- `migrator` — SQL-скрипты для миграций, запускаются отдельно, там пишутся скрипты наката, старые скрипты не изменяются, подход "только вперед"
  - Новые скрипты именуем последовательно (`n+1_*.sql`)

---

## 🚨 Что не нужно делать

- ❌ Не создавай новых слоёв
- ❌ Не добавляй папки вроде `core`, `utils`, `common`, если не нужно
- ❌ Не придумывай “архитектурные улучшения” без запроса
- ❌ Не добавляй новый код, если задача про изменение существующего
- ❌ Не трогай `*.Obsolete.*` без указания

---

## ✅ Как работать правильно

- Используй уже существующие сущности, структуры, паттерны
- Ищи подходящие маппинги, DTO, типы, прежде чем создавать новые
- Работай в стиле проекта: `маленькие шаги, без магии`
- Работай в рамках проекта: `если это backend, то правки только в /backend или /devops/migrator/sql, если это frontend, то правки только /frontend`
- Если файл или структура уже есть — используй её
- Можешь коммментировать сложные конструкции на русском языке
---

## 🛠 Пример — добавление поля к репорту

1. DTO: `ReportDto`, `ReportUpdateDto`
2. DbModel: `ReportDbModel`, `ReportUpdateDbModel`
3. View: `ReportView`
4. Сервис: `ReportsService.cs`
5. Контроллер: `ReportsController.cs`
6. Frontend: `src/types/report.ts`, `src/api/reports/index.ts`, `src/pages/Report/...`

---

## 🧭 Быстрая навигация

| Что нужно          | Где искать                                          |
|--------------------|------------------------------------------------------|
| Сущности / DTO / BO| `backend/Bugget.Entities/`                          |
| Бизнес-логика      | `backend/Bugget.BO/Services`                        |
| Доступ к данным    | `backend/Bugget.DA/Postgres/`                       |
| Web API            | `backend/Bugget/Controllers`                        |
| WebSocket          | `backend/Bugget/Hubs/ReportPageHub.cs`             |
| UI-страницы        | `frontend/src/pages/`                               |
| Компоненты страницы| `frontend/src/pages/{Page}/components`             |
| Общие компоненты   | `frontend/src/components/`                          |
| Effector store     | `frontend/src/store/`                               |
| Типы               | `frontend/src/types/`                               |
| API-интерфейсы     | `frontend/src/api/`                                 |
| Хуки               | `frontend/src/hooks/`                               |
| Миграции SQL       | `devops/migrator/sql/*.sql`                         |

---

## 📌 Для AI

- Лишний раз не торопись
- Если не хватает интерфейса или чего-то что упростит задачу — спроси
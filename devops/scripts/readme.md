# Локальный запуск компонентов в Docker

## Требования

Перед запуском убедитесь, что у вас установлены:

- Docker
- Docker Compose

## Запуск компонентов

В проекте есть файлы для запуска различных наборов сервисов:

- `all.yml` – запуск всей платформы (PostgreSQL, Backend, Frontend).
- `pg_front.yml` – запуск только PostgreSQL и Frontend.
- `pg_back.yml` – запуск только PostgreSQL и Backend.

### Запуск

1. Перейдите в директорию `devops/scripts`:

   ```sh
   cd devops/scripts
   ```

2. Запустите нужный набор сервисов:

   - Полный запуск всех компонентов:
     ```sh
     docker-compose -f all.yml up --pull always -d
     ```
   - Запуск фронтенда и базы данных:
     ```sh
     docker-compose -f pg_front.yml up --pull always -d
     ```
   - Запуск бэкенда и базы данных:
     ```sh
     docker-compose -f pg_back.yml up --pull always -d
     ```
   - Запуск платформы с пересборкой образов на основе текущей ветки:
     ```sh
     docker-compose --env-file .build.env -f all.yml build --no-cache  
     docker-compose --env-file .build.env -f all.yml up -d
     ```

   Флаг `-d` запускает контейнеры в фоновом режиме. Если хотите видеть логи всех компонентов в реальном времени, уберите этот флаг.

3. Проверьте запущенные контейнеры (или используйте UI-интерфейс Docker):

   ```sh
   docker ps
   ```

## Остановка компонентов

Для остановки и удаления всех запущенных контейнеров используйте:

```sh
docker-compose -f <file_name>.yml down
```

Где `<file_name>` – это `all.yml`, `pg_back.yml` или `pg_front.yml` в зависимости от запущенной конфигурации.

# Локальный запуск компонентов в Docker

## Требования

Перед запуском убедитесь, что у вас установлены:

- Docker
- Docker Compose

## Запуск компонентов

В проекте есть три файла для запуска различных наборов сервисов:

- `test.yml` – Запуск всей платформы (PostgreSQL, Backend, Frontend).
- `test_backend.yml` – Запуск PostgreSQL и Frontend (без Backend).
- `test_frontend.yml` – Запуск PostgreSQL и Backend (без Frontend).

### Запуск

1. Перейдите в директорию `devops/scripts`:

   ```sh
   cd devops/scripts
   ```

2. Запустите нужный набор сервисов:

   - Полный запуск всех компонентов:
     ```sh
     docker-compose -f test.yml up -d
     ```
   - Запуск бэкенда и базы данных (без фронтенда):
     ```sh
     docker-compose -f test_backend.yml up -d
     ```
   - Запуск фронтенда и базы данных (без бэкенда):
     ```sh
     docker-compose -f test_frontend.yml up -d
     ```

   Флаг `-d` позволяет запустить контейнеры в фоновом режиме. Иногда полезно его не указывать, чтобы видеть логи всех компонентов.

3. Проверьте запущенные контейнеры (или через UI-интерфейс Docker):

   ```sh
   docker ps
   ```

## Остановка компонентов

Для остановки и удаления всех запущенных контейнеров используйте:

```sh
docker-compose -f <file_name>.yml down
```

Замените `<file_name>` на `test.yml`, `test_backend.yml` или `test_frontend.yml` в зависимости от используемого конфигурационного файла.

## Логи сервисов

Для просмотра логов определенного сервиса используйте:

```sh
docker-compose -f <file_name>.yml logs -f <service_name>
```

Например, для просмотра логов бэкенда:

```sh
docker-compose -f test.yml logs -f backend
```

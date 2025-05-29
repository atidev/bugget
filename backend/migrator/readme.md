# Мигратор
Консольное приложение для автоматизации наката схемы бд

## Локальный запуск

1 - поднимаем пустую бд
```sh
docker run --rm \
--name postgres_bugget \
-e POSTGRES_PASSWORD=postgres \
-e POSTGRES_HOST_AUTH_METHOD=trust \
-p 5432:5432 \
--network bridge \
postgres
```
2 - запускам с конфигурацией **localMigrator**  
3 - смотрим что получилось

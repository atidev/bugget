# Фронтенд

## Стэк

- vite + ts + effector
- lucide – иконки
- tailwind – css-framework
- [daisyui](https://daisyui.com/) – plugin для tailwind готовыми компонентами

TODO писать сюда все новые пакеты что добавляем и что они делают, почему добавляем, чтобы не было дубликатов

- date-fns
- simple-git hooks – прекоммит хуки
- lint-staged – запуск комманд только для "staged" файлов
- eslint+prettier – линтер+форматтер

## Локальная разработка

### Перед началом работы

- npm ci – установка зависимостей и активация прекоммит-хуков

### Команды для локального запуска

- npm run dev – запуск фронтового приложения на дев-сервере
- npm run start-backend – запуск локальной БД и локального бэкенда
- npm start – запуск двух предыдущих команд последовательно

### Precommit-хуки

- если не работают прекоммит хуки (должны активироваться командой npm ci), то нужно вручную запустить команду `npx simple-git-hooks`

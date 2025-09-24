#!/bin/sh
set -e

# Заменяем плейсхолдеры в env.js на реальные значения
sed -i "s|__API_URL__|${API_URL}|g" /usr/share/nginx/html/env.js

# Запускаем Nginx
exec nginx -g "daemon off;"

#!/bin/sh
set -e

# Заменяем плейсхолдеры в env.js на реальные значения
sed -i "s|__API_URL__|${API_URL}|g" /usr/share/nginx/html/env.js
sed -i "s|__BASE_PATH__|${BASE_PATH:-/}|g" /usr/share/nginx/html/env.js
sed -i "s|__USERS_API_URL__|${USERS_API_URL}|g" /usr/share/nginx/html/env.js

# Запускаем Nginx
exec nginx -g "daemon off;"

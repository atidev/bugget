#!/bin/sh
set -e

# Заменяем плейсхолдер в env.js на реальный API_URL
sed -i "s|__API_URL__|${API_URL}|g" /usr/share/nginx/html/env.js

# Запускаем Nginx
exec nginx -g "daemon off;"

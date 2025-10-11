# bugget-api

## backend-v2-api

### Reports API v2

#### POST /v2/reports
Создание нового отчета.

##### Запрос
```json
{
  "title": "string" // required, min length 1, max length 128
}
```

##### Ответ
```json
{
  "id": "integer",
  "title": "string",
  "status": "integer", // Backlog = 0, Resolved = 1, InProgress = 2, Rejected = 3
  "responsible_user_id": "string",
  "past_responsible_user_id": "string", // предыдущий ответственный для кнопки ("вернуть репорт")
  "creator_user_id": "string",
  "creator_team_id": "string", // в настоящий момент не используется
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

##### Сайд-эффекты
1. Автоматические действия:
   - Добавление создателя в участники отчета

#### GET /v2/reports/{reportId}
Получение полной информации об отчете.

##### Параметры
- `reportId` (path, integer) - ID отчета

##### Ответ
```json
{
  "id": "integer",
  "title": "string",
  "status": "integer", // Backlog = 0, Resolved = 1, InProgress = 2, Rejected = 3
  "responsible_user_id": "string", // только id юзера
  "past_responsible_user_id": "string", // предыдущий ответственный для кнопки ("вернуть репорт")
  "creator_user_id": "string",
  "creator_team_id": "string", // в настоящий момент не используется
  "created_at": "datetime",
  "updated_at": "datetime",
  "participants_user_ids": ["string"], // только id юзера, его мета будет получаться в методе ниже
  "bugs": [
    {
      "id": "integer",
      "report_id": "integer",
      "receive": "string", // фактический
      "expect": "string", // ожидаемый
      "status": "integer", // Active = 0, Archived = 1, Rejected = 2
      "creator_user_id": "string",
      "created_at": "datetime",
      "updated_at": "datetime",
      "comments": [
        {
          "id": "integer",
          "bug_id": "integer",
          "text": "string",
          "creator_user_id": "string",
          "created_at": "datetime",
          "updated_at": "datetime",
          "attachments": [
            {
              "id": "integer",
              "entity_id": "integer", // в будущем спрячу
              "attach_type": "integer", // для комментов не будет этого поля
              "storage_key": "string", // в будущем спрячу
              "storage_kind": "integer", // в будущем спрячу
              "created_at": "datetime",
              "creator_user_id": "string",
              "length_bytes": "integer", // в будущем спрячу
              "file_name": "string", // это можно при наведении будет отображать, но хз
              "mime_type": "string", // в будущем спрячу
              "has_preview": "boolean", // если стоит true то можно запросить /preview (иначе дефолтная иконка)
              "is_gzip_compressed": "boolean" // в будущем спрячу
            }
          ]
        }
      ],
      "attachments": [
        {
          "id": "integer",
          "entity_id": "integer",
          "attach_type": "integer", // тут оставлю, поэтому полю понимаем слева или справа аттач у бага
          "storage_key": "string",
          "storage_kind": "integer",
          "created_at": "datetime",
          "creator_user_id": "string",
          "length_bytes": "integer",
          "file_name": "string",
          "mime_type": "string",
          "has_preview": "boolean",
          "is_gzip_compressed": "boolean"
        }
      ]
    }
  ]
}
```

#### PATCH /v2/reports/{reportId}
Частичное обновление отчета.

##### Параметры
- `reportId` (path, integer) - ID отчета

##### Запрос
В действительности передается только одно поле - которое изменяется (остальное не надо передавать!)
```json
{
  "title": "string", // optional, min length 1, max length 128
  "status": "integer", // optional, range 0-3 (Backlog = 0, Resolved = 1, InProgress = 2, Rejected = 3)
  "responsible_user_id": "string" // optional, min length 1, max length 256
}
```

##### Ответ
```json
{
  "id": "integer",
  "title": "string",
  "status": "integer", // Backlog = 0, Resolved = 1, InProgress = 2, Rejected = 3
  "responsible_user_id": "string",
  "past_responsible_user_id": "string", // предыдущий ответственный для кнопки ("вернуть репорт")
  "updated_at": "datetime"
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveReportPatch(view)`
   - Тело события:
   Придет только измененное поле, остальное null (важно реагировать только не на null!)
   ```json
   {
     "title": "string",
     "status": "integer",
     "responsibleUserId": "string",
     "pastResponsibleUserId": "string"
   }
   ```
   Обрати внимание, что в веб-сокетах все приходит в camelCase. Если что как работают сокеты и что там приходит можно глянуть прям в браузере

2. Автоматические действия:
   - При смене ответственного в статусе "backlog" - автоматический переход в "in progress"
   - Добавление нового ответственного в участники отчета
   - Отправка уведомлений через Mattermost новому ответственному

3. Возможные ошибки:
   - 404: Отчет не найден
   - 400: Невалидные входные данные

### Как обрабатывать ошибки
Чаще всего ошибок быть не должно, то есть на UI их никак не отобразить, поэтому все ошибки должны отображатся в errno стиле, завязываться необходимо на поле reason
```json
{
    "error": "код ошибки",
    "reason": "человекочитаемый вид ошибки"
}
```

### Users API v1

#### POST /v1/users/batch/list
Получение информации о пользователях по их идентификаторам.

##### Запрос
```json
["string", "string1"] // массив id пользователей
```

##### Ответ
```json
[
  {
    "id": "string",
    "name": "string",
    "photo_url": "string", // если в браузере есть авторизация в time то можно попробовать по урлу достать изображение, в будущем подумаем как избегать авторизации
    "team_id": "string" // пока не нужно
  }
]
```

##### Возможные ошибки
- 400: Пустой массив или null
- 400: Невалидные входные данные

### Bugs API v2

#### POST /v2/reports/{reportId}/bugs
Создание нового бага в отчете.

##### Параметры
- `reportId` (path, integer) - ID отчета

##### Запрос
```json
{
  "receive": "string", // optional, min length 1, max length 2048
  "expect": "string" // optional, min length 1, max length 2048
}
```
Примечание: хотя поля опциональные, хотя бы одно из них должно быть заполнено.

##### Ответ
```json
{
  "id": "integer",
  "receive": "string",
  "expect": "string",
  "created_at": "datetime",
  "updated_at": "datetime",
  "creator_user_id": "string",
  "status": "integer" // Active = 0, Archived = 1, Rejected = 2
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveBugCreate` - событие создания нового бага
   - Тело события:
   ```json
   {
     "id": "integer",
     "receive": "string",
     "expect": "string",
     "createdAt": "datetime",
     "updatedAt": "datetime",
     "creatorUserId": "string",
     "status": "integer"
   }
   ```

#### PATCH /v2/reports/{reportId}/bugs/{bugId}
Частичное обновление бага.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага

##### Запрос
```json
{
  "receive": "string", // optional, min length 1, max length 2048
  "expect": "string", // optional, min length 1, max length 2048
  "status": "integer" // optional, range 0-2 (Active = 0, Archived = 1, Rejected = 2)
}
```
В действительности передается только одно поле - которое изменяется (остальное не надо передавать!)

##### Ответ
```json
{
  "id": "integer",
  "receive": "string",
  "expect": "string",
  "updated_at": "datetime",
  "status": "integer" // Active = 0, Archived = 1, Rejected = 2
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveBugPatch` - событие обновления бага
   - Параметры события:
     - `bugId` (integer) - ID обновленного бага
     - Тело события:
     ```json
     {
       "receive": "string",
       "expect": "string",
       "status": "integer"
     }
     ```
   Придет только измененное поле, остальное null (важно реагировать только не на null!)

### Bug Attachments API v2

#### POST /v2/reports/{reportId}/bugs/{bugId}/attachments
Загрузка вложения для бага.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `attachType` (query, integer) - Тип вложения (0 = Expected, 1 = Fact)

##### Запрос
- Content-Type: multipart/form-data
- Параметры:
  - `file` (file) - Файл для загрузки
    - Максимальный размер: 10 MB
    - Разрешенные расширения:
    ```json
    [
      ".txt",
      ".csv",
      ".json",
      ".md",
      ".png",
      ".jpg",
      ".webp",
      ".gif"
    ]
    ```
    - Максимальное кол-во файлов для одной из сторон: 10

##### Ответ
```json
{
  "id": "integer",
  "entity_id": "integer",
  "attach_type": "integer",
  "storage_key": "string",
  "storage_kind": "integer",
  "created_at": "datetime",
  "creator_user_id": "string",
  "length_bytes": "integer",
  "file_name": "string",
  "mime_type": "string",
  "has_preview": "boolean",
  "is_gzip_compressed": "boolean"
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveBugAttachmentCreate` - событие создания вложения
   - Тело события:
   ```json
   {
     "id": "integer",
     "entityId": "integer",
     "attachType": "integer",
     "createdAt": "datetime",
     "creatorUserId": "string",
     "fileName": "string",
     "hasPreview": "boolean"
   }
   ```

2. Автоматические действия:
   - Оптимизация изображений
   Она происходит после того как юзер загрузил файл (иначе 5-10сек ожидания), поэтому важно после оптимизации отловить веб-сокет событие:
   - `ReceiveBugAttachmentOptimized` - событие оптимизации вложения
   - Тело события:
   ```json
   {
     "id": "integer",
     "entityId": "integer",
     "attachType": "integer",
     "createdAt": "datetime",
     "creatorUserId": "string",
     "fileName": "string",
     "hasPreview": "boolean"
   }
   ```

#### GET /v2/reports/{reportId}/bugs/{bugId}/attachments/{id}/content
Получение содержимого вложения.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `id` (path, integer) - ID вложения

##### Ответ
- Content-Type: зависит от MIME-типа файла
- Content-Encoding: gzip (если файл сжат)
Если изображение то отображаем, если что-то другое, то по клику можно давать скачать (не будет иконки глаза, а например иконка скачивания)

#### GET /v2/reports/{reportId}/bugs/{bugId}/attachments/{id}/content/preview
Получение превью вложения (только для изображений и если есть флаг has_preview = true).

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `id` (path, integer) - ID вложения

##### Ответ
- Content-Type: image/webp

#### DELETE /v2/reports/{reportId}/bugs/{bugId}/attachments/{id}
Удаление вложения.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `id` (path, integer) - ID вложения

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveBugAttachmentDelete` - событие удаления вложения
   - Параметры события:
     - `id` (integer) - ID вложения
     - `entityId` (integer) - ID бага
     - `attachType` (integer) - тип вложения 0-receive 1-expect

### Comments API v2

#### POST /v2/reports/{reportId}/bugs/{bugId}/comments
Создание нового комментария.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага

##### Запрос
```json
{
  "text": "string" // required, min length 1, max length 2048
}
```

##### Ответ
```json
{
  "id": "integer",
  "bug_id": "integer",
  "text": "string",
  "creator_user_id": "string",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveCommentCreate` - событие создания комментария
   - Тело события:
   ```json
   {
     "id": "integer",
     "bugId": "integer",
     "text": "string",
     "creatorUserId": "string",
     "createdAt": "datetime",
     "updatedAt": "datetime"
   }
   ```

#### DELETE /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}
Удаление комментария (только своего).

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveCommentDelete` - событие удаления комментария
   - Параметры события:
     - `bugId` (integer) - ID бага
     - `commentId` (integer) - ID удаленного комментария

2. Автоматические действия:
   - Удаление всех вложений комментария

#### PUT /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}
Обновление комментария (только своего).

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария

##### Запрос
```json
{
  "text": "string" // required, min length 1, max length 2048
}
```

##### Ответ
```json
{
  "id": "integer",
  "bug_id": "integer",
  "text": "string",
  "creator_user_id": "string",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveCommentUpdate` - событие обновления комментария
   - Тело события:
   ```json
   {
     "id": "integer",
     "bugId": "integer",
     "text": "string",
     "creatorUserId": "string",
     "createdAt": "datetime",
     "updatedAt": "datetime"
   }
   ```

### Comment Attachments API v2

#### POST /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments
Загрузка вложения для комментария.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария

##### Запрос
- Content-Type: multipart/form-data
- Параметры:
  - `file` (file) - Файл для загрузки
    - Максимальный размер: 10 MB
    - Разрешенные расширения:
    ```json
    [
      ".txt",
      ".csv",
      ".json",
      ".md",
      ".png",
      ".jpg",
      ".webp",
      ".gif"
    ]
    ```
    - Максимальное кол-во файлов для комментария: 10

##### Ответ
```json
{
  "id": "integer",
  "entity_id": "integer",
  "attach_type": "integer",
  "storage_key": "string",
  "storage_kind": "integer",
  "created_at": "datetime",
  "creator_user_id": "string",
  "length_bytes": "integer",
  "file_name": "string",
  "mime_type": "string",
  "has_preview": "boolean",
  "is_gzip_compressed": "boolean"
}
```

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveCommentAttachmentCreate` - событие создания вложения
   - Тело события:
   ```json
   {
     "id": "integer",
     "entityId": "integer",
     "attachType": "integer",
     "createdAt": "datetime",
     "creatorUserId": "string",
     "fileName": "string",
     "hasPreview": "boolean"
   }
   ```

2. Автоматические действия:
   - Оптимизация изображений
   Она происходит после того как юзер загрузил файл (иначе 5-10сек ожидания), поэтому важно после оптимизации отловить веб-сокет событие:
   - `ReceiveCommentAttachmentOptimized` - событие оптимизации вложения
   - Тело события:
   ```json
   {
     "id": "integer",
     "entityId": "integer",
     "attachType": "integer",
     "createdAt": "datetime",
     "creatorUserId": "string",
     "fileName": "string",
     "hasPreview": "boolean"
   }
   ```

#### GET /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments/{id}/content
Получение содержимого вложения.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария
- `id` (path, integer) - ID вложения

##### Ответ
- Content-Type: зависит от MIME-типа файла
- Content-Encoding: gzip (если файл сжат)
Если изображение то отображаем, если что-то другое, то по клику можно давать скачать (не будет иконки глаза)

#### GET /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments/{id}/content/preview
Получение превью вложения (только для изображений и если есть флаг has_preview = true).

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария
- `id` (path, integer) - ID вложения

##### Ответ
- Content-Type: image/webp

#### DELETE /v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments/{id}
Удаление вложения.

##### Параметры
- `reportId` (path, integer) - ID отчета
- `bugId` (path, integer) - ID бага
- `commentId` (path, integer) - ID комментария
- `id` (path, integer) - ID вложения

##### Сайд-эффекты
1. WebSocket события:
   - `ReceiveCommentAttachmentDelete` - событие удаления вложения
   - Параметры события:
     - `id` (integer) - ID вложения
     - `entityId` (integer) - ID комментария
     - `attachType` (integer) - тип вложения (всегда 2 для комментариев)

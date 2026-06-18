# AI NPC Dialogue Game

Игровой прототип на Unity с системой диалогов NPC на основе нейросети.

Проект демонстрирует взаимодействие игрока с персонажами через текстовый чат. NPC отвечают с учётом своей роли, характера, текущего состояния квеста, инвентаря игрока и истории диалога.

Данный проект является дипломной работой по теме:
**«Разработка игрового приложения на Unity с системой диалогов NPC на основе нейросети»**

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge\&logo=unity\&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-68217A?style=for-the-badge\&logo=csharp\&logoColor=white)
![Python](https://img.shields.io/badge/Python-3776AB?style=for-the-badge\&logo=python\&logoColor=white)
![FastAPI](https://img.shields.io/badge/FastAPI-009688?style=for-the-badge\&logo=fastapi\&logoColor=white)
![YandexGPT](https://img.shields.io/badge/YandexGPT-Lite-blueviolet?style=for-the-badge)

---

## Описание

В проекте реализована демонстрационная игровая сцена в детективном сеттинге. Игрок помогает расследовать пропажу городской печати из архива: получает разрешение на осмотр, проходит через полицейский пост, находит улику, опрашивает свидетеля и подозреваемого.

Ответы NPC генерируются через локальный backend-сервер, который обращается к AI-провайдеру. В проекте также предусмотрен тестовый `mock`-режим, позволяющий запускать backend без API-ключа (режим не актуален под текущую сцену).

---

## Основные возможности

* Передвижение игрока по 3D-сцене.
* Взаимодействие с NPC и объектами.
* Текстовый диалог с NPC.
* Генерация ответов NPC через backend.
* Интеграция с YandexGPT Lite.
* Тестовый mock-режим без обращения к внешнему API.
* Учёт роли, характера и знаний NPC.
* Учёт квестового состояния и инвентаря.
* Система квестов.
* Инвентарь.
* Журнал квестов.
* Уведомления о получении предметов и изменении состояния квестов.
* Открытие прохода после проверки разрешения.
* Главное меню.
* Меню паузы.
* Автоматический запуск локального backend-сервера в релизной сборке.

---

## Демонстрационная сцена

Сюжетный контекст:

> В городском архиве пропала важная печать. Игрок помогает Инспектору Баттлеру провести первичное расследование, найти улику и проверить возможного подозреваемого.

Основные персонажи:

* **Инспектор Баттлер** — ведёт расследование и выдаёт поручения.
* **Полицейский** — охраняет проход к месту происшествия.
* **Мира** — свидетельница, видевшая подозрительную фигуру.
* **Ренард** — писарь архива, дающий противоречивые показания.

---

## Стек технологий

| Компонент              | Технология            |
| ---------------------- | --------------------- |
| Игровой движок         | Unity                 |
| Язык клиента           | C#                    |
| UI                     | Unity UI, TextMeshPro |
| Сетевое взаимодействие | UnityWebRequest       |
| Backend                | Python, FastAPI       |
| Сервер                 | Uvicorn               |
| AI-провайдер           | YandexGPT Lite        |
| Тестовый режим         | Mock provider         |

---

## Структура проекта

```text
/
├── Assets/                  # Unity-ассеты, сцены, скрипты, UI
├── Packages/                # Unity packages
├── ProjectSettings/         # настройки Unity-проекта
├── ai-npc-backend/          # backend-сервер
│   ├── app/
│   │   ├── providers/
│   │   │   ├── mock_provider.py
│   │   │   └── yandex_provider.py
│   │   ├── config.py
│   │   ├── main.py
│   │   └── models.py
│   ├── server_launcher.py
│   ├── requirements.txt
│   └── .env.example
└── README.md
```

---

## Настройка backend

Перейти в папку backend:

```bash
cd ai-npc-backend
```

Создать виртуальное окружение:

```bash
python -m venv .venv
```

Активировать виртуальное окружение в Windows:

```bash
.venv\Scripts\activate
```

Установить зависимости:

```bash
pip install -r requirements.txt
```

Создать файл `.env` на основе `.env.example`.
Подробнее в "Настройка .env".

Пример `.env`:

```env
AI_PROVIDER=yandex
YANDEX_API_KEY=your_api_key_here
YANDEX_FOLDER_ID=your_folder_id_here
YANDEX_MODEL=yandexgpt-lite/latest
YANDEX_BASE_URL=https://llm.api.cloud.yandex.net/v1 
BACKEND_DEBUG=false
PORT=8000
```

Запустить backend:

```bash
python server_launcher.py
```

Проверить работу backend можно по адресу:

```text
http://127.0.0.1:8000/
```

Ожидаемый ответ:

```json
{"message":"Backend is running"}
```

---

## Настройка `.env`

Для работы backend с реальной моделью YandexGPT необходимо создать файл:

```text
ai-npc-backend/.env
```

или для релизной сборки:

```text
Backend/.env
```

Пример содержимого:

```env
AI_PROVIDER=yandex
YANDEX_API_KEY=your_api_key_here
YANDEX_FOLDER_ID=your_folder_id_here
YANDEX_MODEL=yandexgpt-lite/latest
YANDEX_BASE_URL=https://llm.api.cloud.yandex.net/v1
BACKEND_DEBUG=false
PORT=8000
```

Назначение параметров:

| Параметр           | Описание                                                                                           |
| ------------------ | -------------------------------------------------------------------------------------------------- |
| `AI_PROVIDER`      | Используемый провайдер ответов. Для YandexGPT указывается `yandex`, для тестового режима — `mock`. |
| `YANDEX_API_KEY`   | API-ключ для доступа к YandexGPT.                                                                  |
| `YANDEX_FOLDER_ID` | Идентификатор каталога в Yandex Cloud.                                                             |
| `YANDEX_MODEL`     | Используемая модель, например `yandexgpt-lite/latest`.                                             |
| `YANDEX_BASE_URL`  | Адрес API Yandex AI Studio.                                                                        |
| `BACKEND_DEBUG`    | Включение или отключение отладочной информации.                                                    |
| `PORT`             | Локальный порт backend-сервера.                                                                    |

API-ключ создаётся в Yandex AI Studio / Yandex Cloud. После создания ключ необходимо сразу сохранить, так как повторно его значение обычно не отображается.

Файл `.env` содержит секретные данные и не должен публиковаться в репозитории. В GitHub добавляется только файл `.env.example` без настоящих ключей.

## Mock-режим

Для запуска без API-ключа можно указать в `.env`:

```env
AI_PROVIDER=mock
```

В этом режиме backend не обращается к YandexGPT и возвращает тестовые ответы.
Был реализован для первоначальных тестах, не актуален на данном этапе

---

## Запуск проекта в Unity

1. Открыть проект в Unity.
2. Запустить backend-сервер.
3. Открыть сцену `MainMenuScene`.
4. Нажать Play.
5. В главном меню выбрать `Новая игра`.

Основные сцены:

```text
MainMenuScene
DemoScene_Detective
```

В Build Profile порядок сцен должен быть следующим:

```text
0 — MainMenuScene
1 — DemoScene_Detective
```

---

## Управление

```text
WASD — движение
Мышь — камера
E — взаимодействие
I — инвентарь
J — журнал квестов
Esc — меню паузы
Enter — отправить сообщение в диалоге
Shift + Enter — перенос строки в диалоге
```

---

## Сборка Unity-билда

Рекомендуемые настройки Build Profile:

```text
Platform: Windows
Architecture: Intel 64-bit
Development Build: Off
Script Debugging: Off
Autoconnect Profiler: Off
```

Сборка должна включать сцены:

```text
0 — MainMenuScene
1 — DemoScene_Detective
```

После сборки структура релизной папки должна выглядеть примерно так:

```text
AI NPC Dialogue Demo/
├── AI NPC Dialogue Demo.exe
├── AI NPC Dialogue Demo_Data/
├── UnityPlayer.dll
├── UnityCrashHandler64.exe
├── MonoBleedingEdge/
├── D3D12/
└── Backend/
    ├── server.exe
    └── .env.example
```

Для локальной демонстрации с реальной моделью необходимо создать файл:

```text
Backend/.env
```

---

## Сборка backend в server.exe

Установить PyInstaller:

```bash
pip install pyinstaller
```

Собрать backend:

```bash
pyinstaller --clean --noconfirm --onefile --name server --collect-all fastapi --collect-all starlette --collect-all pydantic --collect-all uvicorn --collect-all openai --collect-all httpx server_launcher.py
```

Готовый файл появится по пути:

```text
dist/server.exe
```

Его нужно поместить в релизную папку Unity-билда:

```text
Backend/server.exe
```

---

## Безопасность

Файл `.env` содержит API-ключ и не должен публиковаться в репозитории.

В репозиторий добавляется только пример конфигурации:

```text
.env.example
```

Рекомендуемые исключения для `.gitignore`:

```gitignore
.env
.venv/
__pycache__/
*.pyc
dist/
build/
*.spec
```

---

## Ограничения

* Проект является демонстрационным прототипом.
* Сюжетная линия представлена в виде ограниченного вертикального среза.
* Сохранение и загрузка игры не реализованы.
* Расширенные настройки графики и звука не реализованы.
* Для работы с реальной моделью требуется настроенный backend и API-ключ.

---

## Статус проекта

Проект разработан в рамках дипломной работы и предназначен для демонстрации интеграции AI-диалогов NPC с игровыми механиками Unity.

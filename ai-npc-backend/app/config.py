import os
from dotenv import load_dotenv

load_dotenv()


class Settings:
    AI_PROVIDER = os.getenv("AI_PROVIDER", "mock")
    YANDEX_API_KEY = os.getenv("YANDEX_API_KEY", "")
    YANDEX_FOLDER_ID = os.getenv("YANDEX_FOLDER_ID", "")
    YANDEX_MODEL = os.getenv("YANDEX_MODEL", "")
    YANDEX_BASE_URL = os.getenv("YANDEX_BASE_URL", "")
    BACKEND_DEBUG = os.getenv("BACKEND_DEBUG", "false").lower() == "true"


settings = Settings()
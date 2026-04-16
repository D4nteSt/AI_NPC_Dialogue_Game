from openai import AsyncOpenAI
from app.config import settings


class YandexProvider:
    def __init__(self):
        self.client = AsyncOpenAI(
            api_key=settings.YANDEX_API_KEY,
            base_url=settings.YANDEX_BASE_URL,
            project=settings.YANDEX_FOLDER_ID,
        )

    def _build_model_uri(self) -> str:
        return f"gpt://{settings.YANDEX_FOLDER_ID}/{settings.YANDEX_MODEL}"

    async def generate_reply(
            self,
            prompt: str,
            npc_name: str = "",
            temperature: float = 0.3,
            max_output_tokens: int = 180
    ) -> tuple[str, dict]:
        response = await self.client.responses.create(
            model=self._build_model_uri(),
            temperature=temperature,
            instructions="",
            input=prompt,
            max_output_tokens=max_output_tokens,
        )

        reply_text = getattr(response, "output_text", None)
        if not reply_text:
            reply_text = "Модель не вернула текст ответа."

        reply_text = self._normalize_reply(reply_text, npc_name)

        usage = {
            "inputTokens": getattr(response.usage, "input_tokens", 0) if getattr(response, "usage", None) else 0,
            "outputTokens": getattr(response.usage, "output_tokens", 0) if getattr(response, "usage", None) else 0,
        }

        print("NORMALIZED REPLY:", reply_text)

        return reply_text, usage

    def _normalize_reply(self, text: str, npc_name: str = "") -> str:
        if not text:
            return ""

        cleaned = text.strip()

        # Убираем имя говорящего
        prefixes = []
        if npc_name:
            prefixes.extend([
                f"{npc_name}:",
                f"{npc_name} :",
                f"{npc_name.lower()}:",
                f"{npc_name.lower()} :",
            ])

        prefixes.extend(["NPC:", "Персонаж:", "Ответ:", "Реплика:"])

        changed = True
        while changed:
            changed = False
            cleaned = cleaned.strip()

            for prefix in prefixes:
                if cleaned.startswith(prefix):
                    cleaned = cleaned[len(prefix):].strip()
                    changed = True

            while cleaned.startswith(("—", "–", "-", "— ", "– ", "- ")):
                cleaned = cleaned[1:].strip()
                changed = True

            quote_pairs = [
                ("«", "»"),
                ('"', '"'),
                ("“", "”"),
                ("„", "“"),
                ("'", "'"),
            ]

            for left, right in quote_pairs:
                if cleaned.startswith(left) and cleaned.endswith(right) and len(cleaned) >= 2:
                    cleaned = cleaned[1:-1].strip()
                    changed = True

        return cleaned.strip()
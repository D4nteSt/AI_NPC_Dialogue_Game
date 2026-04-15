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

        usage = {
            "inputTokens": getattr(response.usage, "input_tokens", 0) if getattr(response, "usage", None) else 0,
            "outputTokens": getattr(response.usage, "output_tokens", 0) if getattr(response, "usage", None) else 0,
        }

        reply_text = self._strip_speaker_prefix(reply_text, npc_name)
        reply_text = self._strip_outer_quotes(reply_text)

        return reply_text, usage

    def _strip_speaker_prefix(self, text: str, npc_name: str = "") -> str:
        if not text:
            return ""

        cleaned = text.strip()

        prefixes = []
        if npc_name:
            prefixes.extend([
                f"{npc_name}:",
                f"{npc_name} :",
                f"{npc_name.lower()}:",
                f"{npc_name.lower()} :",
            ])

        prefixes.extend(["NPC:", "Персонаж:", "Ответ:", "Реплика:"])

        for prefix in prefixes:
            if cleaned.startswith(prefix):
                cleaned = cleaned[len(prefix):].strip()
                break

        return cleaned

    def _strip_outer_quotes(self, text: str) -> str:
        if not text:
            return ""

        cleaned = text.strip()

        if cleaned.startswith("«") and cleaned.endswith("»"):
            cleaned = cleaned[1:-1].strip()

        if cleaned.startswith('"') and cleaned.endswith('"'):
            cleaned = cleaned[1:-1].strip()

        return cleaned
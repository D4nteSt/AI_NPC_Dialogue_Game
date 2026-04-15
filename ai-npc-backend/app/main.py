import time
from fastapi import FastAPI
from app.models import (
    NpcDialogueRequest,
    NpcDialogueResponse,
    UsageData,
    DebugData,
)
from app.config import settings
from app.providers.mock_provider import build_mock_reply
from app.providers.yandex_provider import YandexProvider

app = FastAPI(title="AI NPC Backend")

yandex_provider = YandexProvider() if settings.AI_PROVIDER == "yandex" else None


@app.get("/")
async def root():
    return {"message": "Backend is running"}


@app.post("/npc-dialogue", response_model=NpcDialogueResponse)
async def npc_dialogue(request: NpcDialogueRequest):
    start_time = time.time()

    try:
        if settings.AI_PROVIDER == "yandex":
            temperature = request.options.temperature if request.options else 0.3
            max_output_tokens = request.options.maxOutputTokens if request.options else 180

            reply_text, usage_dict = await yandex_provider.generate_reply(
                prompt=request.prompt,
                npc_name=request.npcName,
                temperature=temperature,
                max_output_tokens=max_output_tokens,
            )

            provider_name = "yandex"
            model_name = settings.YANDEX_MODEL
        else:
            reply_text = build_mock_reply(request)
            usage_dict = {"inputTokens": 0, "outputTokens": 0}
            provider_name = "mock"
            model_name = request.options.model if request.options and request.options.model else "local-test"

        latency_ms = int((time.time() - start_time) * 1000)

        debug_data = None
        if request.options and request.options.debug:
            debug_data = DebugData(
                promptUsed=request.prompt,
                latencyMs=latency_ms,
            )

        return NpcDialogueResponse(
            success=True,
            replyText=reply_text,
            provider=provider_name,
            model=model_name,
            usage=UsageData(**usage_dict),
            debug=debug_data,
            errorMessage=None,
        )

    except Exception as e:
        latency_ms = int((time.time() - start_time) * 1000)

        debug_data = None
        if request.options and request.options.debug:
            debug_data = DebugData(
                promptUsed=request.prompt,
                latencyMs=latency_ms,
            )

        return NpcDialogueResponse(
            success=False,
            replyText="",
            provider=settings.AI_PROVIDER,
            model=settings.YANDEX_MODEL if settings.AI_PROVIDER == "yandex" else "local-test",
            usage=UsageData(inputTokens=0, outputTokens=0),
            debug=debug_data,
            errorMessage=str(e),
        )
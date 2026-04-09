import time
from fastapi import FastAPI
from app.models import (
    NpcDialogueRequest,
    NpcDialogueResponse,
    UsageData,
    DebugData,
)

app = FastAPI(title="AI NPC Backend")


@app.get("/")
async def root():
    return {"message": "Backend is running"}


@app.post("/npc-dialogue", response_model=NpcDialogueResponse)
async def npc_dialogue(request: NpcDialogueRequest):
    start_time = time.time()

    reply_text = build_mock_reply(request)

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
        provider=(request.options.provider if request.options else "mock"),
        model=(request.options.model if request.options and request.options.model else "local-test"),
        usage=UsageData(inputTokens=0, outputTokens=0),
        debug=debug_data,
        errorMessage=None,
    )


def build_mock_reply(request: NpcDialogueRequest) -> str:
    npc_name = request.npcName.lower()
    player_message = request.playerMessage.lower().strip()
    quest_status = (request.dialogueContext.questStatus or "").strip()

    is_lea = "лея" in npc_name
    is_old_man = "старик" in npc_name

    if is_lea:
        if quest_status == "Completed":
            return "Если это и правда та самая вещь, лучше не держать ее при себе дольше, чем нужно."
        if quest_status == "TurnedIn":
            return "Теперь здесь будто стало тише. Надеюсь, так и останется."
        if "что" in player_message or "какое" in player_message:
            return "Речь о старой реликвии из руин. Я бы не трогала ее без нужды, но оставлять ее там тоже нельзя."
        return "Здесь редко что-то происходит без последствий. Если ввязался в это, будь внимателен."

    if is_old_man:
        if quest_status == "Completed":
            return "Да... если предмет у тебя, не медли. Старым вещам не стоит долго менять руки."
        if quest_status == "TurnedIn":
            return "Ты исполнил просьбу, и я этого не забуду."
        if "что" in player_message or "какое" in player_message:
            return "Есть вещь, что не должна покоиться среди руин. Найди ее и принеси мне."
        return "Не всякая тропа ведет прямо, путник. Но эту дорогу тебе все же придется пройти."

    return "Я услышал тебя. Скажи яснее, если ждешь ясного ответа."
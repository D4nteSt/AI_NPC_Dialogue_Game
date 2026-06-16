from app.models import NpcDialogueRequest

# Этот файл устарел, здесь используются старые тестовые фразы с первоначальным сеттингом
# Если хотите проверить работоспособность, измените значения имен npc и желаемые реплики
# В текущей сцене они работать не будут

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
from typing import List, Optional
from pydantic import BaseModel


class DialogueContextData(BaseModel):
    npcRole: Optional[str] = None
    npcPersonality: Optional[str] = None
    npcSpeechStyle: Optional[str] = None
    npcBackstory: Optional[str] = None
    npcLocationContext: Optional[str] = None
    npcKnowledge: Optional[str] = None
    npcUnknowns: Optional[str] = None
    npcMotivation: Optional[str] = None
    npcSecret: Optional[str] = None
    npcAttitudeToPlayer: Optional[str] = None
    npcCurrentEmotionalState: Optional[str] = None
    npcConversationTendency: Optional[str] = None

    questId: Optional[str] = None
    questName: Optional[str] = None
    questDescription: Optional[str] = None
    questStatus: Optional[str] = None

    inventoryItems: List[str] = []
    dialogueHistory: List[str] = []


class RequestOptions(BaseModel):
    provider: Optional[str] = "mock"
    model: Optional[str] = None
    temperature: Optional[float] = 0.7
    maxOutputTokens: Optional[int] = 180
    debug: Optional[bool] = False


class NpcDialogueRequest(BaseModel):
    npcId: str
    npcName: str
    isQuestGiver: bool = False
    playerMessage: str

    dialogueContext: DialogueContextData
    prompt: Optional[str] = None
    options: Optional[RequestOptions] = None


class UsageData(BaseModel):
    inputTokens: int = 0
    outputTokens: int = 0


class DebugData(BaseModel):
    promptUsed: Optional[str] = None
    latencyMs: Optional[int] = None


class NpcDialogueResponse(BaseModel):
    success: bool
    replyText: str
    provider: str = "mock"
    model: str = "local-test"
    usage: UsageData = UsageData()
    debug: Optional[DebugData] = None
    errorMessage: Optional[str] = None
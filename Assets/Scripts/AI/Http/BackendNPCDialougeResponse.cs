using System;

[Serializable]
public class BackendNpcDialogueResponse
{
    public bool success;
    public string replyText;
    public string provider;
    public string model;
    public BackendUsageData usage;
    public BackendDebugData debug;
    public string errorMessage;
}

[Serializable]
public class BackendUsageData
{
    public int inputTokens;
    public int outputTokens;
}

[Serializable]
public class BackendDebugData
{
    public string promptUsed;
    public int latencyMs;
}
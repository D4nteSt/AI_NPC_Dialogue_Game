using System.Collections.Generic;

public class DialogueOutcomeExecutionResult
{
    private readonly List<string> summaries = new List<string>();

    public IReadOnlyList<string> Summaries => summaries;

    public bool HasSummary => summaries.Count > 0;

    public void AddSummary(string summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
            return;

        summaries.Add(summary.Trim());
    }

    public string ToPromptText()
    {
        if (!HasSummary)
            return string.Empty;

        return string.Join("\n", summaries);
    }
}
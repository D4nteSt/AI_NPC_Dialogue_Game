using System.Text;
using UnityEngine;

public class StubNPCResponseGenerator : MonoBehaviour, INPCResponseGenerator
{
    public string GenerateResponse(DialogueContext context)
    {
        if (context == null)
            return "я не пон€л, что произошло.";

        StringBuilder builder = new StringBuilder();

        builder.Append("я услышал теб€");

        if (!string.IsNullOrWhiteSpace(context.playerMessage))
        {
            builder.Append(": \"");
            builder.Append(context.playerMessage);
            builder.Append("\". ");
        }

        if (!string.IsNullOrWhiteSpace(context.questStatus))
        {
            builder.Append("—осто€ние квеста: ");
            builder.Append(context.questStatus);
            builder.Append(". ");
        }

        if (context.inventoryItems != null && context.inventoryItems.Count > 0)
        {
            builder.Append("я вижу, что у теб€ есть: ");
            builder.Append(string.Join(", ", context.inventoryItems));
            builder.Append(". ");
        }

        if (!string.IsNullOrWhiteSpace(context.npcPersonality))
        {
            builder.Append("ћой характер: ");
            builder.Append(context.npcPersonality);
        }

        return builder.ToString();
    }
}
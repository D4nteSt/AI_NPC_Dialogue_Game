using UnityEngine;

public class SceneDialogueContext : MonoBehaviour
{
    [TextArea(3, 8)]
    [SerializeField] private string worldDescription;

    [TextArea(3, 8)]
    [SerializeField] private string knownCharacters;

    [TextArea(3, 8)]
    [SerializeField] private string worldRules;

    public string WorldDescription => worldDescription;
    public string KnownCharacters => knownCharacters;
    public string WorldRules => worldRules;
}
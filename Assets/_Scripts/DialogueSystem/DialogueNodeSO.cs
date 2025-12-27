using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNodeSO", menuName = "Scriptable Objects/DialogueNodeSO")]
public class DialogueNodeSO : ScriptableObject
{
    public string Name;
    public string Text;
    public float charAppearTime = 0.05f;
    public Color NameColor;
}

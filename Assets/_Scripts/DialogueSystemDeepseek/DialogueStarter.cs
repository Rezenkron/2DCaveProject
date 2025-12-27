using System.Collections.Generic;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public List<DialogueNode> testDialogue;

    private void Start()
    {
         DialogueSystem.Instance.StartDialogue(testDialogue);
    }
}
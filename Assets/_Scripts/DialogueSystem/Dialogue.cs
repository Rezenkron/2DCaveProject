using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private bool initOnStart = false;
    [SerializeField] private DialogueNodeSO[] nodes;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI text;

    public event Action OnDialogueStart;
    public event Action OnDialogueEnd;

    private int index = 0;
    private bool isTyping = false;
    private bool isActive = false;

    private void Start()
    {
        if (initOnStart) StartDialogue();
    }

    private void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) InterruptTyping();
            else NextNode();
        }
    }

    private void NextNode()
    {
        if (nodes == null || index >= nodes.Length)
        {
            StopDialogue();
            return;
        }
        
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        string hexNameColor = "#" + ColorUtility.ToHtmlStringRGB(nodes[index].NameColor);
        nameText.text = $"<color={hexNameColor}>{nodes[index].Name}</color>";
        text.text = nodes[index].Text;
        text.maxVisibleCharacters = 0;

        for (int i = 0; i < text.text.Length; i++)
        {
            isTyping = true;
            text.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(nodes[index].charAppearTime);
        }
        isTyping = false;
        index++;
    }

    private void InterruptTyping()
    {
        StopAllCoroutines();
        text.maxVisibleCharacters = text.text.Length;
        isTyping = false;
        index++;
    }

    public void StartDialogue(bool reset = true)
    {
        isActive = true;

        if (nodes == null)
        {
            Debug.LogError("Dialogue nodes are missing!");
            StopDialogue();
            return;
        }

        if (reset) index = 0;

        dialoguePanel.SetActive(true);
        OnDialogueStart?.Invoke();
        NextNode();

    }

    public void StopDialogue()
    {
        StopAllCoroutines();
        isActive = false;
        dialoguePanel.SetActive(false);
        index = 0;
        OnDialogueEnd?.Invoke();
    }
}

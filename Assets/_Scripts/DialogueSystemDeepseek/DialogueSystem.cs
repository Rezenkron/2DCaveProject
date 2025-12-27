using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerIconImage;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private GameObject optionButtonPrefab;

    [Header("Settings")]
    [SerializeField] private bool skipOnClick = true;
    [SerializeField] private Color defaultNameColor = Color.yellow;
    [SerializeField] private Color defaultTextColor = Color.white;
    [SerializeField] private float defaultCharAppearTime = 0.05f; // Время появления одного символа по умолчанию

    private List<DialogueNode> currentDialogue;
    private int currentNodeIndex;
    private bool isDialogueActive;
    private bool isTextAnimating;
    private Coroutine textAnimationCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && skipOnClick && Input.GetMouseButtonDown(0))
        {
            if (isTextAnimating)
                SkipTextAnimation();
            else if (currentNodeIndex < currentDialogue.Count &&
                   (currentDialogue[currentNodeIndex].options == null ||
                    currentDialogue[currentNodeIndex].options.Count == 0))
                GoToNextNode();
        }
    }

    public void StartDialogue(List<DialogueNode> dialogue)
    {
        if (dialogue == null || dialogue.Count == 0) return;

        currentDialogue = dialogue;
        currentNodeIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        DisplayCurrentNode();
    }

    private void DisplayCurrentNode()
    {
        if (currentNodeIndex < 0 || currentNodeIndex >= currentDialogue.Count)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = currentDialogue[currentNodeIndex];

        // Установка имени персонажа и цвета
        speakerNameText.text = node.speakerName;
        speakerNameText.color = node.nameColor.a > 0 ? node.nameColor : defaultNameColor;

        // Установка иконки персонажа
        if (node.speakerIcon != null)
        {
            speakerIconImage.sprite = node.speakerIcon;
            speakerIconImage.gameObject.SetActive(true);
        }
        else
        {
            speakerIconImage.gameObject.SetActive(false);
        }

        // Очистка предыдущих вариантов ответа
        ClearOptions();

        // Установка текста диалога и цвета
        dialogueText.text = node.dialogueText;
        dialogueText.color = node.textColor.a > 0 ? node.textColor : defaultTextColor;
        dialogueText.maxVisibleCharacters = 0;

        // Запуск анимации текста
        StartTextAnimation(node.charAppearTime > 0 ? node.charAppearTime : defaultCharAppearTime);

        // Создание кнопок вариантов ответа (если анимация завершена)
        if (!isTextAnimating)
            CreateOptionButtons(node);

        // Воспроизведение аудио реплики
        PlayVoiceClip(node);
    }

    private void StartTextAnimation(float charAppearTime)
    {
        if (textAnimationCoroutine != null)
            StopCoroutine(textAnimationCoroutine);

        textAnimationCoroutine = StartCoroutine(AnimateText(charAppearTime));
    }

    private IEnumerator AnimateText(float charAppearTime)
    {
        isTextAnimating = true;
        int totalCharacters = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charAppearTime);
        }

        isTextAnimating = false;

        // После завершения анимации создаем кнопки (если есть)
        CreateOptionButtons(currentDialogue[currentNodeIndex]);
    }

    private void SkipTextAnimation()
    {
        if (textAnimationCoroutine != null)
            StopCoroutine(textAnimationCoroutine);

        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        isTextAnimating = false;

        // После пропуска анимации сразу создаем кнопки (если есть)
        CreateOptionButtons(currentDialogue[currentNodeIndex]);
    }

    private void CreateOptionButtons(DialogueNode node)
    {
        if (node.options == null || node.options.Count == 0) return;

        foreach (DialogueOption option in node.options)
        {
            if (option.requiresCondition && !CheckCondition(option.conditionName))
                continue;

            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = option.optionText;

            if (option.endsDialogue)
                button.onClick.AddListener(EndDialogue);
            else
                button.onClick.AddListener(() => {
                    currentNodeIndex = option.nextNodeIndex;
                    DisplayCurrentNode();
                });
        }
    }

    private void PlayVoiceClip(DialogueNode node)
    {
        if (node.voiceClip != null)
            AudioSource.PlayClipAtPoint(node.voiceClip, Camera.main.transform.position);
    }

    private void GoToNextNode()
    {
        currentNodeIndex++;
        DisplayCurrentNode();
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
    }

    private bool CheckCondition(string conditionName)
    {
        // Реализуйте проверку условий по необходимости
        return true;
    }

    public void EndDialogue()
    {
        if (textAnimationCoroutine != null)
            StopCoroutine(textAnimationCoroutine);

        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        currentDialogue = null;
    }

    public bool IsDialogueActive() => isDialogueActive;
}

[System.Serializable]
public class DialogueNode
{
    public string speakerName;
    [TextArea(3, 10)] public string dialogueText;
    public List<DialogueOption> options;
    public Sprite speakerIcon;
    public AudioClip voiceClip;

    [Header("Colors")]
    public Color nameColor = new Color(0, 0, 0, 0);
    public Color textColor = new Color(0, 0, 0, 0);

    [Header("Animation Settings")]
    [Tooltip("Время появления одного символа (0 для значения по умолчанию)")]
    public float charAppearTime = 0f;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public int nextNodeIndex;
    public bool requiresCondition;
    public string conditionName;
    public bool endsDialogue;
}
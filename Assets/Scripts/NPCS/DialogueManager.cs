using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicesPanel;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    [Header("Visual Cue Management")]
    [SerializeField] private GameObject visualCue;

    [Header("Objects to Disappear")]
    [SerializeField] private List<GameObject> objectsToDisappear;

    private Story currentStory;
    private bool dialogueActive = false;
    private float skipTimer = 0f;
    private const float skipThreshold = 2f;

    private int currentChoiceIndex = 0;

    public delegate void SpecialPhraseHandler(string phrase);
    public event SpecialPhraseHandler OnSpecialPhraseDetected;

    public delegate void DialogueEndHandler();
    public event DialogueEndHandler OnDialogueEnd;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);

        if (visualCue != null)
            visualCue.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    public void StartInkDialogue(Story story, Transform targetTransform)
    {
        currentStory = story;

        dialoguePanel.SetActive(true);
        dialogueActive = true;

        ShowNextSentence();
    }

    private void Update()
    {
        if (dialogueActive && !choicesPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                skipTimer = 0f;
                ShowNextSentence();
            }

            if (Input.GetKey(KeyCode.E))
            {
                skipTimer += Time.deltaTime;

                if (skipTimer >= skipThreshold)
                {
                    SkipUntilChoiceOrEnd();
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                skipTimer = 0f;
            }
        }

        if (dialogueActive && choicesPanel.activeSelf)
        {
            HandleChoiceSelection();
        }
    }

    private void ShowNextSentence()
    {
        if (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();
            dialogueText.text = nextLine.Trim();
            AutoSizeText(dialogueText);

            // Check for specific phrases in the dialogue
            CheckForSpecialPhrases(nextLine);
        }
        else if (currentStory.currentChoices.Count > 0)
        {
            ShowChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    private void SkipUntilChoiceOrEnd()
    {
        while (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();
            dialogueText.text = nextLine.Trim();
            AutoSizeText(dialogueText);

            // Check for specific text in the dialogue during skip
            CheckForSpecialPhrases(nextLine);
        }

        if (currentStory.currentChoices.Count > 0)
        {
            ShowChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    private void CheckForSpecialPhrases(string line)
    {
        if (line.Contains("*Azis gives Kartoni coffee.*"))
        {
            AbilityManager.Instance.UnlockDash();
            OnSpecialPhraseDetected?.Invoke("*Azis gives Kartoni coffee.*");
        }
        if (line.Contains("DJ Damyan: Any time!"))
        {
            Debug.Log("Glide ability unlocked via special phrase!");
            AbilityManager.Instance.UnlockGlide();
            OnSpecialPhraseDetected?.Invoke("DJ Damyan: Any time!");
        }
        else if (line.Contains("Fiki: Ah, right about that. I managed to find your boots—they were at the nightclub you sung at last night."))
        {
            Debug.Log("Double Jump ability unlocked via special phrase!");
            AbilityManager.Instance.UnlockDoubleJump();
            OnSpecialPhraseDetected?.Invoke("Fiki: Ah, right about that.");
        }
        else if (line.Contains("Koceto: Well, it’s easy. If a branch is blocking your way, just give it a nice whack with the guitar and you should be able to pass right through."))
        {
            Debug.Log("Break ability unlocked via special phrase!");
            AbilityManager.Instance.UnlockBreak();
            OnSpecialPhraseDetected?.Invoke("Koceto: Well, it’s easy.");
        }
        if (line.Contains("DJ Damyan: Any time!") || line.Contains("Fiki: Ah, right about that.") || line.Contains("Koceto: Well, it’s easy."))
        {
            MakeObjectsDisappear();
        }
    }

    private void MakeObjectsDisappear()
    {
        foreach (GameObject obj in objectsToDisappear)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"{obj.name} has been made inactive.");
            }
        }
    }

    private void ShowChoices()
    {
        choicesPanel.SetActive(true);

        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < currentStory.currentChoices.Count)
            {
                choiceTexts[i].gameObject.SetActive(true);
                choiceTexts[i].text = currentStory.currentChoices[i].text;
                AutoSizeText(choiceTexts[i]);
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }

        HighlightChoice(0);
    }

    private void HighlightChoice(int choiceIndex)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i == choiceIndex)
            {
                choiceTexts[i].text = $"<color=#00FF00><b>{currentStory.currentChoices[i].text}</b></color>";
            }
            else
            {
                choiceTexts[i].text = currentStory.currentChoices[i].text;
            }
        }
    }

    private void HandleChoiceSelection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentChoiceIndex = (currentChoiceIndex - 1 + currentStory.currentChoices.Count) % currentStory.currentChoices.Count;
            HighlightChoice(currentChoiceIndex);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentChoiceIndex = (currentChoiceIndex + 1) % currentStory.currentChoices.Count;
            HighlightChoice(currentChoiceIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeChoice(currentChoiceIndex);
        }
    }

    private void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);

        foreach (string tag in currentStory.currentTags)
        {
            HandleTags(tag);
        }

        choicesPanel.SetActive(false);
        currentChoiceIndex = 0;

        ShowNextSentence();
    }

    private void HandleTags(string tag)
    {
        Debug.Log($"Unhandled tag: {tag}");
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false;

        if (visualCue != null)
        {
            visualCue.SetActive(false);
        }

        OnDialogueEnd?.Invoke();
    }

    private void AutoSizeText(TextMeshProUGUI textElement)
    {
        textElement.enableAutoSizing = true;
        textElement.fontSizeMax = 50;
        textElement.fontSizeMin = 20;
        textElement.ForceMeshUpdate();
    }
}

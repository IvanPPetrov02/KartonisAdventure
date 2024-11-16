using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel; // The main dialogue panel
    [SerializeField] private TextMeshProUGUI dialogueText; // Dialogue text box
    [SerializeField] private GameObject choicesPanel; // Choices panel
    [SerializeField] private TextMeshProUGUI[] choiceTexts; // Choice options

    [Header("Visual Cue Management")]
    [SerializeField] private GameObject visualCue; // Reference to the NPC's visual cue

    [Header("Existing Item to Reveal")]
    [SerializeField] private GameObject shoesObject; // Optional object to reveal (example)

    private Story currentStory; // Holds the Ink story runtime instance
    private bool dialogueActive = false; // Tracks if dialogue is ongoing
    private bool skipMode = false; // Tracks if dialogue is being skipped
    private float skipTimer = 0f; // Tracks how long the skip key is held
    private const float skipThreshold = 3f; // Time in seconds to skip dialogue entirely

    private int currentChoiceIndex = 0; // Current choice being highlighted
    private Health playerHealth; // Reference to the player's health (example)

    public delegate void DialogueEndHandler();
    public event DialogueEndHandler OnDialogueEnd;

    private void Awake()
    {
        // Ensure dialogue and choices panels are initially hidden
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);

        // Find the player's health component (if used)
        playerHealth = FindObjectOfType<Health>();
        if (playerHealth == null)
            Debug.LogError("Player Health component not found.");

        // Ensure any interactable objects are initially hidden
        if (shoesObject != null)
            shoesObject.SetActive(false);

        // Ensure visual cue starts inactive
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

        // Activate dialogue panel and start dialogue
        dialoguePanel.SetActive(true);
        dialogueActive = true;

        // Show the first sentence
        ShowNextSentence();
    }

    private void ShowNextSentence()
    {
        if (currentStory.canContinue)
        {
            // Display the next line of dialogue
            string nextLine = currentStory.Continue();
            dialogueText.text = nextLine.Trim();
            AutoSizeText(dialogueText);
        }
        else if (currentStory.currentChoices.Count > 0)
        {
            // If choices are available, display them
            ShowChoices();
        }
        else
        {
            // End the dialogue if no more lines or choices are available
            EndDialogue();
        }
    }

    private void ShowChoices()
    {
        choicesPanel.SetActive(true);

        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < currentStory.currentChoices.Count)
            {
                // Display each choice text
                choiceTexts[i].gameObject.SetActive(true);
                choiceTexts[i].text = currentStory.currentChoices[i].text;
                AutoSizeText(choiceTexts[i]);
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }

        HighlightChoice(0); // Highlight the first choice by default
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

    private void Update()
    {
        if (dialogueActive && !choicesPanel.activeSelf) // Skip logic only when choices aren't visible
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                skipTimer = 0f; // Reset timer on key press
                ShowNextSentence(); // Manually advance the dialogue
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
                skipTimer = 0f; // Reset timer when key is released
            }
        }

        if (dialogueActive && choicesPanel.activeSelf) // Handle choice navigation
        {
            HandleChoiceSelection();
        }
    }

    private void SkipUntilChoiceOrEnd()
    {
        while (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();
            dialogueText.text = nextLine.Trim();
            AutoSizeText(dialogueText);
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
        switch (tag)
        {
            case "SwordGiven":
                Debug.Log("Player received the sword!");
                break;
            case "TakeDamage":
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                    Debug.Log("Player took damage.");
                }
                break;
            default:
                Debug.Log($"Unhandled tag: {tag}");
                break;
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false); // Hide dialogue panel
        dialogueActive = false;

        // Deactivate the visual cue
        if (visualCue != null)
        {
            visualCue.SetActive(false);
        }

        // Trigger the OnDialogueEnd event
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

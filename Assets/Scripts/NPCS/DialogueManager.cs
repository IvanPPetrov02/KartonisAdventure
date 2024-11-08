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

    [Header("Existing Item to Reveal")]
    [SerializeField] private GameObject shoesObject; // Assign the shoes object in the scene

    private Story currentStory;
    private bool dialogueActive = false;
    private Transform currentNPC;
    private Camera mainCamera;

    private int currentChoiceIndex = 0; // Tracks the currently selected choice
    private Health playerHealth; // Reference to the player's health system

    private void Awake()
    {
        mainCamera = Camera.main;
        choicesPanel.SetActive(false);

        // Find the Player's Health component
        playerHealth = FindObjectOfType<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("Player Health component not found in the scene.");
        }
        else
        {
            Debug.Log("Player Health component successfully found.");
        }

        // Ensure shoesObject is initially invisible
        if (shoesObject != null)
        {
            shoesObject.SetActive(false);
        }
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    public void StartInkDialogue(Story story, Transform targetTransform)
    {
        currentStory = story;

        // Position the dialogue panel above the target
        Vector3 targetPosition = targetTransform.position;
        targetPosition.y += 2.0f; // Offset above the target
        dialoguePanel.transform.position = Camera.main.WorldToScreenPoint(targetPosition);

        dialoguePanel.SetActive(true);
        dialogueActive = true;

        StartCoroutine(ShowNextSentenceWithDelay());
    }




    private IEnumerator ShowNextSentenceWithDelay()
    {
        while (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();
            dialogueText.text = nextLine.Trim();
            AutoSizeText(dialogueText);

            // Check for the word "SLAP" in the current dialogue line
            if (nextLine.Contains("SLAP"))
            {
                Debug.Log("SLAP detected in dialogue!");
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                    Debug.Log($"Player took 1 damage. Current health: {playerHealth.currentHealth}");
                }
                else
                {
                    Debug.LogError("Player Health component is null!");
                }
            }

            // Check for "jump double" in the dialogue
            if (nextLine.Contains("jump double"))
            {
                Debug.Log("Double Jump detected! Making shoes visible...");
                RevealShoes();
            }

            yield return new WaitForSeconds(2f);
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

    private void ShowChoices()
    {
        // Keep the main dialogue text visible
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

        HighlightChoice(0); // Highlight the first choice by default
    }

    private void HighlightChoice(int choiceIndex)
    {
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i == choiceIndex)
            {
                choiceTexts[i].text = $"<u>{currentStory.currentChoices[i].text}</u>";
            }
            else
            {
                choiceTexts[i].text = currentStory.currentChoices[i].text;
            }
        }
    }

    private void Update()
    {
        if (currentNPC != null)
        {
            Vector3 npcScreenPosition = mainCamera.WorldToScreenPoint(currentNPC.position);
            npcScreenPosition.y += 150;
            dialoguePanel.transform.position = npcScreenPosition;

            Vector3 choicesPosition = dialoguePanel.transform.position;
            choicesPosition.y -= 100; // Offset for the choices panel
            choicesPanel.transform.position = choicesPosition;
        }

        if (dialogueActive && choicesPanel.activeSelf)
        {
            HandleChoiceSelection();
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            MakeChoice(currentChoiceIndex);
        }
    }

    private void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);

        foreach (string tag in currentStory.currentTags)
        {
            Debug.Log($"Tag found: {tag}");
        }

        choicesPanel.SetActive(false);
        currentChoiceIndex = 0;

        StartCoroutine(ShowNextSentenceWithDelay());
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false;
        currentNPC = null;
    }

    private void AutoSizeText(TextMeshProUGUI textElement)
    {
        textElement.enableAutoSizing = true;
        textElement.fontSizeMax = 50;
        textElement.fontSizeMin = 20;
        textElement.ForceMeshUpdate();
    }

    private void RevealShoes()
    {
        if (shoesObject != null)
        {
            shoesObject.SetActive(true); // Make the shoes visible
        }
        else
        {
            Debug.LogError("Shoes object not assigned!");
        }
    }
}

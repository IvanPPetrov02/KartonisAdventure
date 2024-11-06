using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;
    private bool hasTalked; // Flag to track if the NPC has been interacted with

    private void Awake()
    {
        playerInRange = false;
        hasTalked = false; // NPC starts as "not talked to"
        visualCue.SetActive(false);
    }

    private void Update()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (playerInRange && !hasTalked && !dialogueManager.IsDialogueActive())
        {
            if (!visualCue.activeSelf)
            {
                visualCue.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                visualCue.SetActive(false); // Hide the visual cue
                dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), transform);
                hasTalked = true; // Mark as "talked to"
            }
        }
        else
        {
            if (visualCue.activeSelf)
            {
                visualCue.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
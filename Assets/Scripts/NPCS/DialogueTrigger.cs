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
    private bool hasTalked;

    private void Awake()
    {
        playerInRange = false;
        hasTalked = false;
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
                visualCue.SetActive(false);
                dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), transform);
                hasTalked = true;
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
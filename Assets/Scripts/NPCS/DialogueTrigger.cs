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
    private Collider2D npcCollider;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);

        // Get the NPC's collider
        npcCollider = GetComponent<Collider2D>();
        if (npcCollider == null)
        {
            Debug.LogError("No Collider2D found on the NPC. Please add one.");
        }
    }

    private void Update()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (playerInRange && !dialogueManager.IsDialogueActive())
        {
            if (!visualCue.activeSelf)
            {
                visualCue.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                visualCue.SetActive(false);
                dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), transform);

                // Permanently remove the NPC's collider
                RemoveCollider();
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

    private void RemoveCollider()
    {
        if (npcCollider != null)
        {
            Destroy(npcCollider); // Permanently remove the collider
            npcCollider = null;  // Ensure we don't reference the destroyed collider
        }
    }
}

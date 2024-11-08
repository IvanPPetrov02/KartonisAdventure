using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform; // Reference to the player

    private Collider2D npcCollider;

    private void Awake()
    {
        // Get the collider for this object
        npcCollider = GetComponent<Collider2D>();
        if (npcCollider == null)
        {
            Debug.LogError($"No Collider2D found on {gameObject.name}. Please add one.");
        }

        // Find the player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player not found in the scene. Make sure your player is tagged 'Player'.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered the trigger of {gameObject.name}.");

            // Trigger dialogue immediately
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (!dialogueManager.IsDialogueActive())
        {
            Transform dialoguePosition = transform;

            // If this is an empty trigger, use the player's position for dialogue
            if (npcCollider == null)
            {
                dialoguePosition = playerTransform;
            }

            dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), dialoguePosition);

            // Remove the collider after interaction
            RemoveCollider();
        }
    }

    private void RemoveCollider()
    {
        if (npcCollider != null)
        {
            Debug.Log($"Removing collider from {gameObject.name}.");
            Destroy(npcCollider); // Permanently remove the collider
            npcCollider = null;  // Ensure we don't reference the destroyed collider
        }
    }
}

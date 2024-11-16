using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue; // Assign the visual cue object in the inspector

    private Rigidbody2D playerRigidbody;
    private Collider2D npcCollider; // Reference to the NPC's collider
    private bool hasInteracted = false;
    private bool playerInRange = false; // Tracks if the player is inside the trigger area

    // Variables to store the player's original state
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private Vector3 originalPlayerScale;
    private Vector2 originalPlayerVelocity;
    private float originalPlayerGravityScale;

    private void Awake()
    {
        npcCollider = GetComponent<Collider2D>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogError("Player not found in the scene. Make sure your player is tagged 'Player'.");
        }

        playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();

        if (visualCue != null)
        {
            visualCue.SetActive(false); // Ensure visual cue starts inactive
        }
        else
        {
            Debug.LogError($"Visual cue not assigned to {gameObject.name}. Please assign in the inspector.");
        }
    }

    private void Update()
    {
        // Check if the player is in range and presses "E" to trigger dialogue
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            SavePlayerState(); // Save the player's state before modifying
            TriggerDialogue();

            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero; // Stop any sliding
                playerRigidbody.gravityScale = 5; // Ensure instant fall
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true; // Player is now inside the trigger area

            if (visualCue != null)
            {
                visualCue.SetActive(true); // Show the visual cue
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Player is no longer in range

            if (visualCue != null)
            {
                visualCue.SetActive(false); // Hide the visual cue
            }
        }
    }

    private void SavePlayerState()
    {
        if (playerRigidbody != null)
        {
            originalPlayerVelocity = playerRigidbody.velocity;
            originalPlayerGravityScale = playerRigidbody.gravityScale;
        }
    }

    private void RestorePlayerState()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = originalPlayerVelocity;
            playerRigidbody.gravityScale = originalPlayerGravityScale;
        }
    }

    private void TriggerDialogue()
    {
        if (hasInteracted) return;

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (!dialogueManager.IsDialogueActive())
        {
            dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), transform);

            // Subscribe to dialogue end event to restore player state
            dialogueManager.OnDialogueEnd += HandleDialogueEnd;

            DeactivateVisualCue();
            RemoveCollider(); // Remove the collider after triggering dialogue
            hasInteracted = true;
        }
    }

    private void HandleDialogueEnd()
    {
        RestorePlayerState(); // Restore the player's state

        // Optionally unsubscribe from the event to avoid memory leaks
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueEnd -= HandleDialogueEnd;
        }
    }

    private void DeactivateVisualCue()
    {
        if (visualCue != null)
        {
            visualCue.SetActive(false);
        }
    }

    private void RemoveCollider()
    {
        if (npcCollider != null)
        {
            Destroy(npcCollider); // Remove the collider to prevent further interaction
            npcCollider = null;
        }
    }
}

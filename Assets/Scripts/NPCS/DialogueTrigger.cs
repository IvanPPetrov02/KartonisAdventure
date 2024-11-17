using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Object to Disappear")]
    [SerializeField] private GameObject objectToDisappear; // Assign in Inspector

    private Rigidbody2D playerRigidbody;
    private Collider2D npcCollider;
    private bool hasInteracted = false;
    private bool playerInRange = false;

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

        // Subscribe to the special phrase event
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.OnSpecialPhraseDetected += HandleSpecialPhrase;
        }
    }

    private void Update()
    {
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            SavePlayerState(); // Save player's state
            TriggerDialogue();

            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero; // Stop movement
                playerRigidbody.gravityScale = 5; // Ensure instant fall
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerInRange = true;

            if (visualCue != null)
            {
                visualCue.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (visualCue != null)
            {
                visualCue.SetActive(false);
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

            dialogueManager.OnDialogueEnd += HandleDialogueEnd;

            DeactivateVisualCue();
            RemoveCollider();
            hasInteracted = true;
        }
    }

    private void HandleDialogueEnd()
    {
        RestorePlayerState();

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
            Destroy(npcCollider);
            npcCollider = null;
        }
    }

    private void HandleSpecialPhrase(string phrase)
    {
        if (objectToDisappear != null)
        {
            objectToDisappear.SetActive(false);
            Debug.Log($"{objectToDisappear.name} has disappeared due to the special phrase: {phrase}");
        }
    }

    private void OnDestroy()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.OnSpecialPhraseDetected -= HandleSpecialPhrase;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    private Collider2D npcCollider;

    private void Awake()
    {
        npcCollider = GetComponent<Collider2D>();
        if (npcCollider == null)
        {
            Debug.LogError($"No Collider2D found on {gameObject.name}. Please add one.");
        }
        
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
            
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (!dialogueManager.IsDialogueActive())
        {
            Transform dialoguePosition = transform;
            
            if (npcCollider == null)
            {
                dialoguePosition = playerTransform;
            }

            dialogueManager.StartInkDialogue(new Ink.Runtime.Story(inkJSON.text), dialoguePosition);
            
            RemoveCollider();
        }
    }

    private void RemoveCollider()
    {
        if (npcCollider != null)
        {
            Debug.Log($"Removing collider from {gameObject.name}.");
            Destroy(npcCollider);
            npcCollider = null;
        }
    }
}

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager.DialogueSegment[] myLines; 
    private bool playerInRange;

    // 1. Add a slot for your "E" Canvas
    [SerializeField] private GameObject interactPrompt; 

    void Start()
    {
        // Ensure the prompt is hidden the moment the game starts
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange)
        {
            // If dialogue is playing, hide the 'E' (we don't want it floating while they talk!)
            if (DialogueManager.Instance.PlayingDialogue)
            {
                if (interactPrompt != null) interactPrompt.SetActive(false);
            }
            // If dialogue is NOT playing, show the 'E'
            else
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
                
                // Press 'E' to start talking
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DialogueManager.Instance.StartDialogue(myLines);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = false;
            
            // Hide the 'E' when she walks away
            if (interactPrompt != null) 
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
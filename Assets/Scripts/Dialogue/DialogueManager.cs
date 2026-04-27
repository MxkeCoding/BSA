using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [System.Serializable]
    public class DialogueSegment
    {
        public string SubjectText;
        [TextArea] public string DialogueToPrint;
        [Range(1f, 25f)] public float LettersPerSecond = 15f;
    }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text SubjectText;
    [SerializeField] private TMP_Text BodyText;

    private DialogueSegment[] currentSegments;
    private int DialogueIndex;
    public bool PlayingDialogue { get; private set; }
    private bool Skip;
    private bool isTyping;

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Ensure everything is blank and hidden on startup
        BodyText.text = string.Empty;
        SubjectText.text = string.Empty;
    }

    void Update()
    {
        if (!PlayingDialogue) return;

        // Listen for E or Left Click
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                Skip = true; // Instantly fill text
            }
            else
            {
                PlayNextSegment(); // Move to next line
            }
        }
    }

    public void StartDialogue(DialogueSegment[] segments)
    {
        // Start a routine to delay the UI by one frame.
        // This stops the 'E' key press that triggered the chat 
        // from instantly skipping the first line!
        StartCoroutine(DelayStart(segments));
    }

    private IEnumerator DelayStart(DialogueSegment[] segments)
    {
        yield return null; // Wait exactly one frame

        currentSegments = segments;
        DialogueIndex = 0;
        PlayingDialogue = true;
        dialoguePanel.SetActive(true);
        PlayNextSegment();
    }

    private void PlayNextSegment()
    {
        if (DialogueIndex < currentSegments.Length)
        {
            StartCoroutine(PlayDialogue(currentSegments[DialogueIndex]));
        }
        else
        {
            // 1. Dialogue is officially over, tell the Player script they can move again
            PlayingDialogue = false;
            
            // 2. Wipe the text completely clean!
            BodyText.text = "";
            SubjectText.text = "";
            
            // 3. (Optional) If you want the yellow box to disappear, uncomment the next line. 
            // But since you want the blank box to stay on the UI, we leave this deleted/commented out!
            // dialoguePanel.SetActive(false); 
        }
    }

    private IEnumerator PlayDialogue(DialogueSegment segment)
    {
        isTyping = true;
        Skip = false;
        BodyText.text = string.Empty;
        SubjectText.text = segment.SubjectText;

        float delay = 1f / segment.LettersPerSecond;
        
        for (int i = 0; i < segment.DialogueToPrint.Length; i++)
        {
            if (Skip)
            {
                BodyText.text = segment.DialogueToPrint;
                break; 
            }

            BodyText.text += segment.DialogueToPrint[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        DialogueIndex++;
    }
}
using UnityEngine;
using UnityEngine.UI;
using QTEPack; // Needed for the QuickTimeEvent[cite: 8, 9]

public class SanityManager : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDrainRate = 2f; // How much sanity drops per second

    [Header("Flower UI")]
    public Image flowerUI;
    [Tooltip("Drag the 7 sliced flower sprites here. 0 = Full, 6 = Dead")]
    public Sprite[] flowerFrames; 

    [Header("Panic Attack QTE")]
    public QuickTimeEvent panicQTE;
    public PlayerController player;
    public float panicDamage = 15f; // Health lost on missed skill check
    
    private bool isPanicking = false;

    void Start()
    {
        currentSanity = maxSanity;
        UpdateFlowerUI();
    }

    void Update()
    {
        // Only drain sanity if she isn't already panicking
        if (!isPanicking)
        {
            currentSanity -= sanityDrainRate * Time.deltaTime;
            currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
            
            UpdateFlowerUI();

            if (currentSanity <= 0)
            {
                TriggerPanicAttack();
            }
        }
    }

    void UpdateFlowerUI()
    {
        if (flowerFrames.Length == 0) return;

        float sanityPercent = currentSanity / maxSanity;

        int frameIndex = Mathf.FloorToInt((1f - sanityPercent) * (flowerFrames.Length - 1));
        frameIndex = Mathf.Clamp(frameIndex, 0, flowerFrames.Length - 1);

        flowerUI.sprite = flowerFrames[frameIndex];
    }

    public void TriggerPanicAttack()
    {
        isPanicking = true;
        
        player.SetPanicPortrait(true);
        
        player.enabled = false; 
        
        StartPanicQTE();
    }

    private void StartPanicQTE()
    {
        panicQTE.OnSuccess.RemoveAllListeners();
        panicQTE.OnFail.RemoveAllListeners();

        panicQTE.OnSuccess.AddListener(OnPanicSuccess);
        panicQTE.OnFail.AddListener(OnPanicFail);

        panicQTE.ShowQTE(new Vector2(0, 0), 1f, 0); 
    }

    private void OnPanicSuccess()
    {
        isPanicking = false;
        
        // give her 50 sanity back
        currentSanity = maxSanity * 0.5f; 
        UpdateFlowerUI();
        
        player.SetPanicPortrait(false);

        player.enabled = true;
        panicQTE.Hide();
    }

    private void OnPanicFail()
    {
        player.TakeDamage(panicDamage);

        panicQTE.Hide(); 
        
        if (player.currentHealth > 0) 
        {
            StartPanicQTE();
        }
    }
}
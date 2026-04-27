using UnityEngine;
using System.Collections;

public class SentinelAI : MonoBehaviour
{
    [Header("Hierarchy Links")]
    [SerializeField] private Transform visualsTransform; // Drag the "Visuals" child here!

    [Header("Timers")]
    [SerializeField] private float greenLightDuration = 4f; 
    [SerializeField] private float redLightDuration = 3f;   
    [SerializeField] private float turnDuration = 0.5f; 
    
    [Header("Combat")]
    [SerializeField] private float instantDamage = 30f; 
    
    private PlayerController playerInZone;
    public bool isLooking { get; private set; }

    void Start()
    {
        // Safety check: if you forgot to drag it in, it tries to find a child named "Visuals"
        if (visualsTransform == null) visualsTransform = transform.Find("Visuals");
        
        StartCoroutine(WatchLoop());
    }

    IEnumerator WatchLoop()
    {
        while (true)
        {
            // 1. GREEN LIGHT: Turning Visuals Away
            isLooking = false;
            yield return StartCoroutine(SmoothTurn(180)); 
            yield return new WaitForSeconds(greenLightDuration);

            // 2. RED LIGHT: Turning Visuals Back
            yield return StartCoroutine(SmoothTurn(0)); 
            isLooking = true;

            // CHECK: Since the Parent (and its Trigger) never moved, 
            // the OnTriggerEnter logic is still perfectly accurate!
            if (playerInZone != null && !playerInZone.isHidden)
            {
                playerInZone.TakeDamage(instantDamage);
            }

            yield return new WaitForSeconds(redLightDuration);
        }
    }

    IEnumerator SmoothTurn(float targetY)
    {
        float time = 0;
        // Notice we are now rotating visualsTransform, NOT "transform"
        Quaternion startRotation = visualsTransform.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, targetY, 0);

        while (time < turnDuration)
        {
            visualsTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, time / turnDuration);
            time += Time.deltaTime;
            yield return null;
        }

        visualsTransform.localRotation = endRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = null;
        }
    }
}
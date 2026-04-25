using UnityEngine;
using System.Collections;

public class SentinelAI : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float greenLightDuration = 4f; 
    [SerializeField] private float redLightDuration = 3f;   
    [SerializeField] private float turnDuration = 0.5f; // How long the turn takes
    
    [Header("Combat")]
    [SerializeField] private float instantDamage = 30f; 
    [SerializeField] private float detectionRange = 20f; 
    [SerializeField] private PlayerController player;

    public bool isLooking { get; private set; }

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(WatchLoop());
    }

    IEnumerator WatchLoop()
    {
        while (true)
        {
            // 1. GREEN LIGHT: Turning Away
            isLooking = false;
            yield return StartCoroutine(SmoothTurn(180)); // Rotate to face away
            yield return new WaitForSeconds(greenLightDuration);

            // 2. RED LIGHT: Turning Back
            // Start rotating back to 0 (facing player)
            yield return StartCoroutine(SmoothTurn(0)); 
            
            // The MOMENT the turn is finished, we set isLooking to true
            isLooking = true;

            // INSTANT CHECK: Did he catch you?
            CheckForCatch();

            yield return new WaitForSeconds(redLightDuration);
        }
    }

    // New function to rotate smoothly instead of instantly
    IEnumerator SmoothTurn(float targetY)
    {
        float time = 0;
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(0, targetY, 0);

        while (time < turnDuration)
        {
            // Spherically interpolates between two rotations over time
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, time / turnDuration);
            time += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.localRotation = endRotation; // Ensure it finishes exactly at the target
    }

    void CheckForCatch()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange && !player.isHidden)
        {
            Debug.Log("SAMPAGUITA SPOTTED!");
            player.TakeDamage(instantDamage);
        }
    }
}
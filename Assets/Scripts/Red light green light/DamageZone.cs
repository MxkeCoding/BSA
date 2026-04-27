using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageInterval = 1.0f; // Every 1 second
    private float nextDamageTime;

    private void OnTriggerStay(Collider other)
    {
        // Check if the object inside the trigger is the Player
        if (other.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                // Find the PlayerController script on the object that entered
                PlayerController player = other.GetComponent<PlayerController>();

                if (player != null)
                {
                    player.TakeDamage(damageAmount); // Call the function we just made
                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }
}
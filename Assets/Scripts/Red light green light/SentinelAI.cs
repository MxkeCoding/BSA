using UnityEngine;
using System.Collections;

public class SentinelAI : MonoBehaviour
{
    [Header("Hierarchy Links")]
    [SerializeField] private Animator anim; 

    [Header("Timers")]
    [SerializeField] private float greenLightDuration = 4f; 
    [SerializeField] private float redLightDuration = 3f;   
    [SerializeField] private float turnDuration = 0.5f; 
    
    [Header("Combat")]
    [SerializeField] private float instantDamage = 30f; 
    [SerializeField] private AudioClip gunShotSound; 
    
    private PlayerController playerInZone;
    public bool isLooking { get; private set; }

    private const int STATE_BACK = 0;
    private const int STATE_SIDE = 1;
    private const int STATE_FRONT = 2;
    private const int STATE_SHOOT = 3;

    void Start()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        StartCoroutine(WatchLoop());
    }

    IEnumerator WatchLoop()
    {
        while (true)
        {
            // Green light
            isLooking = false;
            anim.SetInteger("State", STATE_BACK);
            yield return new WaitForSeconds(greenLightDuration);

            // Warnin
            anim.SetInteger("State", STATE_SIDE);
            yield return new WaitForSeconds(turnDuration);

            // Redlight
            isLooking = true;
            anim.SetInteger("State", STATE_FRONT);

            yield return new WaitForSeconds(0.1f); 

            // check if player hiding
            if (playerInZone != null && !playerInZone.isHidden)
            {
                // shooting sprite
                anim.SetInteger("State", STATE_SHOOT);
                
                // wait half a second
                yield return new WaitForSeconds(0.5f); 
                
                //
                if (playerInZone != null && !playerInZone.isHidden) 
                {
                    
                    if (gunShotSound != null && SoundFXManager.instance != null)
                    {
                        SoundFXManager.instance.PlaySoundFXClip(gunShotSound, transform, 0.8f);
                    }

                    // Deal the damage
                    
                    playerInZone.TakeDamage(instantDamage);
                }
                
                yield return new WaitForSeconds(0.5f); 
            }

            // Finish the Red Light duration
            yield return new WaitForSeconds(redLightDuration);

            isLooking = false; 
            anim.SetInteger("State", STATE_SIDE);
            yield return new WaitForSeconds(turnDuration);
        }
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
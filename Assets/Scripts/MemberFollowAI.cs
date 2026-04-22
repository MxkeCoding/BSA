using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberFollowAI : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 5f; // Match Sampaguita's walk speed
    [SerializeField] private float runSpeed = 8f;  // Match Sampaguita's run speed
    [SerializeField] private float startDistance = 1.5f; // Distance to start walking
    [SerializeField] private float stopDistance = 1.5f;  // Distance to stop and go idle
    private bool isCurrentlyWalking = false;    

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private const string IS_WALK_PARAM = "isWalk";

    // Start is called before the first frame update
    void Start()
    {
        // Allow visuals to be structured with child objects.
        anim = gameObject.GetComponent<Animator>();
        if (anim == null) anim = gameObject.GetComponentInChildren<Animator>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followTarget == null) return;
        
        Animator targetAnim = followTarget.GetComponentInChildren<Animator>();
        if (targetAnim != null)
        {
            // Mimic Crouch
            bool isTargetCrouching = targetAnim.GetBool("isCrouch");
            anim.SetBool("isCrouch", isTargetCrouching);

            // NEW: Mimic Run
            bool isTargetRunning = targetAnim.GetBool("isRun");
            anim.SetBool("isRun", isTargetRunning);
        }

        float currentDist = Vector3.Distance(transform.position, followTarget.position);

        // 1. Logic to decide if we SHOULD be walking
        if (currentDist > startDistance)
        {
            isCurrentlyWalking = true;
        }
        else if (currentDist <= stopDistance)
        {
            isCurrentlyWalking = false;
        }

        // 2. Execution of movement and animation
        if (isCurrentlyWalking)
        {
            // NEW: Choose speed based on the animator state we just mimicked
            float currentMoveSpeed = anim.GetBool("isRun") ? runSpeed : walkSpeed;
            float step = currentMoveSpeed * Time.deltaTime;
            
            Vector3 lastPos = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, followTarget.position, step);

            Vector3 realDirection = (transform.position - lastPos).normalized;

            if (realDirection != Vector3.zero)
            {
                anim.SetBool(IS_WALK_PARAM, true);

                // Manual damping for smooth turns Dabesaba
                float smoothedX = Mathf.MoveTowards(anim.GetFloat("moveX"), realDirection.x, 5f * Time.deltaTime);
                float smoothedY = Mathf.MoveTowards(anim.GetFloat("moveY"), realDirection.z, 5f * Time.deltaTime);
                anim.SetFloat("moveX", smoothedX);
                anim.SetFloat("moveY", smoothedY);
            }
        }
        else
        {
            anim.SetBool(IS_WALK_PARAM, false);
        }
    }
    public void SetFollowDistance(float followDistance)
    {
        // We update the stop distance to match your spacing
        stopDistance = followDistance;

        // We set startDistance slightly higher (like 0.3 more) 
        // This prevents "jittering" where the AI starts and stops constantly.
        startDistance = followDistance;
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
}
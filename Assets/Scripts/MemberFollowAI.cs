using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberFollowAI : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private int speed;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float startDistance = 1.3f; // Distance to start walking
    [SerializeField] private float stopDistance = 1f;  // Distance to stop and go idle
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
            float step = speed * Time.deltaTime;
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
        minDistance = followDistance;
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2f; 
    
    [Header("Visuals")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private float currentSpeed; 
    
    private const string IS_WALK_PARAM = "isWalk";
    private const string IS_CROUCH_PARAM = "isCrouch"; 

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // Stop all movement and crouching if dialogue is playing
        if (DialogueManager.Instance != null && DialogueManager.Instance.PlayingDialogue)
        {
            anim.SetBool(IS_WALK_PARAM, false); 
            anim.SetBool(IS_CROUCH_PARAM, false); 
            movement = Vector3.zero; 
            return; 
        }

        // Read Inputs
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;
        bool isCrouching = playerControls.Player.Crouch.IsPressed();

        movement = new Vector3(x, 0, z).normalized;

        // Change speed based on crouching state
        currentSpeed = isCrouching ? crouchSpeed : walkSpeed;

        // Update Animator
        if (movement != Vector3.zero) 
        {
            anim.SetFloat("moveX", x);
            anim.SetFloat("moveY", z);
        }

        anim.SetBool(IS_WALK_PARAM, movement != Vector3.zero);
        anim.SetBool(IS_CROUCH_PARAM, isCrouching); 
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * currentSpeed * Time.deltaTime);
        
    }

    public void SetOverworldVisuals(Animator animator, SpriteRenderer spriteRenderer, Vector3 scale)
    {
        anim = animator;
        playerSprite = spriteRenderer;
        transform.localScale = scale;
    }
}
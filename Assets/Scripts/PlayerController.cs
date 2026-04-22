using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float runSpeed = 8f; 

    [Header("Stamina Settings")]
    [SerializeField] private Image staminaBarImage;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainRate = 25f;  
    [SerializeField] private float regenRate = 15f; 
    [SerializeField] private float recoveryThreshold = 20f;
    [SerializeField] private Color normalColor = new Color(0.6f, 0.7f, 1f); 
    [SerializeField] private Color exhaustedColor = Color.red;
    private float currentStamina;
    private bool isExhausted;
    
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

        // stamina
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.PlayingDialogue)
        {
            anim.SetBool(IS_WALK_PARAM, false); 
            anim.SetBool(IS_CROUCH_PARAM, false); 
            movement = Vector3.zero; 
            return; 
        }

        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        // crouch
        bool isCrouching = playerControls.Player.Crouch.IsPressed();
        // run
        bool isRunning = playerControls.Player.Run.IsPressed(); 
        movement = new Vector3(x, 0, z).normalized;
        bool isMoving = movement != Vector3.zero;

        // Handle Exhaustion State
        if (currentStamina <= 0) isExhausted = true;
        if (isExhausted && currentStamina >= recoveryThreshold) isExhausted = false;

        // stamina logic
        if (isRunning && isMoving && !isCrouching && currentStamina > 0 && !isExhausted)
        {
            currentSpeed = runSpeed;
            currentStamina -= drainRate * Time.deltaTime;
        }
        else
        {
            currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
            
            // regen if not running or if standing still
            if (currentStamina < maxStamina)
            {
                currentStamina += regenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        float staminaRatio = currentStamina / maxStamina;
        if (staminaBarImage != null)
        {
            staminaBarImage.fillAmount = staminaRatio;
            
            // Toggle between your chosen colors
            staminaBarImage.color = isExhausted ? exhaustedColor : normalColor;
        }

        if (movement != Vector3.zero) 
        {
            anim.SetFloat("moveX", x);
            anim.SetFloat("moveY", z);
        }
        anim.SetBool(IS_WALK_PARAM, isMoving);
        anim.SetBool(IS_CROUCH_PARAM, isCrouching); 
        bool actuallyRunning = isRunning && isMoving && !isCrouching && !isExhausted;
        anim.SetBool("isRun", actuallyRunning);
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
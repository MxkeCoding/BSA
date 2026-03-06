using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    // 
    [SerializeField] private int stepsInGrass; // grass step 
    [SerializeField] private LayerMask grassLayer; // grass layer
    

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    
    // Grass variables
    private bool movingInGrass;
    private float stepTimer;
    private const float timePerStep = 0.5f;

    private const string IS_WALK_PARAM = "isWalk";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3(x, 0, z).normalized;

        // Send values to the Blend Tree
        // If you aren't moving, keep the last movement values so she stays facing that way
        if (movement != Vector3.zero) 
        {
            anim.SetFloat("moveX", x);
            anim.SetFloat("moveY", z);
        }

        anim.SetBool(IS_WALK_PARAM, movement != Vector3.zero);
        
        // REMOVE the playerSprite.flipX logic entirely!
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

        // checks if collides with grass
        Collider[] colliders = Physics.OverlapSphere(transform.position,1,grassLayer);
        movingInGrass = colliders.Length!=0 && movement !=Vector3.zero;



    }
}

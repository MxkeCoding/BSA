using UnityEngine;

public class UIHover : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverSpeed = 3f;   // How fast it bobs
    [SerializeField] private float hoverAmount = 10f; // How high/low it goes (in UI pixels)

    private Vector3 startPos;

    void Start()
    {
        //  where you placed it in the Canvas
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Sine wave 
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
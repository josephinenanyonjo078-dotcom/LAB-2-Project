using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class COURTBASE : MonoBehaviour
{
    [Header("Courtyard Settings")]
    [SerializeField] private bool isGround = true;
    [SerializeField] private LayerMask interactableLayer = -1;
    
    private BoxCollider courtyardCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCourtyardCollider();
    }
    
    private void SetupCourtyardCollider()
    {
        // Get or add BoxCollider component
        courtyardCollider = GetComponent<BoxCollider>();
        
        if (courtyardCollider == null)
        {
            courtyardCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Configure collider for ground
        if (isGround)
        {
            courtyardCollider.isTrigger = false; // Solid collision
            courtyardCollider.size = new Vector3(20f, 0.1f, 20f); // Adjust size as needed
            courtyardCollider.center = new Vector3(0, -0.05f, 0); // Slightly below ground level
        }
        
        // Ensure the collider has proper physics material
        PhysicsMaterial groundMaterial = new PhysicsMaterial("GroundMaterial");
        groundMaterial.dynamicFriction = 0.6f;
        groundMaterial.staticFriction = 0.6f;
        groundMaterial.bounciness = 0.1f;
        groundMaterial.frictionCombine = PhysicsMaterialCombine.Average;
        groundMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        
        courtyardCollider.material = groundMaterial;
        
        // Add rigidbody to make it a static physics object
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // Static object that doesn't move but provides collision
        rb.useGravity = false;
    }
    
    // Method to check if an object should collide with the courtyard
    private void OnTriggerEnter(Collider other)
    {
        if (!isGround)
        {
            // Handle trigger events if needed
            Debug.Log($"Object {other.name} entered courtyard trigger area");
        }
    }
    
    // Method to handle collision events
    private void OnCollisionEnter(Collision collision)
    {
        if (isGround)
        {
            Debug.Log($"Object {collision.gameObject.name} collided with courtyard ground");
            
            // Ensure the object stops falling
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                // Apply slight upward force to prevent sinking
                Vector3 contactPoint = collision.contacts[0].point;
                Vector3 upwardForce = Vector3.up * 0.1f;
                otherRb.AddForceAtPosition(upwardForce, contactPoint, ForceMode.Impulse);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

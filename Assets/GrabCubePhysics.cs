using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrabCubePhysics : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float angularDrag = 0.5f;
    [SerializeField] private bool useGravity = true;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.down;
    
    [Header("Collision Settings")]
    [SerializeField] private PhysicsMaterial cubePhysicsMaterial;
    
    private Rigidbody rb;
    private Collider cubeCollider;
    private bool isGrounded = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    
    void Start()
    {
        SetupPhysics();
        SetupGrabInteraction();
    }
    
    private void SetupPhysics()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure rigidbody properties
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
        rb.useGravity = useGravity;
        rb.isKinematic = false;
        
        // Get or add Collider
        cubeCollider = GetComponent<Collider>();
        if (cubeCollider == null)
        {
            cubeCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Ensure collider is not a trigger
        cubeCollider.isTrigger = false;
        
        // Set up physics material
        if (cubePhysicsMaterial == null)
        {
            cubePhysicsMaterial = new PhysicsMaterial("CubeMaterial");
            cubePhysicsMaterial.dynamicFriction = 0.3f;
            cubePhysicsMaterial.staticFriction = 0.3f;
            cubePhysicsMaterial.bounciness = 0.1f;
            cubePhysicsMaterial.frictionCombine = PhysicsMaterialCombine.Average;
            cubePhysicsMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }
        
        cubeCollider.material = cubePhysicsMaterial;
    }
    
    private void SetupGrabInteraction()
    {
        // Get or add XRGrabInteractable
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }
        
        // Configure grab settings
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable.MovementType.VelocityTracking;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
    }
    
    void Update()
    {
        CheckGrounded();
    }
    
    private void CheckGrounded()
    {
        // Cast a ray downward to check for ground
        Vector3 rayStart = transform.position + groundCheckOffset;
        RaycastHit hit;
        
        isGrounded = Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance, groundLayer);
        
        if (isGrounded)
        {
            // Optional: Apply additional stabilization when on ground
            if (rb.linearVelocity.y < 0.1f && rb.linearVelocity.y > -0.1f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with ground/courtyard
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.GetComponent<COURTBASE>() != null)
        {
            Debug.Log($"Grab cube collided with: {collision.gameObject.name}");
            
            // Prevent sinking into ground
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 contactNormal = collision.contacts[0].normal;
            
            // Apply slight upward force if object is sinking
            if (contactNormal.y > 0.7f) // Mostly horizontal surface
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Max(0, rb.linearVelocity.y), rb.linearVelocity.z);
            }
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // Maintain proper contact with ground
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.GetComponent<COURTBASE>() != null)
        {
            // Prevent sliding when at rest
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    
    // Public method to reset cube position if it falls too far
    public void ResetPosition(Vector3 resetPosition)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = resetPosition;
    }
    
    // Visualize ground check in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + groundCheckOffset;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(rayStart + Vector3.down * groundCheckDistance, 0.05f);
    }
}

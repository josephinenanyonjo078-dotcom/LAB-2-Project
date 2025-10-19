using UnityEngine;

/// <summary>
/// Central physics manager to handle global physics settings and ensure proper collision detection
/// </summary>
public class PhysicsManager : MonoBehaviour
{
    [Header("Global Physics Settings")]
    [SerializeField] private float globalGravity = -9.81f;
    [SerializeField] private bool enablePhysicsDebug = true;
    
    [Header("Fall Prevention")]
    [SerializeField] private float maxFallDistance = 50f;
    [SerializeField] private Vector3 resetPosition = new Vector3(0, 1, 0);
    [SerializeField] private bool autoResetFallenObjects = true;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayers = 1;
    [SerializeField] private float groundCheckRadius = 0.5f;
    
    private static PhysicsManager instance;
    public static PhysicsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PhysicsManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PhysicsManager");
                    instance = go.AddComponent<PhysicsManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        SetupGlobalPhysics();
    }
    
    void Start()
    {
        // Find and configure all physics objects in the scene
        ConfigureScenePhysics();
    }
    
    void Update()
    {
        if (autoResetFallenObjects)
        {
            CheckForFallenObjects();
        }
    }
    
    private void SetupGlobalPhysics()
    {
        // Set global gravity
        Physics.gravity = new Vector3(0, globalGravity, 0);
        
        // Configure physics settings
        Physics.defaultSolverIterations = 6;
        Physics.defaultSolverVelocityIterations = 1;
        Physics.bounceThreshold = 2f;
        Physics.sleepThreshold = 0.005f;
        Physics.defaultContactOffset = 0.01f;
        
        Debug.Log("Global physics settings configured");
    }
    
    private void ConfigureScenePhysics()
    {
        // Find all COURTBASE objects and ensure they're properly configured
        COURTBASE[] courtyardObjects = FindObjectsOfType<COURTBASE>();
        foreach (COURTBASE courtyard in courtyardObjects)
        {
            if (courtyard.GetComponent<Collider>() == null)
            {
                Debug.LogWarning($"COURTBASE object {courtyard.name} is missing a collider!");
            }
        }
        
        // Find all grab cubes and ensure they have proper physics
        GrabCubePhysics[] grabCubes = FindObjectsOfType<GrabCubePhysics>();
        foreach (GrabCubePhysics cube in grabCubes)
        {
            if (cube.GetComponent<Rigidbody>() == null)
            {
                Debug.LogWarning($"Grab cube {cube.name} is missing a rigidbody!");
            }
        }
        
        Debug.Log($"Configured physics for {courtyardObjects.Length} courtyard objects and {grabCubes.Length} grab cubes");
    }
    
    private void CheckForFallenObjects()
    {
        // Check all rigidbodies for objects that have fallen too far
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.gameObject.GetComponent<GrabCubePhysics>() != null)
            {
                // Check if grab cube has fallen too far
                if (rb.transform.position.y < (resetPosition.y - maxFallDistance))
                {
                    Debug.Log($"Resetting fallen object: {rb.gameObject.name}");
                    ResetObjectPosition(rb.gameObject);
                }
            }
        }
    }
    
    public void ResetObjectPosition(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        obj.transform.position = resetPosition;
        
        Debug.Log($"Reset object {obj.name} to position {resetPosition}");
    }
    
    public bool IsObjectGrounded(GameObject obj)
    {
        Vector3 rayStart = obj.transform.position;
        RaycastHit hit;
        
        return Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckRadius + 0.1f, groundLayers);
    }
    
    public void ApplyUpwardForce(GameObject obj, float force = 1f)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }
    
    // Debug visualization
    void OnDrawGizmos()
    {
        if (enablePhysicsDebug)
        {
            // Draw ground check area
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(resetPosition, groundCheckRadius);
            
            // Draw reset position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(resetPosition, Vector3.one * 0.5f);
        }
    }
    
    // Public methods for other scripts to use
    public void SetGlobalGravity(float gravity)
    {
        globalGravity = gravity;
        Physics.gravity = new Vector3(0, globalGravity, 0);
    }
    
    public void SetGroundLayers(LayerMask layers)
    {
        groundLayers = layers;
    }
    
    public LayerMask GetGroundLayers()
    {
        return groundLayers;
    }
}

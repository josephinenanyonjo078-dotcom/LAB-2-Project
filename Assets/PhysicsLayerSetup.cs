using UnityEngine;

/// <summary>
/// Helper script to set up proper physics layers and collision matrix
/// Attach this to a GameObject in your scene and run it once to configure layers
/// </summary>
public class PhysicsLayerSetup : MonoBehaviour
{
    [Header("Layer Configuration")]
    [SerializeField] private bool setupLayersOnStart = false;
    
    [Header("Layer Names")]
    [SerializeField] private string groundLayerName = "Ground";
    [SerializeField] private string interactableLayerName = "Interactable";
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string uiLayerName = "UI";
    
    void Start()
    {
        if (setupLayersOnStart)
        {
            SetupPhysicsLayers();
        }
    }
    
    [ContextMenu("Setup Physics Layers")]
    public void SetupPhysicsLayers()
    {
        Debug.Log("Setting up physics layers and collision matrix...");
        
        // Note: Layer names and collision matrix setup needs to be done manually in Unity Editor
        // This script provides guidance on what needs to be configured
        
        PrintLayerSetupInstructions();
    }
    
    private void PrintLayerSetupInstructions()
    {
        Debug.Log("=== PHYSICS LAYER SETUP INSTRUCTIONS ===");
        Debug.Log("1. Go to Edit > Project Settings > Tags and Layers");
        Debug.Log("2. Set up the following layers:");
        Debug.Log($"   - Layer 8: {groundLayerName}");
        Debug.Log($"   - Layer 9: {interactableLayerName}");
        Debug.Log($"   - Layer 10: {playerLayerName}");
        Debug.Log($"   - Layer 11: {uiLayerName}");
        Debug.Log("");
        Debug.Log("3. Go to Edit > Project Settings > Physics");
        Debug.Log("4. Configure collision matrix:");
        Debug.Log($"   - {groundLayerName} should collide with {interactableLayerName} and {playerLayerName}");
        Debug.Log($"   - {interactableLayerName} should collide with {groundLayerName} and {playerLayerName}");
        Debug.Log($"   - {playerLayerName} should collide with {groundLayerName} and {interactableLayerName}");
        Debug.Log($"   - {uiLayerName} should NOT collide with any physics objects");
        Debug.Log("");
        Debug.Log("5. Assign layers to your objects:");
        Debug.Log($"   - Courtyard/Court Base: {groundLayerName}");
        Debug.Log($"   - Grab Cube: {interactableLayerName}");
        Debug.Log($"   - VR Player: {playerLayerName}");
        Debug.Log("=== END INSTRUCTIONS ===");
    }
    
    /// <summary>
    /// Helper method to assign proper layers to objects
    /// </summary>
    [ContextMenu("Assign Layers to Scene Objects")]
    public void AssignLayersToSceneObjects()
    {
        // Find and assign layers to common objects
        AssignLayerToObjectsWithTag("Ground", groundLayerName);
        AssignLayerToObjectsWithComponent<COURTBASE>(groundLayerName);
        AssignLayerToObjectsWithComponent<GrabCubePhysics>(interactableLayerName);
        
        Debug.Log("Layer assignment completed. Check console for any missing objects.");
    }
    
    private void AssignLayerToObjectsWithTag(string tag, string layerName)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        int layerIndex = LayerMask.NameToLayer(layerName);
        
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' not found. Please create it in Project Settings > Tags and Layers");
            return;
        }
        
        foreach (GameObject obj in objects)
        {
            obj.layer = layerIndex;
            Debug.Log($"Assigned layer '{layerName}' to {obj.name}");
        }
    }
    
    private void AssignLayerToObjectsWithComponent<T>(string layerName) where T : Component
    {
        T[] components = FindObjectsOfType<T>();
        int layerIndex = LayerMask.NameToLayer(layerName);
        
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' not found. Please create it in Project Settings > Tags and Layers");
            return;
        }
        
        foreach (T component in components)
        {
            component.gameObject.layer = layerIndex;
            Debug.Log($"Assigned layer '{layerName}' to {component.gameObject.name}");
        }
    }
}

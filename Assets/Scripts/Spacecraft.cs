using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// The manager of the spacecraft as a whole. responsible for managing what mode each piece is in as well as activating engines
/// </summary>
public class Spacecraft : MonoBehaviour {
    
    private static Spacecraft Instance;

    public static bool IsBuildMode { get; private set; }
    
    private Rigidbody2D rb;
    private FixedJoint2D[] partJoints;
    private Rigidbody2D[] partRigidbodies;
    
    // Cache original joint connections to preserve part-to-part links
    private System.Collections.Generic.Dictionary<FixedJoint2D, Rigidbody2D> originalJointConnections;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Engine engine;



    private void Awake() {
        // Singleton pattern to prevent duplicate spacecrafts
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Get Rigidbody2D from self or children (SpacecraftStart)
        rb = GetComponentInChildren<Rigidbody2D>();
        
        // Cache original joint connections
        CacheOriginalJointConnections();
    }
    
    private void CacheOriginalJointConnections() {
        if (originalJointConnections == null) {
            originalJointConnections = new System.Collections.Generic.Dictionary<FixedJoint2D, Rigidbody2D>();
        }
        
        FixedJoint2D[] joints = GetComponentsInChildren<FixedJoint2D>(true);
        foreach (FixedJoint2D joint in joints) {
            if (!originalJointConnections.ContainsKey(joint)) {
                originalJointConnections[joint] = joint.connectedBody;
            }
        }
    }
    
    private void Start() {
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction;
        gameInput.OnActivateEngineCanceledAction += GameInput_OnActivateEngineAction;

        // Listen for scene changes to update physics mode
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize physics mode based on current scene
        UpdatePhysicsMode();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Delay physics update to next frame to ensure all children are initialized
        StartCoroutine(UpdatePhysicsModeDelayed());
    }

    private System.Collections.IEnumerator UpdatePhysicsModeDelayed() {
        // Wait one frame to ensure all child objects are initialized
        yield return null;
        UpdatePhysicsMode();
    }
    
    private void UpdatePhysicsMode() {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "BuildScene") {
            SetBuildingMode();
        } else if (currentScene == "FlightScene") {
            SetFlightMode();
        }
    }
    
    private void SetBuildingMode() {
        IsBuildMode = true;
        // Ensure rb is not null - try to find it if missing
        if (rb == null) {
            rb = GetComponentInChildren<Rigidbody2D>();
            if (rb == null) {
                Debug.LogError("Spacecraft: Rigidbody2D not found on Spacecraft or in children!");
                return;
            }
        }
        
        // Find all part rigidbodies and joints
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        
        // Disable all joints first
        foreach (FixedJoint2D joint in partJoints) {
            joint.enabled = false;
        }
        
        // Make all parts kinematic and disable simulation
        foreach (Rigidbody2D partRb in partRigidbodies) {
            if (partRb != rb) {
                partRb.bodyType = RigidbodyType2D.Kinematic;
                partRb.simulated = false;
            }
        }
        
        // Disable main spacecraft physics
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        
        // Disable engine
        if (engine != null) {
            engine.enabled = false;
        }
    }
    
    private void SetFlightMode() {
        IsBuildMode = false;
        
        // Ensure rb is not null - try to find it if missing
        if (rb == null) {
            rb = GetComponentInChildren<Rigidbody2D>();
            if (rb == null) {
                Debug.LogError("Spacecraft: Rigidbody2D not found on Spacecraft or in children!");
                return;
            }
        }
        
        // Update cached joints if needed
        CacheOriginalJointConnections();
        
        // Find all part rigidbodies and joints
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        
        // Get all colliders
        Collider2D[] partColliders = GetComponentsInChildren<Collider2D>();
        
        // DISABLE PartDrag components in flight mode (so parts can't be dragged)
        PartDrag[] partDrags = GetComponentsInChildren<PartDrag>();
        foreach (PartDrag partDrag in partDrags) {
            partDrag.enabled = false;
        }
        
        // Step 1: Ensure all bodies are kinematic/simulated OFF first
        foreach (Rigidbody2D partRb in partRigidbodies) {
            partRb.bodyType = RigidbodyType2D.Kinematic;
            partRb.simulated = false;
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        
        // Step 2: Enable colliders and ensure they're not triggers
        foreach (Collider2D collider in partColliders) {
            collider.enabled = true;
            collider.isTrigger = false;
        }
        
        // Step 3: Restore joint connections from cache (preserves part-to-part links)
        foreach (FixedJoint2D joint in partJoints) {
            Rigidbody2D jointRb = joint.GetComponent<Rigidbody2D>();
            if (jointRb == rb) {
                // Skip joints on the main spacecraft Rigidbody2D
                continue;
            }
            
            // Restore original connection if cached, otherwise connect to main rb
            if (originalJointConnections != null && originalJointConnections.ContainsKey(joint)) {
                Rigidbody2D originalConnected = originalJointConnections[joint];
                // If original was null or destroyed, fall back to main rb
                if (originalConnected != null && originalConnected.gameObject.activeInHierarchy) {
                    joint.connectedBody = originalConnected;
                } else {
                    joint.connectedBody = rb;
                }
            } else {
                joint.connectedBody = rb; // Fallback if not cached
            }
            
            joint.enableCollision = false;
            joint.enabled = true;
        }
        
        // Step 4: Sync transforms BEFORE enabling physics
        Physics2D.SyncTransforms();
        
        // Step 5: Zero all velocities before enabling dynamics
        foreach (Rigidbody2D partRb in partRigidbodies) {
            partRb.linearVelocity = Vector2.zero;
            partRb.angularVelocity = 0f;
        }
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Step 6: NOW enable physics for all parts
        foreach (Rigidbody2D partRb in partRigidbodies) {
            if (partRb != rb) {
                partRb.simulated = true;
                partRb.bodyType = RigidbodyType2D.Dynamic;
                partRb.gravityScale = 1f;
                partRb.excludeLayers = 0;
            }
        }
        
        // Enable main spacecraft physics
        rb.simulated = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.excludeLayers = 0;
        
        // Step 7: Wake up all Rigidbody2D components
        foreach (Rigidbody2D partRb in partRigidbodies) {
            partRb.WakeUp();
        }
        rb.WakeUp();

        // Enable engine component for flight mode (so FixedUpdate can run)
        if (engine != null) {
            engine.enabled = true;
        }
    }
    
    private void GameInput_OnActivateEngineAction(object sender, GameInput.EngineActivatedEventArgs e) { 
        engine.enabled = e.activated;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) {
            Instance = null;
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spacecraft : MonoBehaviour {
    
    private Rigidbody2D rb;
    private FixedJoint2D[] partJoints;
    private Rigidbody2D[] partRigidbodies;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Engine engine;

    private void Awake() {
        DontDestroyOnLoad(this);
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Start() {
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction; //Adds GameInput_OnActivateEngineAction() as a listener to the OnActivateEngineAction event. 
        gameInput.OnActivateEngineCanceledAction += GameInput_OnActivateEngineAction;

        // Listen for scene changes to update physics mode
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize physics mode based on current scene
        UpdatePhysicsMode();
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
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
        // Find all part rigidbodies and joints
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        
        // Make all parts kinematic and disable joints for building
        foreach (Rigidbody2D partRb in partRigidbodies) {
            if (partRb != rb) { // Don't affect main spacecraft rigidbody
                partRb.bodyType = RigidbodyType2D.Kinematic;
                partRb.simulated = false;
            }
        }
        
        // Disable joints during building
        foreach (FixedJoint2D joint in partJoints) {
            joint.enabled = false;
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
        // Find all part rigidbodies and joints
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        
        // Enable physics for all parts
        foreach (Rigidbody2D partRb in partRigidbodies) {
            if (partRb != rb) { // Don't affect main spacecraft rigidbody
                partRb.bodyType = RigidbodyType2D.Dynamic;
                partRb.simulated = true;
            }
        }
        
        // Enable joints for flight
        foreach (FixedJoint2D joint in partJoints) {
            joint.enabled = true;
        }
        
        // Enable main spacecraft physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
    }
    
    private void GameInput_OnActivateEngineAction(object sender, GameInput.EngineActivatedEventArgs e) { 
        engine.enabled = e.activated;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

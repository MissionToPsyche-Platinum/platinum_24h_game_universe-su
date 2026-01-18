using UnityEngine;
using UnityEngine.SceneManagement;

public class Spacecraft : MonoBehaviour {
    private FixedJoint2D[] partJoints;
    private Rigidbody2D[] partRigidbodies;

    [SerializeField] private GameInput gameInput;

    private void Awake() {
        DontDestroyOnLoad(this);
    }
    
    private void Start() {
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
        // Find all part rigidbodies, joints, and engine scripts
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        Engine[] engineScripts = GetComponentsInChildren<Engine>();
        
        // Make all parts kinematic and disable joints for building
        foreach (Rigidbody2D partRb in partRigidbodies) {
            partRb.bodyType = RigidbodyType2D.Kinematic;
            partRb.simulated = false;
        }
        
        // Disable joints during building
        foreach (FixedJoint2D joint in partJoints) {
            joint.enabled = false;
        }
        
        
        // Disable engine
        foreach (Engine e in engineScripts) {
            e.enabled = false;
        }
    }
    
    private void SetFlightMode() {
        // Find all part rigidbodies and joints
        partRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        partJoints = GetComponentsInChildren<FixedJoint2D>();
        Engine[] engineScripts = GetComponentsInChildren<Engine>();
        
        // Enable physics for all parts
        foreach (Rigidbody2D partRb in partRigidbodies) {
            partRb.bodyType = RigidbodyType2D.Dynamic;
            partRb.simulated = true;
        }
        
        // Enable joints for flight
        foreach (FixedJoint2D joint in partJoints) {
            joint.enabled = true;
        }
        
        
        // Enable engines
        foreach (Engine e in engineScripts) {
            e.enabled = true;
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

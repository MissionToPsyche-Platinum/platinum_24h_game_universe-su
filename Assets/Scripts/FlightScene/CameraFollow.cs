using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Camera script that locks onto and follows the spacecraft in FlightScene.
/// This is attached to the camera in the flight scene.
/// </summary>
public class CameraFollow : MonoBehaviour {

    public static event EventHandler<AsteroidPassingEventArgs> OnAsteroidPassing;
    
    public class AsteroidPassingEventArgs : EventArgs {
        public GameObject asteroid;
        public bool isEntering;
    
        public AsteroidPassingEventArgs(GameObject asteroid, bool isEntering) {
            this.asteroid = asteroid;
            this.isEntering = isEntering;
        }
    }
    
    [Header("Follow Settings")]
    [Tooltip("The target to follow (will auto-find Spacecraft if not assigned)")]
    [SerializeField] private Transform target;
    
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    [Tooltip("Whether to follow the target immediately on start")]
    [SerializeField] private bool followOnStart = true;
    
    private Spacecraft spacecraft;
    private bool isFollowing = false;
    private Camera cam;
    private Rigidbody2D targetRb;
    
    private void Awake() {
        cam = GetComponent<Camera>();
        
        // If target is already assigned, we can use it
        if (target != null) {
            isFollowing = true;
        }
    }
    
    private void Start() {
        // find spacecraft
        if (target == null) {
            StartCoroutine(FindSpacecraftCoroutine());
        }
    }
    
    private IEnumerator FindSpacecraftCoroutine() {
        // Only look for spacecraft in FlightScene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene != "FlightScene") {
            yield break;
        }
        
        // Keep trying to find the spacecraft
        while (spacecraft == null || target == null) {
            spacecraft = Spacecraft.GetInstance();
            
            if (spacecraft == null) {
                spacecraft = FindFirstObjectByType<Spacecraft>();
            }
            
            if (spacecraft != null) {
                // Get the Rigidbody2D transform as the target
                Rigidbody2D rb = spacecraft.GetComponentInChildren<Rigidbody2D>();
                if (rb != null) {
                    target = rb.transform;
                    targetRb = rb;
                    isFollowing = true;
                    Debug.Log("CameraFollow: Successfully locked onto Spacecraft!");
                    break;
                }
            }
            
            // Wait a bit before trying again
            yield return new WaitForSeconds(0.1f);
        }
        
        // If followOnStart is true, immediately snap to target position
        if (followOnStart && target != null) {
            transform.position = target.position + offset;
        }
    }
    
    private void LateUpdate() {
        if (!isFollowing || target == null) return;

        // Rigidbody2D interpolation handles smooth rendering between physics steps,
        // so we can follow the target position directly with no lag
        transform.position = target.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D objectCollider) {
        GameObject otherObject = objectCollider.gameObject;
        
        //Only runs if otherObject is an asteroid
        if (otherObject.GetComponent<AsteroidFlight>() == null) return; 
        
        OnAsteroidPassing?.Invoke(this, new AsteroidPassingEventArgs(otherObject, true));
    }
    
    private void OnTriggerExit2D(Collider2D objectCollider) {
        GameObject otherObject = objectCollider.gameObject;
        
        //Only runs if otherObject is an asteroid
        if (otherObject.GetComponent<AsteroidFlight>() == null) return; 
        
        OnAsteroidPassing?.Invoke(this, new AsteroidPassingEventArgs(otherObject, false));
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
        isFollowing = newTarget != null;
    }
    
    public void StopFollowing() {
        isFollowing = false;
    }
    
    public void StartFollowing() {
        if (target != null) {
            isFollowing = true;
        }
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// Camera script that locks onto and follows the spacecraft in FlightScene.
/// This is attached to the camera in the flight scene.
/// </summary>
public class CameraFollow : MonoBehaviour {
    
    [Header("Follow Settings")]
    [Tooltip("The target to follow (will auto-find Spacecraft if not assigned)")]
    [SerializeField] private Transform target;
    
    [Tooltip("Smoothing speed for camera movement")]
    [SerializeField] private float smoothSpeed = 5f;
    
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    [Tooltip("Whether to follow the target immediately on start")]
    [SerializeField] private bool followOnStart = true;
    
    private Spacecraft spacecraft;
    private bool isFollowing = false;
    private Camera cam;
    
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
        // Only follow if we have a target and are in FlightScene
        if (!isFollowing || target == null) {
            return;
        }
        
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene != "FlightScene") {
            // Stop following if not in FlightScene
            isFollowing = false;
            return;
        }
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move towards the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
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
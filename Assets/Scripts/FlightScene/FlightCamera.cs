using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Camera script that locks onto and follows the spacecraft in FlightScene.
/// This is attached to the camera in the flight scene.
/// </summary>
public class FlightCamera : MonoBehaviour {
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
    
    private void Awake() {
        target = Spacecraft.GetInstance().transform;
        
        SetColliderSize();
    }
    
    private void SetColliderSize() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Camera cam = GetComponent<Camera>();

        const float DEFAULT_CAM_WIDTH = 3.55f;
        const float DEFAULT_CAM_HEIGHT = 2f;
        const float ADDITIONAL_SIZE = .75f;
        float boxX = cam.orthographicSize * DEFAULT_CAM_WIDTH + ADDITIONAL_SIZE;
        float boxY = cam.orthographicSize * DEFAULT_CAM_HEIGHT + ADDITIONAL_SIZE;

        boxCollider.size = new Vector2(boxX, boxY);
    }
    
    private void LateUpdate() {
        if (target == null) return;
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
}
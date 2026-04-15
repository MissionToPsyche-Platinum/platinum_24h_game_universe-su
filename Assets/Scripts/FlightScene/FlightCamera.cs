using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Camera script that locks onto and follows the spacecraft in FlightScene.
/// This is attached to the camera in the flight scene.
/// </summary>
public class FlightCamera : MonoBehaviour {
    
    private const float DEFAULT_CAM_WIDTH = 3.55f;
    private const float DEFAULT_CAM_HEIGHT = 2f;
    public static event EventHandler<AsteroidPassingEventArgs> OnAsteroidPassing;
    
    public class AsteroidPassingEventArgs : EventArgs {
        public GameObject asteroid;
        public bool isEntering;
    
        public AsteroidPassingEventArgs(GameObject asteroid, bool isEntering) {
            this.asteroid = asteroid;
            this.isEntering = isEntering;
        }
    }

    [SerializeField] private float transitionToPsycheSpeed;
    
    [Tooltip("Offset from the target position")]
    [SerializeField] private Vector3 offset;
    
    private Transform spacecraft;
    private Transform psycheAsteroid;
    private Transform target;
    private bool transitionToPsyche = false;
    
    private void Awake() {
        spacecraft = Spacecraft.GetInstance().transform;
        psycheAsteroid = PsycheAsteroid.Instance.transform;
        target = spacecraft;
        
        SetColliderSize();
    }
    
    private void Start() {
        OrbitAssist.OnEnteredOrbit += OrbitAssist_OnEnteredOrbit;
    }
    
    private void SetColliderSize() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        
        const float ADDITIONAL_SIZE = .75f;
        float boxX = GetCameraWidth() + ADDITIONAL_SIZE;
        float boxY = GetCameraHeight() + ADDITIONAL_SIZE;

        boxCollider.size = new Vector2(boxX, boxY);
    }
    
    private void LateUpdate() {
        if (transitionToPsyche) {
            TransitionToPsyche();
            return;
        }

        if (!target) return;
        transform.position = target.position + offset;
    }

    private void TransitionToPsyche() {
        Vector3 targetPos = target.position + offset;
        
        transform.position = Vector3.Lerp(transform.position, targetPos, transitionToPsycheSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPos) <= .05f) transitionToPsyche = false;
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
    
    private void OrbitAssist_OnEnteredOrbit(object sender, System.EventArgs e) {
        target = psycheAsteroid;
        transitionToPsyche = true;
    }

    public float GetCameraWidth() => GetComponent<Camera>().orthographicSize * DEFAULT_CAM_WIDTH;
    
    public float GetCameraHeight() => GetComponent<Camera>().orthographicSize * DEFAULT_CAM_HEIGHT;
}
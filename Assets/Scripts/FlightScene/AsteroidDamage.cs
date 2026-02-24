using System;
using UnityEngine;

/// <summary>
/// Script that damages the spacecraft when it collides with an asteroid.
/// this is attached to asteroids.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AsteroidDamage : MonoBehaviour {
    
    [Header("Damage Settings")]
    [Tooltip("Amount of damage to deal to the spacecraft on collision")]
    [SerializeField] private float damage = 10f;
    
    [Header("Behavior Settings")]
    [Tooltip("Destroy this asteroid on collision with spacecraft")]
    [SerializeField] private bool destroyOnCollision = false;
    
    [Tooltip("Disable this component after first collision")]
    [SerializeField] private bool disableAfterCollision = false;
    
    private float lastDamageTime = -1f;
    private float damageCooldown;
    

    //Start func is used for this bc AsteroidController Instance is defined after this Awake() method is called. 
    private void Start() => damageCooldown = AsteroidController.Instance.GetDamageCoolDown();
    
    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision.gameObject);
    
    private void HandleCollision(GameObject other) {
        // Only work in FlightScene
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene != "FlightScene") {
            return;
        }
        
        // Check cooldown
        if (damageCooldown > 0f && Time.time < lastDamageTime + damageCooldown) {
            return;
        }
        
        // Check if the colliding object is part of the spacecraft
        Spacecraft spacecraft = GetSpacecraftFromGameObject(other);
        
        if (spacecraft != null) {
            // Deal damage to the spacecraft
            spacecraft.TakeDamage(damage);
            lastDamageTime = Time.time;
            
            Debug.Log($"Asteroid dealt {damage} damage to spacecraft! Health: {spacecraft.CurrentHealth}/{spacecraft.MaxHealth}");
            
            // Handle post-collision behavior
            if (destroyOnCollision) {
                Destroy(gameObject);
            } else if (disableAfterCollision) {
                enabled = false;
            }
        }
    }
    
    private Spacecraft GetSpacecraftFromGameObject(GameObject obj) {
        if (obj == null) return null;
        
        // Check if this GameObject has a Spacecraft component
        Spacecraft spacecraft = obj.GetComponent<Spacecraft>();
        if (spacecraft != null) {
            return spacecraft;
        }
        
        // Check if this GameObject is a child of the spacecraft
        Transform parent = obj.transform.parent;
        while (parent != null) {
            spacecraft = parent.GetComponent<Spacecraft>();
            if (spacecraft != null) {
                return spacecraft;
            }
            parent = parent.parent;
        }
        
        // Check all parts of the spacecraft using the static instance
        Spacecraft spacecraftInstance = Spacecraft.GetInstance();
        if (spacecraftInstance != null) {
            // Check if the colliding object is part of the spacecraft
            Transform objTransform = obj.transform;
            Transform spacecraftTransform = spacecraftInstance.transform;
            
            // Check if obj is a child of spacecraft
            if (objTransform.IsChildOf(spacecraftTransform)) {
                return spacecraftInstance;
            }
        }
        
        return null;
    }
    
    public void SetDamage(float newDamage) {
        damage = newDamage;
    }
    
    public void SetDestroyOnCollision(bool shouldDestroy) {
        destroyOnCollision = shouldDestroy;
    }
}
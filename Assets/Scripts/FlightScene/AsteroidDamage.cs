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
    private int spacecraftLayer;
    

    //Start func is used for this bc AsteroidController Instance is defined after this Awake() method is called.
    private void Start() {
        damageCooldown = AsteroidController.Instance.GetDamageCoolDown();
        spacecraftLayer = LayerMask.NameToLayer("SpaceCraft");
    }
    
    private void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision.gameObject);
    
    private void HandleCollision(GameObject other) {
        // Only damage objects on the SpaceCraft layer
        if (other.layer != spacecraftLayer) {
            return;
        }

        // Check cooldown
        if (damageCooldown > 0f && Time.time < lastDamageTime + damageCooldown) {
            return;
        }

        Spacecraft spacecraft = Spacecraft.GetInstance();
        if (spacecraft == null) return;

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
    
    public void SetDamage(float newDamage) {
        damage = newDamage;
    }
    
    public void SetDestroyOnCollision(bool shouldDestroy) {
        destroyOnCollision = shouldDestroy;
    }
}
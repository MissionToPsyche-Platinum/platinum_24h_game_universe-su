using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI component that displays the spacecraft's health as a red bar.
/// </summary>
[RequireComponent(typeof(Image))]
public class HealthBarUI : MonoBehaviour {
    
    [Header("Health Bar Settings")]
    [SerializeField] private Color healthColor = Color.red;
    [Tooltip("Optional: background image for the health bar container")]
    [SerializeField] private Image backgroundImage;
    
    private Image healthBarImage;
    private Spacecraft spacecraft;
    private bool isSubscribed = false;
    
    private void Awake() {
        healthBarImage = GetComponent<Image>();
        
        // Set the color to red
        healthBarImage.color = healthColor;
        
        // Set image type to Filled for smooth health bar animation
        healthBarImage.type = Image.Type.Filled;
        healthBarImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Initialize to full health so it's visible
        healthBarImage.fillAmount = 1f;
    }
    
    private void Start() {
        // Start coroutine to find spacecraft
        StartCoroutine(FindSpacecraftCoroutine());
    }
    
    private IEnumerator FindSpacecraftCoroutine() {
        // Keep trying to find the spacecraft until found
        while (spacecraft == null) {
            // Try using the static instance method first
            spacecraft = Spacecraft.GetInstance();
            
            // If still null, try FindFirstObjectByType as fallback
            if (spacecraft == null) {
                spacecraft = FindFirstObjectByType<Spacecraft>();
            }
            
            if (spacecraft == null) {
                // Wait a bit before trying again
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // Once found, subscribe to events
        if (spacecraft != null && !isSubscribed) {
            spacecraft.OnHealthChanged += Spacecraft_OnHealthChanged;
            isSubscribed = true;
            
            // Initialize health bar with current health
            UpdateHealthBar(spacecraft.HealthPercentage);
            
            Debug.Log("HealthBarUI: Successfully connected to Spacecraft!");
        }
    }

    
    private void Spacecraft_OnHealthChanged(object sender, float healthPercentage) {
        UpdateHealthBar(healthPercentage);
    }
    
    private void UpdateHealthBar(float healthPercentage) {
        if (healthBarImage != null) {
            // Clamp health percentage between 0 and 1
            healthPercentage = Mathf.Clamp01(healthPercentage);
            
            // Update the fill amount (0 = empty, 1 = full)
            healthBarImage.fillAmount = healthPercentage;
        }
    }
    
    private void OnEnable() {
        // when scene loads, try to reconnect
        if (spacecraft == null || !isSubscribed) {
            StartCoroutine(FindSpacecraftCoroutine());
        } else if (spacecraft != null && isSubscribed) {
            // Update health bar immediately if already connected
            UpdateHealthBar(spacecraft.HealthPercentage);
        }
    }
    
    private void OnDestroy() {
        // Unsubscribe from events to prevent memory leaks
        if (spacecraft != null && isSubscribed) {
            spacecraft.OnHealthChanged -= Spacecraft_OnHealthChanged;
            isSubscribed = false;
        }
    }
}
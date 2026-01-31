using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI component that displays the spacecraft's health as a red bar.
/// </summary>
[RequireComponent(typeof(Image))]
public class HealthBarUI : MonoBehaviour {
    
    [Tooltip("Optional: background image for the health bar container")]
    [SerializeField] private Image backgroundImage;
    
    private Image healthBarImage;
    private Spacecraft spacecraft;
    private bool isSubscribed = false;
    
    private void Awake() {
        spacecraft = Spacecraft.GetInstance();
        
        healthBarImage = GetComponent<Image>();
        
        // Initialize to full health so it's visible
        healthBarImage.fillAmount = 1f;
        UpdateHealthBar(spacecraft.HealthPercentage);
    }
    
    private void Start() {
        spacecraft.OnHealthChanged += Spacecraft_OnHealthChanged;
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
    
    private void OnDestroy() {
        // Unsubscribe from events to prevent memory leaks
        if (spacecraft != null && isSubscribed) {
            spacecraft.OnHealthChanged -= Spacecraft_OnHealthChanged;
            isSubscribed = false;
        }
    }
}
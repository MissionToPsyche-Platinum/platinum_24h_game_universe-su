using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI component that displays the engine's fuel as a colored bar.
/// Works similarly to HealthBarUI, but listens for fuel changes instead.
/// </summary>
[RequireComponent(typeof(Image))]
public class FuelBarUI : MonoBehaviour
{
    [Header("Fuel Bar Settings")]
    [SerializeField] private Color fuelColor = Color.yellow;

    [Tooltip("Optional: background image for the fuel bar container")]
    [SerializeField] private Image backgroundImage;

    // Reference to the Image component that visually represents the fuel bar fill
    private Image fuelBarImage;

    // Reference to the Engine that owns the fuel
    private Engine engine;

    // Tracks whether we have successfully subscribed to the engine's fuel event
    private bool isSubscribed = false;

    private void Awake()
    {
        // Cache the Image component on this GameObject
        fuelBarImage = GetComponent<Image>();

        // Set the bar color (yellow by default)
        fuelBarImage.color = fuelColor;

        // Configure this image to behave like a horizontal fill bar
        fuelBarImage.type = Image.Type.Filled;
        fuelBarImage.fillMethod = Image.FillMethod.Horizontal;
        fuelBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        // Initialize to full so the bar is visible immediately
        fuelBarImage.fillAmount = 1f;
    }

    private void Start()
    {
        // Start coroutine to locate the Engine in the scene
        StartCoroutine(FindEngineCoroutine());
    }

    // Continuously attempts to find the Engine object until it exists,
    // then subscribes to its fuel change event.
    private IEnumerator FindEngineCoroutine()
    {
        // Keep trying to find the engine until found
        while (engine == null)
        {
            engine = FindFirstObjectByType<Engine>();

            if (engine == null)
            {
                // Wait briefly before trying again
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Once found, subscribe to the fuel change event
        if (engine != null && !isSubscribed)
        {
            engine.OnFuelChanged += Engine_OnFuelChanged;
            isSubscribed = true;

            // Initialize fuel bar with current fuel level
            UpdateFuelBar(engine.FuelPercentage);

            Debug.Log("FuelBarUI: Successfully connected to Engine!");
        }
    }


    // Called whenever the engine reports that fuel has changed.
    private void Engine_OnFuelChanged(object sender, float fuelPercentage)
    {
        UpdateFuelBar(fuelPercentage);
    }


    // Updates the visual fill amount of the fuel bar.
    private void UpdateFuelBar(float fuelPercentage)
    {
        if (fuelBarImage != null)
        {
            // Clamp fuel percentage between 0 and 1
            fuelPercentage = Mathf.Clamp01(fuelPercentage);

            // Update the fill amount (0 = empty, 1 = full)
            fuelBarImage.fillAmount = fuelPercentage;
        }
    }

    private void OnEnable()
    {
        // When scene loads or object is re-enabled, try to reconnect
        if (engine == null || !isSubscribed)
        {
            StartCoroutine(FindEngineCoroutine());
        }
        else if (engine != null && isSubscribed)
        {
            // Update fuel bar immediately if already connected
            UpdateFuelBar(engine.FuelPercentage);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (engine != null && isSubscribed)
        {
            engine.OnFuelChanged -= Engine_OnFuelChanged;
            isSubscribed = false;
        }
    }
}

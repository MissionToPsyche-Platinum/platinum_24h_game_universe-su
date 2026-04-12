using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays the spacecraft's fuel as a filled bar.
/// </summary>
[RequireComponent(typeof(Image))]
public class FuelBarUI : MonoBehaviour {

    [Tooltip("Background image for the fuel bar container")]
    [SerializeField] private Image backgroundImage;

    private Image fuelBarImage;
    private Spacecraft spacecraft;

    private void Awake() {
        spacecraft = Spacecraft.GetInstance();
        fuelBarImage = GetComponent<Image>();

        fuelBarImage.fillAmount = 1f;
    }

    private void Start() {
        spacecraft.OnFuelChanged += Spacecraft_OnFuelChanged;
        UpdateFuelBar(spacecraft.FuelPercentage);
    }

    private void Spacecraft_OnFuelChanged(object sender, float fuelPercentage) {
        UpdateFuelBar(fuelPercentage);
    }

    private void UpdateFuelBar(float fuelPercentage) {
        if (fuelBarImage != null) {
            fuelBarImage.fillAmount = Mathf.Clamp01(fuelPercentage);
        }
    }

    private void OnDestroy() {
        if (spacecraft != null) {
            spacecraft.OnFuelChanged -= Spacecraft_OnFuelChanged;
        }
    }
}

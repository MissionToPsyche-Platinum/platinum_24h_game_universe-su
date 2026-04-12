using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays the spacecraft's energy as a filled bar.
/// </summary>
[RequireComponent(typeof(Image))]
public class EnergyBarUI : MonoBehaviour {

    [Tooltip("Background image for the energy bar container")]
    [SerializeField] private Image backgroundImage;

    private Image energyBarImage;
    private Spacecraft spacecraft;

    private void Awake() {
        spacecraft = Spacecraft.GetInstance();
        energyBarImage = GetComponent<Image>();

        energyBarImage.fillAmount = 1f;
    }

    private void Start() {
        spacecraft.OnEnergyChanged += Spacecraft_OnEnergyChanged;
        UpdateEnergyBar(spacecraft.EnergyPercentage);
    }

    private void Spacecraft_OnEnergyChanged(object sender, float energyPercentage) {
        UpdateEnergyBar(energyPercentage);
    }

    private void UpdateEnergyBar(float energyPercentage) {
        if (energyBarImage != null) {
            energyBarImage.fillAmount = Mathf.Clamp01(energyPercentage);
        }
    }

    private void OnDestroy() {
        if (spacecraft != null) {
            spacecraft.OnEnergyChanged -= Spacecraft_OnEnergyChanged;
        }
    }
}

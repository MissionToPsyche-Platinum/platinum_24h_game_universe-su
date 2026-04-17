using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays the spacecraft's fuel as a filled bar.
/// </summary>
[RequireComponent(typeof(Image))]
public class FuelBarUI : MonoBehaviour {

    [Tooltip("Background image for the fuel bar container")]
    [SerializeField] private Image backgroundImage;

    [Header("Tank Dividers")]
    [Tooltip("Width of each tick mark in pixels")]
    [SerializeField] private float tickWidth = 2f;

    [Tooltip("Color of the tick marks separating tank capacities")]
    [SerializeField] private Color tickColor = new Color(0f, 0f, 0f, 0.7f);

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
        CreateTankDividers();
    }

    private void CreateTankDividers() {
        int tankCount = spacecraft.GetComponentsInChildren<FuelTank>().Length;
        if (tankCount <= 1) return;

        for (int i = 1; i < tankCount; i++) {
            GameObject tick = new GameObject($"TankDivider_{i}", typeof(RectTransform), typeof(Image));
            tick.transform.SetParent(transform, false);

            Image tickImage = tick.GetComponent<Image>();
            tickImage.color = tickColor;
            tickImage.raycastTarget = false;

            RectTransform tickRect = tick.GetComponent<RectTransform>();
            float normalizedX = (float)i / tankCount;
            tickRect.anchorMin = new Vector2(normalizedX, 0f);
            tickRect.anchorMax = new Vector2(normalizedX, 1f);
            tickRect.pivot = new Vector2(0.5f, 0.5f);
            tickRect.anchoredPosition = Vector2.zero;
            tickRect.sizeDelta = new Vector2(tickWidth, 0f);
        }
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

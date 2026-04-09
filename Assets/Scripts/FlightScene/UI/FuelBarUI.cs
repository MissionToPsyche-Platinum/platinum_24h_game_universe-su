using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class FuelBarUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    private Image fuelBarImage;
    private List<Engine> engines = new List<Engine>();
    private bool isSubscribed = false;

    private void Awake()
    {
        fuelBarImage = GetComponent<Image>();
        fuelBarImage.fillAmount = 1f;
    }

    private void Start()
    {
        StartCoroutine(FindEnginesCoroutine());
    }

    private IEnumerator FindEnginesCoroutine()
    {
        while (engines.Count == 0)
        {
            Engine[] found = FindObjectsByType<Engine>(FindObjectsSortMode.None);
            if (found.Length > 0)
            {
                engines.AddRange(found);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        foreach (Engine engine in engines)
        {
            engine.OnFuelChanged += Engine_OnFuelChanged;
        }
        isSubscribed = true;

        UpdateFuelBar(GetAverageFuel());
        Debug.Log("FuelBarUI: Successfully connected to " + engines.Count + " engines!");
    }

    private void Engine_OnFuelChanged(object sender, float fuelPercentage)
    {
        UpdateFuelBar(GetAverageFuel());
    }

    private float GetAverageFuel()
    {
        if (engines.Count == 0) return 0f;
        float total = 0f;
        foreach (Engine engine in engines)
        {
            if (engine != null) total += engine.FuelPercentage;
        }
        return total / engines.Count;
    }

    private void UpdateFuelBar(float fuelPercentage)
    {
        if (fuelBarImage != null)
        {
            fuelBarImage.fillAmount = Mathf.Clamp01(fuelPercentage);
        }
    }

    private void OnEnable()
    {
        if (engines.Count == 0 || !isSubscribed)
        {
            StartCoroutine(FindEnginesCoroutine());
        }
        else
        {
            UpdateFuelBar(GetAverageFuel());
        }
    }

    private void OnDestroy()
    {
        if (isSubscribed)
        {
            foreach (Engine engine in engines)
            {
                if (engine != null) engine.OnFuelChanged -= Engine_OnFuelChanged;
            }
            isSubscribed = false;
        }
    }
}
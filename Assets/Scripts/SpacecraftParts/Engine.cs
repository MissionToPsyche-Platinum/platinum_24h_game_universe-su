// Engine.cs
using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour
{
    [SerializeField] private int speed;

    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 10f;

    private GameInput gameInput;
    private Rigidbody2D engineRigidbody2D;

    // NOTE: Consider removing 'static' later if you ever have multiple Engine objects.
    private static bool active;

    private float fuelAmount;

    // Event + percentage for UI (HealthBar-style)
    public event System.EventHandler<float> OnFuelChanged;

    public float FuelPercentage
    {
        get
        {
            if (maxFuel <= 0f)
            {
                return 0f;
            }
            return fuelAmount / maxFuel;
        }
    }

    public void Awake()
    {
        enabled = false;

        fuelAmount = maxFuel;
        OnFuelChanged?.Invoke(this, FuelPercentage);

        engineRigidbody2D = GetComponentInParent<Rigidbody2D>();

        gameInput = GameInput.instance;
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction;
        gameInput.OnActivateEngineCanceledAction += GameInput_OnActivateEngineAction;
    }

    private void FixedUpdate()
    {
        if (!active)
        {
            return;
        }

        Debug.Log(fuelAmount); // Log current fuel amount for testing

        // Check and see if we have fuel
        if (fuelAmount <= 0f)
        {
            fuelAmount = 0f;
            OnFuelChanged?.Invoke(this, FuelPercentage);
            return; // no fuel so no thrust
        }

        // Apply thrust
        engineRigidbody2D.AddForce(speed * transform.up * Time.fixedDeltaTime);

        // Consume fuel AFTER thrust
        ConsumeFuel();
    }

    private void GameInput_OnActivateEngineAction(object sender, GameInput.EngineActivatedEventArgs e)
    {
        active = e.activated;
    }

    private void ConsumeFuel()
    {
        float fuelConsumptionRate = 1f; // consumption rate
        fuelAmount -= fuelConsumptionRate * Time.fixedDeltaTime;

        // Clamp and notify
        if (fuelAmount < 0f)
        {
            fuelAmount = 0f;
        }

        OnFuelChanged?.Invoke(this, FuelPercentage);
    }

    public float GetFuelAmount()
    {
        return fuelAmount;
    }
}


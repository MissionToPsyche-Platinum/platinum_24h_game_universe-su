using System;
using TMPro;
using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour {
    [SerializeField] private int speed;
    [SerializeField] private Rigidbody2D engineRigidbody2D;
    [SerializeField] private SpriteRenderer engineVisual;
    [SerializeField] private TextMeshProUGUI idUI;
    
    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 10f;
    
    public int engineID = -1;
    private GameInput gameInput;


    private bool active;
    private float fuelAmount;

    public event System.EventHandler<float> OnFuelChanged;


    public void Awake() {
        active = false;
        
        fuelAmount = maxFuel;
        gameInput = GameInput.Instance;
        
        OnFuelChanged?.Invoke(this, FuelPercentage);
        
        SetEngineID();
    }

    public void Start() {
        gameInput.OnEnginePerformedAction += GameInput_OnEngineAction; //Adds GameInput_OnEngineAction() as a listener to the OnEngineAction event. 
        gameInput.OnEngineCanceledAction += GameInput_OnEngineAction;
    }

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
    
    private void SetEngineID() {
        engineID = SpacecraftPartDatabase.Instance.CreateEngineID();
        idUI.text = engineID.ToString();
    }

    private void GameInput_OnEngineAction(object sender, GameInput.EngineEventArgs e) { 
        if(engineID == e.engineNum) {
            active = e.activated;
            if (active) engineVisual.color = Color.red;
            else engineVisual.color = Color.yellow;
        }
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
}

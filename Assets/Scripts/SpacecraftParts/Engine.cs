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

    [SerializeField] private int _engineID;

    public int engineID {
        get => _engineID;
        set {
            _engineID = value;
            idUI.text = value.ToString();
        }
    }
    
    public static int totalEngineCount;
    private GameInput gameInput;
    private bool active;
    private float fuelAmount;

    public event System.EventHandler<float> OnFuelChanged;


    public void Awake() {
        totalEngineCount++;
        engineID = totalEngineCount;
        
        fuelAmount = maxFuel;
        gameInput = GameInput.Instance;
        
        OnFuelChanged?.Invoke(this, FuelPercentage);
    }

    public void Start() {
        gameInput.OnEnginePerformedAction += GameInput_OnEngineAction; //Adds GameInput_OnEngineAction() as a listener to the OnEngineAction event. 
        gameInput.OnEngineCanceledAction += GameInput_OnEngineAction;
    }

    public float FuelPercentage {
        get {
            if (maxFuel <= 0f) return 0f;
            
            return fuelAmount / maxFuel;
        }
    }
    
    private void FixedUpdate() {
        if(active && TryConsumeFuel()) ActivateEngine(); //Only activates if engine is active and there is fuel.
    }

    private void ActivateEngine() => engineRigidbody2D.AddForce(transform.up * (speed * Time.fixedDeltaTime));
    
    private void GameInput_OnEngineAction(object sender, GameInput.EngineEventArgs e) { 
        if(engineID == e.engineNum) active = e.activated;
            
        if (active) engineVisual.color = Color.red;
        else engineVisual.color = Color.yellow;
    }
    
    private bool TryConsumeFuel() {
        float fuelConsumptionRate = 1f; // consumption rate
        fuelAmount -= fuelConsumptionRate * Time.fixedDeltaTime;

        // Clamp
        if (fuelAmount < 0f) {
            fuelAmount = 0f;
            return false;
        }
        
        OnFuelChanged?.Invoke(this, FuelPercentage);

        return true;
    }

    private void OnDestroy() {
        gameInput.OnEnginePerformedAction -= GameInput_OnEngineAction;
        gameInput.OnEngineCanceledAction -= GameInput_OnEngineAction;
    }
}

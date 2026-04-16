using System;
using TMPro;
using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour {
    [SerializeField] private int speed;
    [SerializeField] private SpriteRenderer engineVisual;
    [SerializeField] private TextMeshProUGUI idUI;
    
    [Header("Fuel Settings")]
    [SerializeField] float fuelCostPerSecond = 1f;

    [Header("Energy Settings")]
    [SerializeField] private float energyCostPerSecond = 1f;

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
    private Spacecraft spacecraft;
    private Rigidbody2D spacecraftRB;
    private bool active;
    private float fuelAmount;

    public event System.EventHandler<float> OnFuelChanged;


    public void Awake() {
        totalEngineCount++;
        engineID = totalEngineCount;

        gameInput = GameInput.Instance;
        spacecraft = Spacecraft.GetInstance();
        spacecraftRB = spacecraft.gameObject.GetComponent<Rigidbody2D>();
    }

    public void Start() {
        gameInput.OnEnginePerformedAction += GameInput_OnEngineAction; //Adds GameInput_OnEngineAction() as a listener to the OnEngineAction event. 
        gameInput.OnEngineCanceledAction += GameInput_OnEngineAction;
    }
    
    private void FixedUpdate() {
        if (active && TryConsumeEnergy() && TryConsumeFuel()) ActivateEngine();
    }

    private void ActivateEngine() => spacecraftRB.AddForceAtPosition(transform.up * speed, transform.position);
    
    private void GameInput_OnEngineAction(object sender, GameInput.EngineEventArgs e) { 
        if(engineID == e.engineNum) active = e.activated;
            
        if (active) engineVisual.color = Color.red;
        else engineVisual.color = Color.yellow;
    }

    private bool TryConsumeFuel() {
        if (spacecraft == null) return false;
        float fuelCost = fuelCostPerSecond * Time.fixedDeltaTime;
        return spacecraft.TryConsumeFuel(fuelCost);
    }
    
    private void AdjustEngineIDsForDeletion(GameObject engineToBeDeleted) {
        if (!engineToBeDeleted.TryGetComponent<Engine>(out Engine deletedEngine)) return;
        int engineID = deletedEngine.engineID;
        int totalEngines = totalEngineCount;

        totalEngineCount = Math.Max(0, totalEngineCount - 1);

        if (engineID == totalEngines) return;
        if (spacecraft == null) return;
        
        foreach (Transform child in spacecraft.transform) {
            if (!child.TryGetComponent(out Engine otherEngine)) continue;

            if (otherEngine.engineID > engineID) otherEngine.engineID--;
        }
    }

    private bool TryConsumeEnergy() {
        if (spacecraft == null) return false;
        float energyCost = energyCostPerSecond * Time.fixedDeltaTime;
        return spacecraft.TryConsumeEnergy(energyCost);
    }

    private void OnDestroy() {
        gameInput.OnEnginePerformedAction -= GameInput_OnEngineAction;
        gameInput.OnEngineCanceledAction -= GameInput_OnEngineAction;
        AdjustEngineIDsForDeletion(gameObject);
    }
}

using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour
{
    [SerializeField] private int speed;
    private GameInput gameInput;
    private Rigidbody2D engineRigidbody2D;
    private static bool active;
    private float fuelAmount = 10f; // Fuel amount

    public void Awake()
    {
        enabled = false;
        engineRigidbody2D = GetComponentInParent<Rigidbody2D>();
        gameInput = GameInput.instance;
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction; //Adds GameInput_OnActivateEngineAction() as a listener to the OnActivateEngineAction event. 
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
            return; // no fuel so no thrust
        }

        // Apply thrust
        engineRigidbody2D.AddForce(
            speed * transform.up * Time.fixedDeltaTime
        );

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
        fuelAmount -= fuelConsumptionRate * Time.deltaTime;
    }
}

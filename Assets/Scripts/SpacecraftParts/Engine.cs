using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour {
    [SerializeField] private int speed;
    private GameInput gameInput;
    private Rigidbody2D engineRigidbody2D;
    private static bool active;

    public void Awake()
    {
        enabled = false;
        engineRigidbody2D =  GetComponentInParent<Rigidbody2D>();
        gameInput = GameInput.instance;
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction; //Adds GameInput_OnActivateEngineAction() as a listener to the OnActivateEngineAction event. 
        gameInput.OnActivateEngineCanceledAction += GameInput_OnActivateEngineAction;
    }
    private void FixedUpdate()
    {
        if (active)
        {
            engineRigidbody2D.AddForce(speed * transform.up * Time.deltaTime);
        }
    }

    private void GameInput_OnActivateEngineAction(object sender, GameInput.EngineActivatedEventArgs e) { 
        active = e.activated;
    }
}

using System;
using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour {
    [SerializeField] private int speed;
    [SerializeField] private SpriteRenderer engineVisual;
    
    public int engineID = -1;
    private GameInput gameInput;
    private Rigidbody2D engineRigidbody2D;
    private static bool active;

    public void Awake() {
        active = false;
        engineRigidbody2D = GetComponentInParent<Rigidbody2D>();
        gameInput = GameInput.Instance;
        engineID = SpacecraftPartDatabase.Instance.CreateEngineID();
    }

    public void Start() {
        gameInput.OnEnginePerformedAction += GameInput_OnEngineAction; //Adds GameInput_OnEngineAction() as a listener to the OnEngineAction event. 
        gameInput.OnEngineCanceledAction += GameInput_OnEngineAction;
    }
    
    private void FixedUpdate() {
        if (active) engineRigidbody2D.AddForce(speed * transform.up * Time.deltaTime);
    }

    private void GameInput_OnEngineAction(object sender, GameInput.EngineEventArgs e) { 
        if(engineID == e.engineNum) {
            active = e.activated;
            if (active) engineVisual.color = Color.red;
            else engineVisual.color = Color.yellow;
        }
    }
}

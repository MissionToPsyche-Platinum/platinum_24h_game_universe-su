using UnityEngine;

public class Spacecraft : MonoBehaviour {
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Engine engine;
    
    private void Start() {
        gameInput.OnActivateEnginePerformedAction += GameInput_OnActivateEngineAction; //Adds GameInput_OnActivateEngineAction() as a listener to the OnActivateEngineAction event. 
        gameInput.OnActivateEngineCanceledAction += GameInput_OnActivateEngineAction;
    }
    
    private void GameInput_OnActivateEngineAction(object sender, GameInput.EngineActivatedEventArgs e) {
        engine.enabled = e.activated;
    }
}

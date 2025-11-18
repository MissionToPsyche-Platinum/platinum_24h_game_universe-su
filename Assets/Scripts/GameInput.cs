using UnityEngine;
using System;

public class GameInput : MonoBehaviour {
    public event EventHandler<EngineActivatedEventArgs> OnActivateEnginePerformedAction;
    public event EventHandler<EngineActivatedEventArgs> OnActivateEngineCanceledAction;


    
    private InputSystem_Actions inputActions;
    private bool engineActivated = false;
    
    public class EngineActivatedEventArgs : EventArgs {
        public bool activated;

        public EngineActivatedEventArgs(bool activated) => this.activated = activated;
    }
    
    public void Awake() {
        inputActions = new InputSystem_Actions();

        inputActions.Spacecraft.Enable();
        inputActions.Spacecraft.ActivateEngine.performed += ActivateEngine_performed;
        inputActions.Spacecraft.ActivateEngine.canceled += ActivateEngine_canceled;
    }
    
    private void ActivateEngine_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnActivateEnginePerformedAction?.Invoke(this, new EngineActivatedEventArgs(true)); //"?.Invoke" basically checks if theres any listeners (methods). If there are listeners, calls all of 'em.
    }
    
    private void ActivateEngine_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnActivateEngineCanceledAction?.Invoke(this, new EngineActivatedEventArgs(false)); //"?.Invoke" basically checks if theres any listeners (methods). If there are listeners, calls all of 'em.
    }
    
}

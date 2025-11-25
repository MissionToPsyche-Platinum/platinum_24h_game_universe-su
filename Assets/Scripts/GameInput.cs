using UnityEngine;
using System;
using UnityEditor;

public class GameInput : MonoBehaviour {
    public event EventHandler<EngineActivatedEventArgs> OnActivateEnginePerformedAction;
    public event EventHandler<EngineActivatedEventArgs> OnActivateEngineCanceledAction;
    public event EventHandler<NumKeyEventArgs> OnNumKeyPerformedAction;

    public event EventHandler OnLeftMouseClickPerformedAction;

    
    private InputSystem_Actions inputActions;
    
    public class EngineActivatedEventArgs : EventArgs {
        public bool activated;

        public EngineActivatedEventArgs(bool activated) => this.activated = activated;
    }
    
    public class NumKeyEventArgs : EventArgs {
        public int key;

        public NumKeyEventArgs(int key) => this.key = key;
    }
    
    public void Awake() {
        inputActions = new InputSystem_Actions();

        inputActions.Spacecraft.Enable();
        inputActions.Spacecraft.ActivateEngine.performed += ActivateEngine_performed;
        inputActions.Spacecraft.ActivateEngine.canceled += ActivateEngine_canceled;
        
        inputActions.Spacecraft.KeyOne.performed += KeyOne_performed;
        inputActions.Spacecraft.KeyTwo.performed += KeyTwo_performed;
        inputActions.Spacecraft.KeyThree.performed += KeyThree_performed;
        inputActions.Spacecraft.KeyFour.performed += KeyFour_performed;
        inputActions.Spacecraft.KeyFive.performed += KeyFive_performed;
        inputActions.Spacecraft.KeySix.performed += KeySix_performed;
        inputActions.Spacecraft.KeySeven.performed += KeySeven_performed;
        inputActions.Spacecraft.KeyEight.performed += KeyEight_performed;



        inputActions.Spacecraft.LeftMouseClick.performed += LeftMouseClick_performed;
    }
    
    private void ActivateEngine_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnActivateEnginePerformedAction?.Invoke(this, new EngineActivatedEventArgs(true)); //"?.Invoke" basically checks if theres any listeners (methods). If there are listeners, calls all of 'em.
    }
    
    private void ActivateEngine_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnActivateEngineCanceledAction?.Invoke(this, new EngineActivatedEventArgs(false)); 
    }
    
    private void KeyOne_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(1)); 
    }
    
    private void KeyTwo_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(2)); 
    }

    private void KeyThree_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(3));
    }

    private void KeyFour_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(4));
    }

    private void KeyFive_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(5));
    }

    private void KeySix_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(6));
    }

    private void KeySeven_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(7));
    }

    private void KeyEight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnNumKeyPerformedAction?.Invoke(this, new NumKeyEventArgs(8));
    }
    private void LeftMouseClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnLeftMouseClickPerformedAction?.Invoke(this, EventArgs.Empty); 
    }
    
    private void OnDisable() {
        // Disable input actions when the component is disabled
        CleanupInputActions();
    }
    
    private void OnDestroy() {
        // Cleanup input actions when the object is destroyed
        CleanupInputActions();
    }
    
    private void CleanupInputActions() {
        // Properly disable and cleanup input actions
        if (inputActions != null) {
            // Unsubscribe from all events first
            inputActions.Spacecraft.ActivateEngine.performed -= ActivateEngine_performed;
            inputActions.Spacecraft.ActivateEngine.canceled -= ActivateEngine_canceled;
            
            inputActions.Spacecraft.KeyOne.performed -= KeyOne_performed;
            inputActions.Spacecraft.KeyTwo.performed -= KeyTwo_performed;
            inputActions.Spacecraft.KeyThree.performed -= KeyThree_performed;
            inputActions.Spacecraft.KeyFour.performed -= KeyFour_performed;
            inputActions.Spacecraft.KeyFive.performed -= KeyFive_performed;
            inputActions.Spacecraft.KeySix.performed -= KeySix_performed;
            inputActions.Spacecraft.KeySeven.performed -= KeySeven_performed;
            inputActions.Spacecraft.KeyEight.performed -= KeyEight_performed;
            inputActions.Spacecraft.LeftMouseClick.performed -= LeftMouseClick_performed;
            
            // Always disable the action maps (safe to call even if already disabled)
            inputActions.Spacecraft.Disable();
            
            // Disable the entire input system and dispose
            inputActions.Disable();
            inputActions.Dispose();
            inputActions = null;
        }
    }
}

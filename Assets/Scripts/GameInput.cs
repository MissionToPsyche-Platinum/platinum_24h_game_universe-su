using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

//Class that handles input and triggers events based on it.

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }
    public event EventHandler<EngineEventArgs> OnEnginePerformedAction;
    public event EventHandler<EngineEventArgs> OnEngineCanceledAction;
    public event EventHandler<NumKeyEventArgs> OnNumKeyPerformedAction;

    public event EventHandler OnLeftMouseClickPerformedAction;

    public event EventHandler OnDeletePartPerformedAction;

    public event EventHandler OnSetFlightScenePerformedAction;
    
    private InputSystem_Actions inputActions;
    
    public class EngineEventArgs : EventArgs {
        public bool activated;
        public int engineNum;
        
        public EngineEventArgs(bool activated, int engineNum) {
            this.activated = activated;
            this.engineNum = engineNum;
        }
    }
    
    public class NumKeyEventArgs : EventArgs {
        public int key;
    
        public NumKeyEventArgs(int key) => this.key = key;
    }
    
    public void Awake() {
        Instance = this;
        DontDestroyOnLoad(this);
        
        inputActions = new InputSystem_Actions();
        
        if (SceneManager.GetActiveScene().name == "FlightScene") inputActions.Spacecraft.Enable();
        else inputActions.SpacecraftBuilding.Enable();
        
        inputActions.General.Enable();
    }

    public void Start() {
        inputActions.Spacecraft.EngineOne.performed += EngineOne_performed;
        inputActions.Spacecraft.EngineOne.canceled += EngineOne_canceled;
        inputActions.Spacecraft.EngineTwo.performed += EngineTwo_performed;
        inputActions.Spacecraft.EngineTwo.canceled += EngineTwo_canceled;
        inputActions.Spacecraft.EngineThree.performed += EngineThree_performed;
        inputActions.Spacecraft.EngineThree.canceled += EngineThree_canceled;
        inputActions.Spacecraft.EngineFour.performed += EngineFour_performed;
        inputActions.Spacecraft.EngineFour.canceled += EngineFour_canceled;
        
        inputActions.SpacecraftBuilding.KeyOne.performed += KeyOne_performed;
        inputActions.SpacecraftBuilding.KeyTwo.performed += KeyTwo_performed;
        inputActions.SpacecraftBuilding.KeyThree.performed += KeyThree_performed;
        inputActions.SpacecraftBuilding.KeyFour.performed += KeyFour_performed;
        inputActions.SpacecraftBuilding.KeyFive.performed += KeyFive_performed;
        inputActions.SpacecraftBuilding.KeySix.performed += KeySix_performed;
        inputActions.SpacecraftBuilding.KeySeven.performed += KeySeven_performed;
        inputActions.SpacecraftBuilding.KeyEight.performed += KeyEight_performed;

        inputActions.SpacecraftBuilding.DeletePart.performed += DeletePart_performed;
        inputActions.SpacecraftBuilding.LeftMouseClick.performed += LeftMouseClick_performed;
    
        inputActions.General.SceneSwitch.performed += SceneSwitch_performed;
    }
    
    private void EngineOne_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnEnginePerformedAction?.Invoke(this, new EngineEventArgs(true, 1)); //"?.Invoke" basically checks if theres any listeners (methods). If there are listeners, calls all of 'em.
    }
    
    private void EngineOne_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
       OnEngineCanceledAction?.Invoke(this, new EngineEventArgs(false, 1)); 
    }

    private void EngineTwo_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEnginePerformedAction?.Invoke(this, new EngineEventArgs(true, 2));
    }
     
    private void EngineTwo_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEngineCanceledAction?.Invoke(this, new EngineEventArgs(false, 2)); 
    }
     
    private void EngineThree_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEnginePerformedAction?.Invoke(this, new EngineEventArgs(true, 3));
    }

    private void EngineThree_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEngineCanceledAction?.Invoke(this, new EngineEventArgs(false, 3));
    }

    private void EngineFour_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEnginePerformedAction?.Invoke(this, new EngineEventArgs(true, 4)); 
    }
     
    private void EngineFour_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) { 
        OnEngineCanceledAction?.Invoke(this, new EngineEventArgs(false, 4)); 
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

    private void DeletePart_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnDeletePartPerformedAction?.Invoke(this, EventArgs.Empty);
    }
    
    private void LeftMouseClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnLeftMouseClickPerformedAction?.Invoke(this, EventArgs.Empty); 
    }
    
    private void SceneSwitch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (SceneManager.GetActiveScene().name == "FlightScene") SetBuildScene();
        else SetFlightScene();
    }

    public void SetBuildScene() {
        SceneManager.LoadScene("BuildScene");
        
        inputActions.Spacecraft.Disable();
        inputActions.SpacecraftBuilding.Enable();
    }

    private void SetFlightScene() {
        SceneManager.LoadScene("FlightScene");
        
        inputActions.SpacecraftBuilding.Disable();
        inputActions.Spacecraft.Enable();
        
        OnSetFlightScenePerformedAction?.Invoke(this, EventArgs.Empty);
    }

    public void SetCreditsScene() {
        SceneManager.LoadScene("CreditsScene");

        inputActions.Spacecraft.Disable();
        inputActions.SpacecraftBuilding.Disable();
    }

    public void SetMissionDetailsScene() {
        SceneManager.LoadScene("MissionDetailsScene");

        inputActions.Spacecraft.Disable();
        inputActions.SpacecraftBuilding.Disable();
    }

    public void SetMainMenuScene() {
        SceneManager.LoadScene("MainMenuScene");

        inputActions.Spacecraft.Disable();
        inputActions.SpacecraftBuilding.Disable();
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
        //Properly disable and cleanup input actions
        if (inputActions != null) {
            // Unsubscribe from all events first
            inputActions.Spacecraft.EngineOne.performed -= EngineOne_performed;
            inputActions.Spacecraft.EngineOne.canceled -= EngineOne_canceled;
            inputActions.Spacecraft.EngineTwo.performed -= EngineTwo_performed;
            inputActions.Spacecraft.EngineTwo.canceled -= EngineTwo_canceled;
            inputActions.Spacecraft.EngineThree.performed -= EngineThree_performed;
            inputActions.Spacecraft.EngineThree.canceled -= EngineThree_canceled;
            inputActions.Spacecraft.EngineFour.performed -= EngineFour_performed;
            inputActions.Spacecraft.EngineFour.canceled -= EngineFour_canceled;
            
            inputActions.SpacecraftBuilding.KeyOne.performed -= KeyOne_performed;
            inputActions.SpacecraftBuilding.KeyTwo.performed -= KeyTwo_performed;
            inputActions.SpacecraftBuilding.KeyThree.performed -= KeyThree_performed;
            inputActions.SpacecraftBuilding.KeyFour.performed -= KeyFour_performed;
            inputActions.SpacecraftBuilding.KeyFive.performed -= KeyFive_performed;
            inputActions.SpacecraftBuilding.KeySix.performed -= KeySix_performed;
            inputActions.SpacecraftBuilding.KeySeven.performed -= KeySeven_performed;
            inputActions.SpacecraftBuilding.KeyEight.performed -= KeyEight_performed;
            inputActions.SpacecraftBuilding.LeftMouseClick.performed -= LeftMouseClick_performed;
            
            // Always disable the action maps (safe to call even if already disabled)
            inputActions.Spacecraft.Disable();
            
            // Disable the entire input system and dispose
            inputActions.Disable();
            inputActions.Dispose();
            inputActions = null;
        }
    }
}

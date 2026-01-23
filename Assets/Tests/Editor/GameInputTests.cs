using NUnit.Framework;
using UnityEngine;

public class GameInputTests {
    
    [Test]
    public void EngineActivatedEventArgs_Constructor_SetsActivated() {
        // Create event arguments with true and false
        var argsTrue = new GameInput.EngineEventArgs(true, 1);
        var argsFalse = new GameInput.EngineEventArgs(false, 1);
        
        // Check that the values were set correctly
        Assert.IsTrue(argsTrue.activated, "Args should set activated to true");
        Assert.IsFalse(argsFalse.activated, "Args should set activated to false");
    }
    
    [Test]
    public void GameInput_EventFires_WhenInvoked() {
        // Create a test GameInput object
        GameObject gameInputObj = new GameObject("TestGameInput");
        GameInput gameInput = gameInputObj.AddComponent<GameInput>();
        bool eventFired = false;
        bool eventValue = false;
        
        // Listen for the event - when it fires, set our flags
        gameInput.OnEnginePerformedAction += (sender, args) => {
            eventFired = true;
            eventValue = args.activated;
        };
        
        // Simulate pressing the engine button by calling the internal method
        var performMethod = typeof(GameInput).GetMethod("ActivateEngine_performed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var defaultContext = default(UnityEngine.InputSystem.InputAction.CallbackContext);
        performMethod?.Invoke(gameInput, new object[] { defaultContext });
        
        // Check that the event fired and passed the correct value
        Assert.IsTrue(eventFired, "Event should fire when invoked");
        Assert.IsTrue(eventValue, "Event should pass correct activated value");
        
        // Clean up test objects
        Object.DestroyImmediate(gameInputObj);
    }
    
    [Test]
    public void GameInput_CanceledEventFires_WhenInvoked() {
        // Create a test GameInput object
        GameObject gameInputObj = new GameObject("TestGameInput");
        GameInput gameInput = gameInputObj.AddComponent<GameInput>();
        bool eventFired = false;
        bool eventValue = true;
        
        // Listen for the canceled event - when it fires, set our flags
        gameInput.OnEngineCanceledAction += (sender, args) => {
            eventFired = true;
            eventValue = args.activated;
        };
        
        // Simulate releasing the engine button by calling the internal method
        var cancelMethod = typeof(GameInput).GetMethod("ActivateEngine_canceled", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var defaultContext = default(UnityEngine.InputSystem.InputAction.CallbackContext);
        cancelMethod?.Invoke(gameInput, new object[] { defaultContext });
        
        // Check that the event fired and passed false
        Assert.IsTrue(eventFired, "Canceled event should fire when invoked");
        Assert.IsFalse(eventValue, "Canceled event should pass false as activated value");
        
        // Clean up test objects
        Object.DestroyImmediate(gameInputObj);
    }
}


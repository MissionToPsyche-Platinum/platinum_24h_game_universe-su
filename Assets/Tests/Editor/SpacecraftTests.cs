using NUnit.Framework;
using UnityEngine;

public class SpacecraftTests {
    
    [Test]
    public void Spacecraft_EnablesEngine_WhenInputEventFired() {
        // Create test objects
        GameObject spacecraftObj = new GameObject("TestSpacecraft");
        GameObject gameInputObj = new GameObject("TestGameInput");
        GameObject engineObj = new GameObject("TestEngine");
        
        Spacecraft spacecraft = spacecraftObj.AddComponent<Spacecraft>();
        GameInput gameInput = gameInputObj.AddComponent<GameInput>();
        Engine engine = engineObj.AddComponent<Engine>();
        
        // Connect the spacecraft to gameInput and engine (needed because fields are private)
        var gameInputField = typeof(Spacecraft).GetField("gameInput", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var engineField = typeof(Spacecraft).GetField("engine", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        gameInputField?.SetValue(spacecraft, gameInput);
        engineField?.SetValue(spacecraft, engine);
        
        // Start with engine turned off
        engine.enabled = false;
        
        // Initialize the spacecraft (sets up event listeners)
        var startMethod = typeof(Spacecraft).GetMethod("Start", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        startMethod?.Invoke(spacecraft, null);
        
        // Simulate pressing the engine button by calling the event handler
        var handlerMethod = typeof(Spacecraft).GetMethod("GameInput_OnActivateEngineAction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        handlerMethod?.Invoke(spacecraft, new object[] { gameInput, new GameInput.EngineEventArgs(true, 1) });
        
        // Check that the engine turned on
        Assert.IsTrue(engine.enabled, "Engine should be enabled when input event fires");
        
        // Clean up test objects
        Object.DestroyImmediate(spacecraftObj);
        Object.DestroyImmediate(gameInputObj);
        Object.DestroyImmediate(engineObj);
    }
    
    [Test]
    public void Spacecraft_DisablesEngine_WhenInputCanceled() {
        // Create test objects
        GameObject spacecraftObj = new GameObject("TestSpacecraft");
        GameObject gameInputObj = new GameObject("TestGameInput");
        GameObject engineObj = new GameObject("TestEngine");
        
        Spacecraft spacecraft = spacecraftObj.AddComponent<Spacecraft>();
        GameInput gameInput = gameInputObj.AddComponent<GameInput>();
        Engine engine = engineObj.AddComponent<Engine>();
        
        // Connect the spacecraft to gameInput and engine (needed because fields are private)
        var gameInputField = typeof(Spacecraft).GetField("gameInput", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var engineField = typeof(Spacecraft).GetField("engine", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        gameInputField?.SetValue(spacecraft, gameInput);
        engineField?.SetValue(spacecraft, engine);
        
        // Start with engine turned on
        engine.enabled = true;
        
        // Initialize the spacecraft (sets up event listeners)
        var startMethod = typeof(Spacecraft).GetMethod("Start", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        startMethod?.Invoke(spacecraft, null);
        
        // Simulate releasing the engine button by calling the event handler
        var handlerMethod = typeof(Spacecraft).GetMethod("GameInput_OnActivateEngineAction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        handlerMethod?.Invoke(spacecraft, new object[] { gameInput, new GameInput.EngineEventArgs(false, 1) });
        
        // Check that the engine turned off
        Assert.IsFalse(engine.enabled, "Engine should be disabled when input is canceled");
        
        // Clean up test objects
        Object.DestroyImmediate(spacecraftObj);
        Object.DestroyImmediate(gameInputObj);
        Object.DestroyImmediate(engineObj);
    }
}


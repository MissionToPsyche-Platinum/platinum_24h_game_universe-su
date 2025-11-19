using NUnit.Framework;
using UnityEngine;

public class EngineTests {
    
    [Test]
    public void Engine_StartsDisabled() {
        // Create a test spacecraft with physics
        GameObject spacecraftObj = TestHelpers.CreateSpacecraftWithRigidbody();
        Engine engine = spacecraftObj.AddComponent<Engine>();
        
        // Initialize the engine
        engine.Awake();
        
        // Check that the engine starts turned off
        Assert.IsFalse(engine.enabled, "Engine should start disabled");
        
        // Clean up test objects
        Object.DestroyImmediate(spacecraftObj);
    }
    
    [Test]
    public void Engine_AppliesVelocityWhenEnabled() {
        // Create test objects
        GameObject spacecraftObj = TestHelpers.CreateSpacecraftWithRigidbody();
        Rigidbody2D rb = spacecraftObj.GetComponent<Rigidbody2D>();
        Spacecraft spacecraft = spacecraftObj.AddComponent<Spacecraft>();
        Engine engine = spacecraftObj.AddComponent<Engine>();
        
        // Connect the engine to the spacecraft (needed because the field is private)
        var spacecraftField = typeof(Engine).GetField("spacecraft", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        spacecraftField?.SetValue(engine, spacecraft);
        
        // Set the engine speed to 10 (needed because the field is private)
        var speedField = typeof(Engine).GetField("speed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        speedField?.SetValue(engine, 10);
        
        // Turn on the engine and run its update method
        engine.enabled = true;
        var updateMethod = typeof(Engine).GetMethod("Update", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        updateMethod?.Invoke(engine, null);
        
        // Check that the spacecraft moves upward at speed 10
        Assert.AreEqual(Vector2.up * 10, rb.linearVelocity, "Engine should apply velocity upward");
        
        // Clean up test objects
        Object.DestroyImmediate(spacecraftObj);
    }
}


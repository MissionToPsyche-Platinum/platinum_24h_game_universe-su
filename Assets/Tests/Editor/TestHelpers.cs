using UnityEngine;

public static class TestHelpers {
    // Creates a test spacecraft GameObject with physics
    public static GameObject CreateSpacecraftWithRigidbody() {
        GameObject spacecraft = new GameObject("TestSpacecraft");
        spacecraft.AddComponent<Rigidbody2D>();
        return spacecraft;
    }
}


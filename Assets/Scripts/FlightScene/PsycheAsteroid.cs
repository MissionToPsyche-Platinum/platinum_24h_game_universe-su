using UnityEngine;

public class PsycheAsteroid : MonoBehaviour {
    public static PsycheAsteroid Instance;

    private void Awake() => Instance = this;
}

using UnityEngine;

public class ElectricPropulsion : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet.
    void Update()
    {

    }
}


using UnityEngine;

//Class defines the behavior of the electric propulsion part. 

public class ElectricPropulsion : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet.
    void Update()
    {

    }
}


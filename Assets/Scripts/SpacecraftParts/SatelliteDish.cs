using UnityEngine;

//Class defines the behavior of the satellite dish part. 

public class SatelliteDish : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet � just a marker component.
    void Update()
    {

    }
}

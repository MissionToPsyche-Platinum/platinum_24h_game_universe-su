using UnityEngine;

public class SatelliteDish : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet – just a marker component.
    void Update()
    {

    }
}

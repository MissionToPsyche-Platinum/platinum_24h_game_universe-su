using UnityEngine;

////Class defines the behavior of the solar panel part. 

public class SolarPanel : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet � just a marker component.
    void Update()
    {

    }
}

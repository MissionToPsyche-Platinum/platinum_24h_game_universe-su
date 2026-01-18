using UnityEngine;

//Class defines the behavior of the magnetometer part. 

public class Magnetometer : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet � just a marker component.
    void Update()
    {

    }
}




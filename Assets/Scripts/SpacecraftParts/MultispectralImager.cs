using UnityEngine;

//Class defines the behavior of the multispectral imager part. 

public class MultispectralImager : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet � just a marker component.
    void Update()
    {

    }
}

using UnityEngine;

public class MultispectralImager : MonoBehaviour
{
    [SerializeField] private Spacecraft spacecraft;

    public void Awake() => enabled = false;

    // No behavior yet – just a marker component.
    void Update()
    {

    }
}

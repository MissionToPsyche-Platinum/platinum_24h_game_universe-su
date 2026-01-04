using UnityEngine;

public class GammaRay : MonoBehaviour
{

    [SerializeField] private Spacecraft spacecraft;

    // Start with the component turned off
    public void Awake() => enabled = false;

    // Update is called once per frame
    void Update()
    {
        
    }
}

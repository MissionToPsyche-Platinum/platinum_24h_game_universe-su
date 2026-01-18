using UnityEngine;
//Class defines the behavior of the gamma ray part. 

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

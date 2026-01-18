using UnityEngine;

//Class defines the behavior of the engine part. 
public class Engine : MonoBehaviour {

    [SerializeField] private Spacecraft spacecraft;
    [SerializeField] private int speed;

    public void Awake() => enabled = false;
    
    private void Update() => spacecraft.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
}

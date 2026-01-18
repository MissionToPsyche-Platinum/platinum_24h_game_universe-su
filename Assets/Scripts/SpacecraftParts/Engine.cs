using UnityEngine;

public class Engine : MonoBehaviour {
    [SerializeField] private int speed;
    private Rigidbody2D engineRigidbody2D;

    public void Awake()
    {
        enabled = false;
        engineRigidbody2D =  GetComponentInParent<Rigidbody2D>();
    }

    public void Start()
    {
    }
    
    private void FixedUpdate()
    {
        engineRigidbody2D.AddForce(speed * transform.up * Time.deltaTime);
    } 
}

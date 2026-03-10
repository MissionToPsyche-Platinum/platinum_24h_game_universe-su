using System;
using UnityEngine;
using Random = System.Random;

public class AsteroidFlight : MonoBehaviour {
    
    [SerializeField] public int asteroidSize;
    public Vector3 direction;
    private float speed;
    private int _spawnSide;
    public int spawnSide {
        private get { return _spawnSide; }
        set {
            _spawnSide = value;
            
            float rand(float min, float max) => UnityEngine.Random.Range(min, max);
            switch (value) {
                case 0: //Spawn above camera
                    direction = new Vector3(rand(-1, 1), rand(-1, 0)).normalized;
                    break;
                case 1: //below
                    direction = new Vector3(rand(-1, 1), rand(0, 1)).normalized;
                    break;
                case 2: //left
                    direction = new Vector3(rand(0, 1), rand(-1, 1)).normalized;
                    break;
                default: //right
                    direction = new Vector3(rand(-1, 0), rand(-1, 1)).normalized;
                    break;
            }
        }
    }

    private void Awake() {
        speed = UnityEngine.Random.Range(.5f, 8f);
    }

    private void Start() {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        transform.position += direction * 2; // offset to prevent asteroids from clipping into eachother during splitting
    }
    
}

using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Static class used to keep track of asteroids, spawning them in and destroying them and stuff
public class AsteroidController : MonoBehaviour {
    public static AsteroidController Instance { get; private set; }
    
    [SerializeField] private int maxAsteroids;
    [SerializeField] private int minAsteroidSpeed;
    [SerializeField] private int maxAsteroidSpeed;
    [SerializeField] private GameObject[] allAsteroidPrefabs;
    [SerializeField] private Camera camera;
    [SerializeField] private float damageCooldown = 1f;
    
    private int currentAsteroidCount;
    private float timeUntilNextAsteroidSpawn = 5f;

    private Dictionary<GameObject, float> outOfCameraTimes = new();
    private void Awake() => Instance = this;

    private void Start() {
        CameraFollow.OnAsteroidPassing += CameraFollow_OnAsteroidPassingAction;
    }
    
    private void Update() {
        foreach (GameObject asteroid in outOfCameraTimes.Keys.ToList()) {
            outOfCameraTimes[asteroid] += Time.deltaTime;
            if(outOfCameraTimes[asteroid] >= 3f) DestroyAsteroid(asteroid);
        }
        
        if (currentAsteroidCount == maxAsteroids) return;

        timeUntilNextAsteroidSpawn -= Time.deltaTime;
        if (timeUntilNextAsteroidSpawn <= 0) SpawnAsteroid();
    }

    private void SpawnAsteroid() {
        GameObject nextAsteroid = allAsteroidPrefabs[UnityEngine.Random.Range(0, 18)];
        if (outOfCameraTimes.ContainsKey(nextAsteroid)) return;
        timeUntilNextAsteroidSpawn = UnityEngine.Random.Range(2, 7); //Adjust asteroid spawn frequency here
        currentAsteroidCount++;

        int spawnSide;
        Vector3 spawnPosition = GetSpawnPosition(out spawnSide);
        if (!Physics.CheckSphere(spawnPosition, 2f, LayerMask.GetMask("Default"))) {
            GameObject asteroid = Instantiate(nextAsteroid, spawnPosition, Quaternion.identity);
            asteroid.GetComponent<AsteroidFlight>().spawnSide = spawnSide;
            outOfCameraTimes.Add(asteroid, 0f);
        } else {
            Debug.Log("Dont Spawn");
        }
    }

    private Vector3 GetSpawnPosition(out int spawnSide) {
        spawnSide = UnityEngine.Random.Range(0, 4);
        Vector3 offset = new Vector3();
        
        switch (spawnSide) {
            case 0: //Spawn above camera
                offset = new Vector3(UnityEngine.Random.Range(-11f, 11f), 8, camera.transform.position.z * -1);
                break;
            case 1: //below
                offset = new Vector3(UnityEngine.Random.Range(-11f, 11f), -8, camera.transform.position.z * -1);
                break;
            case 2: //left
                offset = new Vector3(-11, UnityEngine.Random.Range(-8f, 8f), camera.transform.position.z * -1);
                break;
            case 4: //right
                offset = new Vector3(11, UnityEngine.Random.Range(-8f, 8f), camera.transform.position.z * -1);
                break;
        }
        
        return camera.transform.position + offset;
    }

    private void DestroyAsteroid(GameObject asteroid) {
        outOfCameraTimes.Remove(asteroid);
        Destroy(asteroid);
        currentAsteroidCount--;
    }

    private void CameraFollow_OnAsteroidPassingAction(object sender, CameraFollow.AsteroidPassingEventArgs e) {
        if(e.isEntering) outOfCameraTimes.Remove(e.asteroid);
        else outOfCameraTimes.Add(e.asteroid, 0f);
    }

    public float GetDamageCoolDown() => damageCooldown;
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Class that allows for retrieving ship parts for cloning and getting data about existing parts.
/// </summary>
public class SpacecraftPartDatabase : MonoBehaviour {
    [SerializeField] private PartScriptableObject[] allParts;

    public static SpacecraftPartDatabase Instance;
    private int engineCount = 0;

    public void Awake() {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    
    public int GetPartID(GameObject part) {
        foreach (PartScriptableObject partSO in allParts) {
            if (partSO.part == part) return partSO.partID;
        }

        return -1;
    }
    
    public GameObject GetPartGameObject(int id) {
        foreach (PartScriptableObject partSO in allParts) {
            if (partSO.partID == id) return partSO.part;
        }

        return null;
    }

    public GameObject GetPartGameObject(string objectOrObjectCloneName) {
        if (objectOrObjectCloneName.Contains("(Clone)")) {
            string nameWithoutClone = "";

            foreach (char c in objectOrObjectCloneName) {
                if (c == '(') break;

                nameWithoutClone += c;
            }

            objectOrObjectCloneName = nameWithoutClone;
        }
        
        foreach (PartScriptableObject partSO in allParts) {
            if (partSO.part.name == objectOrObjectCloneName) return partSO.part;
        }

        return null;
    }

    public List<string> GetSnapableDirections(GameObject part) => GetSnapableDirections(GetPartID(part));

    public List<string> GetSnapableDirections(int id) {
        foreach (PartScriptableObject partSO in allParts) {
            if (partSO.partID == id) {
                return partSO.connectingDirections.ToList();
            }
        }

        return null;
    }

    public int CreateEngineID() {
        engineCount++;
        return engineCount;
    }
}

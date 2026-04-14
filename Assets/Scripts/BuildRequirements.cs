using System.Collections.Generic;
using UnityEngine;

public class BuildRequirements : MonoBehaviour {
    public static BuildRequirements Instance { get; private set; }
    
    [SerializeField] private Transform shipRoot;
    [SerializeField] private bool bypassBuildRequirements;

    private readonly SpacePartType[] requiredParts = {
        SpacePartType.GammaRay,
        SpacePartType.Magnetometer,
        SpacePartType.MultispectralImager,
        SpacePartType.NeutronSpectrometer,
        SpacePartType.SatelliteDish,
        SpacePartType.SolarPanel
    };
    
    private void Awake() => Instance = this;

    private void Start() => shipRoot = GameObject.Find("SpacecraftStart").transform;

    public bool IsReadyForFlight(out string message) {
        if (bypassBuildRequirements) {
            message = "Ready for flight!";
            return true;
        }
        
        if (shipRoot == null) {
            message = "Ship root not set.";
            return false;
        }

        PartData[] installed = shipRoot.GetComponentsInChildren<PartData>(true);

        Debug.Log("Installed PartData count = " + installed.Length);

        foreach (PartData p in installed) {
            Debug.Log("Found PartData: " + p.gameObject.name + " | partType=" + p.partType);
        }

        HashSet<SpacePartType> found = new HashSet<SpacePartType>();
        foreach (PartData p in installed) {
            found.Add(p.partType);
        }

        List<string> missing = new List<string>();
        foreach (SpacePartType req in requiredParts) {
            if (!found.Contains(req)) {
                missing.Add(req.ToString());
            }
        }

        if (missing.Count > 0) {
            message = "Missing parts: " + string.Join(", ", missing);
            return false;
        }

        message = "Ready for flight!";
        return true;
    }
}
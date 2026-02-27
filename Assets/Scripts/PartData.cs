using UnityEngine;

public enum SpacePartType
{
    None,
    GammaRay,
    Magnetometer,
    MultispectralImager,
    NeutronSpectrometer,
    SatelliteDish,
    SolarPanel
}

public class PartData : MonoBehaviour
{
    public SpacePartType partType = SpacePartType.None;
}
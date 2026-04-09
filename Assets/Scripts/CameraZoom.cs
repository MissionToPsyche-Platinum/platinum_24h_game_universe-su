using UnityEngine;

/// <summary>
/// Attach to your FlightScene Camera.
/// Zooms in/out using the mouse scroll wheel.
/// </summary>
public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 20f;   // most zoomed in
    [SerializeField] private float maxZoom = 150f;  // most zoomed out

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return; // respect pause

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI indicator that sits on the screen edge and points toward a world-space target
/// when it is off-screen. Hides automatically when the target is visible.
/// </summary>
public class OffScreenIndicatorUI : MonoBehaviour {

    [Tooltip("World-space target this indicator points to")]
    [SerializeField] private Transform target;

    [Tooltip("Padding in pixels from screen edge")]
    [SerializeField] private float edgePadding = 50f;

    private RectTransform indicator;
    private Image indicatorImage;
    private Camera mainCamera;
    private RectTransform canvasRect;

    private void Awake() {
        indicator = GetComponent<RectTransform>();
        indicatorImage = GetComponent<Image>();
        mainCamera = Camera.main;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }

    private void Update() {
        if (target == null || mainCamera == null) {
            indicatorImage.enabled = false;
            return;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        bool isOnScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                          screenPos.y > 0 && screenPos.y < Screen.height &&
                          screenPos.z > 0;

        indicatorImage.enabled = !isOnScreen;

        if (isOnScreen) return;

        // Get screen center and direction to target in screen space
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;

        // Clamp to screen edges with padding
        float halfW = Screen.width / 2f - edgePadding;
        float halfH = Screen.height / 2f - edgePadding;

        // Find where the direction ray hits the screen edge
        float tX = dir.x != 0 ? Mathf.Abs(halfW / dir.x) : float.MaxValue;
        float tY = dir.y != 0 ? Mathf.Abs(halfH / dir.y) : float.MaxValue;
        float t = Mathf.Min(tX, tY);

        Vector2 edgePos = screenCenter + dir * t;

        // Convert screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, edgePos, null, out Vector2 localPoint);
        indicator.localPosition = localPoint;

        // Rotate so the indicator points toward the target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        indicator.localRotation = Quaternion.Euler(0, 0, angle);
    }
}

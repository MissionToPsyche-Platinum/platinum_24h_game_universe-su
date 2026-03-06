using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles dragging a part from the side panel onto the build grid.
/// Creates a ghost preview that snaps to grid cells and shows placement validity.
/// </summary>
public class PanelPartDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    [SerializeField] private PartScriptableObject partData;
    private BuildFactsPopup buildFactsPopup;
    private GameObject ghostPreview;
    private SpriteRenderer ghostSprite;
    private Color baseColor = Color.white;

    private static readonly Color colorValid   = new Color(0.3f, 1f, 0.3f, 0.6f);
    private static readonly Color colorInvalid = new Color(1f, 0.3f, 0.3f, 0.6f);

    public void Initialize(PartScriptableObject part) {
        partData = part;

        // Cache the part's visual color for the ghost preview
        SpriteRenderer sr = part.part.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
    }

    private void Awake() {
        SpriteRenderer sr = partData.part.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
    }

    private void Start()
    {
        buildFactsPopup = GameObject.Find("BuildFactsPopup").GetComponent<BuildFactsPopup>();
    }
    public void OnBeginDrag(PointerEventData eventData) {
        if (partData == null || partData.part == null) return;
        if (!Spacecraft.IsBuildMode) return;

        SpriteRenderer partVisual = partData.part.GetComponentInChildren<SpriteRenderer>();
        if (partVisual == null || partVisual.sprite == null) return;

        ghostPreview = new GameObject("GhostPreview");
        ghostSprite = ghostPreview.AddComponent<SpriteRenderer>();
        ghostSprite.sprite = partVisual.sprite;
        ghostSprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
        ghostSprite.sortingLayerName = "MidDrag";
        ghostSprite.sortingOrder = 100;

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        if (ghostPreview == null) return;
        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (ghostPreview == null) return;

        ShipBuildingGrid grid = ShipBuildingGrid.Instance;
        if (grid != null) {
            Vector3 worldPos = ScreenToWorld(eventData.position);
            Vector3? snapPos = grid.PostionToGridPosition(worldPos);

            if (snapPos != null) {
                (int, int) coords = grid.UnityPositionToGridCoordinates((Vector3)snapPos);
                if (grid.CanPlacePart(partData.part, coords)) {
                    grid.PlacePartAtCoordinates(partData.part, coords);
                    buildFactsPopup.Popup(partData.name);
                }
            }
        }

        Destroy(ghostPreview);
        ghostPreview = null;
        ghostSprite = null;
    }

    private void UpdateGhostPosition(PointerEventData eventData) {
        if (ghostPreview == null) return;

        ShipBuildingGrid grid = ShipBuildingGrid.Instance;
        if (grid == null) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        Vector3? snapPos = grid.PostionToGridPosition(worldPos);

        if (snapPos != null) {
            ghostPreview.transform.position = (Vector3)snapPos;

            (int, int) coords = grid.UnityPositionToGridCoordinates((Vector3)snapPos);
            bool valid = grid.CanPlacePart(partData.part, coords);
            ghostSprite.color = valid ? colorValid : colorInvalid;
        } else {
            ghostPreview.transform.position = worldPos;
            ghostSprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPos) {
        Camera cam = Camera.main;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(
            screenPos.x, screenPos.y, Mathf.Abs(cam.transform.position.z)));
        worldPos.z = 0f;
        return worldPos;
    }
}

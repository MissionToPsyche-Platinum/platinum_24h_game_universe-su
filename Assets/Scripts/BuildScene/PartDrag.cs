using TMPro;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

//Script that allows ship parts to be dragged around the grid. Also connects parts together with joints.
public class PartDrag : MonoBehaviour {
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject objectVisual;
    
    [SerializeField] private GameObject highlight;
    private SpriteRenderer highlightSprite;

    [SerializeField] private Sprite colorblindValid;
    [SerializeField] private Sprite colorblindInvalid;
    private static readonly Color colorValid   = new Color(0.3f, 1f, 0.3f, 0.6f);
    private static readonly Color colorInvalid = new Color(1f, 0.3f, 0.3f, 0.6f);

    private bool colorblindMode;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D partCollider;
    private Quaternion lockedRotation;
    private ShipBuildingGrid shipGrid;
    private SpacecraftPartDatabase partDB;
    private SpriteRenderer objectSprite;
    private Color baseColor;
    private Sprite baseHighlightSprite;
    private string midDragLayer = "MidDrag";
    private string defaultLayer = "Default";
    private string spacecraftLayer = "SpaceCraft";

    private void Awake() {
        partCollider = GetComponent<Collider2D>();
        objectSprite = objectVisual.GetComponent<SpriteRenderer>();
        
        lockedRotation = transform.rotation;
        
        shipGrid = ShipBuildingGrid.Instance;
        highlight = GameObject.Find("Highlight");
        highlightSprite = highlight.GetComponent<SpriteRenderer>();
        baseHighlightSprite = highlightSprite.sprite;
        partDB = SpacecraftPartDatabase.Instance;

        colorblindMode = Settings.instance.colorblindMode;
    }

    private void OnMouseDown() {
        if (!Spacecraft.IsBuildMode) return;

        if (highlight == null)
        {
            highlight = GameObject.Find("Highlight");
            highlightSprite = highlight.GetComponent<SpriteRenderer>();
            colorblindMode = Settings.instance.colorblindMode;
        }

        originalPosition = transform.position;
        baseColor = objectSprite.color;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)
        );

        Debug.Log($"Part is connected: {shipGrid.PartIsConnected(shipGrid.UnityPositionToGridCoordinates(transform.position))}");

        // Clear BOTH grid + dictionary at the original cell
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, -1);
        shipGrid.RemovePlacedPartAtWorldPosition(originalPosition);

        shipGrid.SetSelectedPart(gameObject);

        SetSortingLayer(midDragLayer);
        SetLayer(midDragLayer);
        
        // Enable physics temporarily for dragging
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }
    }

    void OnMouseDrag() {
        if (!Spacecraft.IsBuildMode) return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.rotation = lockedRotation;

        // Snap to grid and show valid/invalid placement color feedback
        if (shipGrid == null) shipGrid = ShipBuildingGrid.Instance;
        if (shipGrid != null) {
            Vector3? snapPos = shipGrid.PostionToGridPosition(curPosition);
            if (snapPos != null) {
                transform.position = (Vector3)snapPos;
                (int, int) coords = shipGrid.UnityPositionToGridCoordinates((Vector3)snapPos);
                GameObject part = partDB.GetPartGameObject(selectedObject.name);
                bool valid = shipGrid.CanPlacePart(part, coords) || CanSwapPart(part, originalPosition);
                objectSprite.color = valid ? colorValid : colorInvalid;
                highlight.transform.position = transform.position;
                highlightSprite.color = colorblindMode ? Color.white : ShipBuildingGrid.colorHighlightInvisible;
                if (colorblindMode) highlightSprite.sprite = valid ? colorblindValid : colorblindInvalid;
            } else {
                transform.position = curPosition;
                highlight.transform.position = curPosition;
                highlightSprite.color = ShipBuildingGrid.colorHighlightInvisible;
                objectSprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
            }
        } else {
            transform.position = curPosition;
            highlight.transform.position = curPosition;
            highlightSprite.color = ShipBuildingGrid.colorHighlightInvisible;
        }
    }

    void OnMouseUp() {
        if (!Spacecraft.IsBuildMode) return;
        if (shipGrid == null || partCollider == null) return;

        objectSprite.color = baseColor;

        transform.rotation = lockedRotation;

        GameObject part = gameObject;

        Vector3? nullableGridSnapPosition = shipGrid.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) {
            // Put it back and re-register it
            PlacePart(gameObject, originalPosition);
            return;
        }

        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;

        if (shipGrid.GetGridCellValue(shipGrid.UnityPositionToGridCoordinates(gridSnapPosition)) == -1) {
            if (TryPlacePart(part, gridSnapPosition)) return;
        } else {
            Collider2D partToBeSwapped = Physics2D.OverlapPoint(gridSnapPosition, LayerMask.GetMask(spacecraftLayer));
            if (partToBeSwapped != null && TrySwapPart(part, originalPosition, partToBeSwapped.gameObject, gridSnapPosition)) return;
        }
        
        PlacePart(gameObject, originalPosition);
        shipGrid.HandleLeftClick();
        highlightSprite.color = ShipBuildingGrid.colorHighlight;
    }

    private bool TryPlacePart(GameObject part, Vector3 worldPosition) {
        GameObject partPrefab = partDB.GetPartGameObject(selectedObject.name);

        if (!shipGrid.CanPlacePart(part, shipGrid.UnityPositionToGridCoordinates(worldPosition))) return false;
        
        PlacePart(part, worldPosition);
        return true;
    }

    private void PlacePart(GameObject part, Vector3 worldPosition) {
        part.transform.position = worldPosition;

        // Update BOTH grid + dictionary at the new cell
        shipGrid.SetGridCellValueByUnityPosition(part.transform.position, partDB.GetPartID(part));
        shipGrid.SetPlacedPartAtWorldPosition(part.transform.position, part.gameObject);

        SetSortingLayer(defaultLayer);
        SetLayer(spacecraftLayer);

        // Reconnect joint and disable physics
        ReconnectPart();
    }

    private bool CanSwapPart(GameObject draggedPart, Vector3 draggedOGPosition, GameObject otherPart, Vector3 otherOGPosition) {
        if (partDB.GetPartID(otherPart) == 0) return false;
        
        int otherID = partDB.GetPartID(otherPart);
        int draggedID = partDB.GetPartID(draggedPart);
        
        shipGrid.SetGridCellValueByUnityPosition(otherOGPosition, -1);
        shipGrid.SetGridCellValueByUnityPosition(draggedOGPosition, otherID);
        bool canPlaceDraggedPart = shipGrid.CanPlacePart(draggedPart, shipGrid.UnityPositionToGridCoordinates(otherOGPosition));
        shipGrid.SetGridCellValueByUnityPosition(draggedOGPosition, -1);
        
        shipGrid.SetGridCellValueByUnityPosition(otherOGPosition, draggedID);
        bool canPlaceOtherPart = shipGrid.CanPlacePart(otherPart, shipGrid.UnityPositionToGridCoordinates(draggedOGPosition));
        shipGrid.SetGridCellValueByUnityPosition(otherOGPosition, -1);
        
        shipGrid.SetGridCellValueByUnityPosition(otherOGPosition, otherID);
        
        if (canPlaceDraggedPart && canPlaceOtherPart) return true;
        
        return false;
    }

    private bool CanSwapPart(GameObject draggedPart, Vector3 draggedOGPosition) {
        Vector3? nullableGridSnapPosition = shipGrid.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) return false;
        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;
        
        Collider2D partToBeSwapped = Physics2D.OverlapPoint(gridSnapPosition, LayerMask.GetMask(spacecraftLayer));
        if (partToBeSwapped == null) return false;

        return CanSwapPart(draggedPart, draggedOGPosition, partToBeSwapped.gameObject, gridSnapPosition);
    }

    private bool TrySwapPart(GameObject draggedPart, Vector3 draggedOGPosition, GameObject otherPart, Vector3 otherOGPosition) {
        if (!CanSwapPart(draggedPart, draggedOGPosition, otherPart, otherOGPosition)) return false;
        
        PlacePart(draggedPart, otherOGPosition);
        PlacePart(otherPart, draggedOGPosition);
        
        return true;
    }

    private void ReconnectPart() {
        if (!Spacecraft.IsBuildMode) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true; 
        }
    }

    private void SetSortingLayer(string layer) {
        objectSprite.sortingLayerName = layer;

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas == null) return;

        canvas.sortingLayerName = layer;
    }

    private void SetLayer(string layer) => gameObject.layer = LayerMask.NameToLayer(layer);
    
    private void Update() {
        if (transform.rotation != lockedRotation) transform.rotation = lockedRotation;
    }
}
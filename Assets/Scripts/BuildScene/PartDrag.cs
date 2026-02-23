using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

//Script that allows ship parts to be dragged around the grid. Also connects parts together with joints.
public class PartDrag : MonoBehaviour {
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject objectVisual;
    
    private static readonly Color colorValid   = new Color(0.3f, 1f, 0.3f, 0.6f);
    private static readonly Color colorInvalid = new Color(1f, 0.3f, 0.3f, 0.6f);

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D partCollider;
    private Quaternion lockedRotation;
    private ShipBuildingGrid shipGrid;
    private SpacecraftPartDatabase partDB;
    private SpriteRenderer objectSprite;
    private Color baseColor;
    private string midDragLayer = "MidDrag";
    private string defaultLayer = "Default";

    private void Awake() {
        partCollider = GetComponent<Collider2D>();
        objectSprite = objectVisual.GetComponent<SpriteRenderer>();
        
        lockedRotation = transform.rotation;
        
        shipGrid = ShipBuildingGrid.Instance;
        partDB = SpacecraftPartDatabase.Instance;
    }

    void OnMouseDown() {
        if (!Spacecraft.IsBuildMode) return;
        
        originalPosition = transform.position;
        baseColor = objectSprite.color;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, -1);
        shipGrid.SetSelectedPart(gameObject);

        SetSortingLayer(midDragLayer);
        SetLayer(midDragLayer);

        // Temporarily disconnect from joints while dragging
        FixedJoint2D joint = GetComponent<FixedJoint2D>();
        if (joint != null) joint.enabled = false;
        
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
        if (shipGrid != null) {
            Vector3? snapPos = shipGrid.PostionToGridPosition(curPosition);
            if (snapPos != null) {
                transform.position = (Vector3)snapPos;
                (int, int) coords = shipGrid.UnityPositionToGridCoordinates((Vector3)snapPos);
                GameObject part = partDB.GetPartGameObject(selectedObject.name);
                bool valid = shipGrid.CanPlacePart(part, coords) || CanSwapPart(part, originalPosition);
                objectSprite.color = valid ? colorValid : colorInvalid;
            } else {
                transform.position = curPosition;
                objectSprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
            }
        } else {
            transform.position = curPosition;
        }
    }

    void OnMouseUp() {
        if (!Spacecraft.IsBuildMode) return;
        if (shipGrid == null || partCollider == null) return;

        objectSprite.color = baseColor;

        transform.rotation = lockedRotation;

        GameObject part = partDB.GetPartGameObject(selectedObject.name);
        
        Vector3? nullableGridSnapPosition = shipGrid.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) return;
        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;

        if (shipGrid.GetGridCellValue(shipGrid.UnityPositionToGridCoordinates(gridSnapPosition)) == -1) {
            if (TryPlacePart(part, gridSnapPosition)) return;
        } else {
            Collider2D partToBeSwapped = Physics2D.OverlapPoint(gridSnapPosition, LayerMask.GetMask(defaultLayer));
            if (partToBeSwapped != null && TrySwapPart(part, originalPosition, partToBeSwapped.gameObject, gridSnapPosition)) return;
        }
        
        PlacePart(gameObject, originalPosition);
    }

    private bool TryPlacePart(GameObject part, Vector3 worldPosition) {
        if (!shipGrid.CanPlacePart(part, shipGrid.UnityPositionToGridCoordinates(worldPosition))) return false;
        
        PlacePart(part, worldPosition);
        return true;
    }

    private void PlacePart(GameObject part, Vector3 worldPosition) {
        part.transform.position = worldPosition;
        shipGrid.SetGridCellValueByUnityPosition(part.transform.position, partDB.GetPartID(part));
        
        SetSortingLayer(defaultLayer);
        SetLayer(defaultLayer);
        
        // Reconnect joint and disable physics
        ReconnectPart();
    }

    private bool CanSwapPart(GameObject draggedPart, Vector3 draggedOGPosition, GameObject otherPart, Vector3 otherOGPosition) {
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
        
        Collider2D partToBeSwapped = Physics2D.OverlapPoint(gridSnapPosition, LayerMask.GetMask(defaultLayer));
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

        FixedJoint2D joint = GetComponent<FixedJoint2D>();
        if (joint != null) joint.enabled = true;

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
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

//Script that allows ship parts to be dragged around the grid. Also connects parts together with joints.
public class PartDrag : MonoBehaviour {
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject objectVisual;
    
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D partCollider;
    private Quaternion lockedRotation;
    private ShipBuildingGrid shipGrid;
    private SpacecraftPartDatabase partDB;
    private SpriteRenderer objectSprite;

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
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, -1);
        shipGrid.SetSelectedPart(gameObject);

        SetSortingLayer("MidDrag");

        // Temporarily disconnect from joints while dragging
        FixedJoint2D joint = GetComponent<FixedJoint2D>();
        if (joint != null) {
            joint.enabled = false;
        }
        
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
        transform.position = curPosition;
        transform.rotation = lockedRotation;
    }

    void OnMouseUp() {
        if (!Spacecraft.IsBuildMode) return;
        if (shipGrid == null || partCollider == null) return;
        
        SetSortingLayer("Default");

        transform.rotation = lockedRotation;

        GameObject part = partDB.GetPartGameObject(selectedObject.name);
        
        Vector3? nullableGridSnapPosition = shipGrid.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) return;
        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;

        if (shipGrid.GetGridCellValue(shipGrid.UnityPositionToGridCoordinates(gridSnapPosition)) == -1) {
            if (TryPlacePart(part, gridSnapPosition)) return;
        } else {
            //gameObject.
            //GameObject partToBeSwapped
            //if (TrySwapPart(part, gridSnapPosition)) return;
        }
        
        transform.position = originalPosition;
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, partDB.GetPartID(part));

        // Reconnect joint and disable physics before returning
        ReconnectPart();
    }

    private bool TryPlacePart(GameObject part, Vector3 worldPosition) {
        if (!shipGrid.CanPlacePart(part, shipGrid.UnityPositionToGridCoordinates(worldPosition))) return false;
        
        transform.position = worldPosition;
        shipGrid.SetGridCellValueByUnityPosition(transform.position, partDB.GetPartID(part));
        
        // Reconnect joint and disable physics
        ReconnectPart();
        return true;
    }

    private bool TrySwapPart(GameObject part1, Vector3 worldPosition1, GameObject part2, Vector3 worldPosition2) {
        return false;
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
    
    private void Update() {
        if (transform.rotation != lockedRotation) transform.rotation = lockedRotation;
    }
}
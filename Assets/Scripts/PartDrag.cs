using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class PartDrag : MonoBehaviour {
    [SerializeField] private GameObject selectedObject;
    
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D partCollider;
    private Quaternion lockedRotation;
    private ShipBuildingGrid shipGrid;

    private void Awake() {
        partCollider = GetComponent<Collider2D>();
        shipGrid = ShipBuildingGrid.instance;
        lockedRotation = transform.rotation;
    }

    void OnMouseDown() {
        originalPosition = transform.position;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, -1);
    }

    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
        transform.rotation = lockedRotation;
    }

    void OnMouseUp() {
        transform.rotation = lockedRotation;
        
        if (shipGrid == null || partCollider == null) return;
        
        Vector3? nullableGridSnapPosition = ShipBuildingGrid.instance.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) return;
        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;

        GameObject part = SpacecraftPartDatabase.Instance.GetPartGameObjectByName(selectedObject.name);
        int partID = SpacecraftPartDatabase.Instance.GetPartID(part);

        if (!shipGrid.CanPlacePart(part, shipGrid.UnityPositionToGridCoordinates(gridSnapPosition))) {
            transform.position = originalPosition;
            shipGrid.SetGridCellValueByUnityPosition(originalPosition, partID);
            return;
        }
        
        transform.position = gridSnapPosition;
        shipGrid.SetGridCellValueByUnityPosition(transform.position, partID);
    }
    
    private void Update() {
        if (transform.rotation != lockedRotation) transform.rotation = lockedRotation;
    }
}
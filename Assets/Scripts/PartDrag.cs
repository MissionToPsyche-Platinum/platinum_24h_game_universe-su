using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

//Script that allows ship parts to be dragged around the grid. Also connects parts together with joints.
public class PartDrag : MonoBehaviour {
    [SerializeField] private GameObject selectedObject;
    
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Collider2D partCollider;
    private Quaternion lockedRotation;
    private ShipBuildingGrid shipGrid;
    private SpacecraftPartDatabase partDB;

    private void Awake() {
        partCollider = GetComponent<Collider2D>();
        lockedRotation = transform.rotation;
        shipGrid = ShipBuildingGrid.instance;
        partDB = SpacecraftPartDatabase.Instance;
    }

    void OnMouseDown() {
        if (!Spacecraft.IsBuildMode) return;
        
        originalPosition = transform.position;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        
        shipGrid.SetGridCellValueByUnityPosition(originalPosition, -1);

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

        transform.rotation = lockedRotation;
        
        if (shipGrid == null || partCollider == null) return;
        
        Vector3? nullableGridSnapPosition = shipGrid.PostionToGridPosition(transform.position);
        if (nullableGridSnapPosition == null) return;
        Vector3 gridSnapPosition = (Vector3)nullableGridSnapPosition;

        GameObject part = partDB.GetPartGameObject(selectedObject.name);
        int partID = partDB.GetPartID(part);

        if (!shipGrid.CanPlacePart(part, shipGrid.UnityPositionToGridCoordinates(gridSnapPosition))) {
            transform.position = originalPosition;
            shipGrid.SetGridCellValueByUnityPosition(originalPosition, partID);
            
            // Reconnect joint and disable physics before returning
            ReconnectPart();
            return;
        }
        
        transform.position = gridSnapPosition;
        shipGrid.SetGridCellValueByUnityPosition(transform.position, partID);
        
        // Reconnect joint and disable physics
        ReconnectPart();
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
    
    private void Update() {
        if (transform.rotation != lockedRotation) transform.rotation = lockedRotation;
    }
}
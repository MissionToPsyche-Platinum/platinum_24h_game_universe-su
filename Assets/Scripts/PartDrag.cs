using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class PartDrag : MonoBehaviour
{

private Vector3 screenPoint;
private Vector3 offset;
private BoxCollider2D partCollider;
private Quaternion lockedRotation;

    private void Awake() {
        partCollider = GetComponent<BoxCollider2D>();
        lockedRotation = transform.rotation;
    }

    void OnMouseDown() {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
        transform.rotation = lockedRotation;
    }

    void OnMouseUp()
    {
        transform.rotation = lockedRotation;
        
        if (ShipBuildingGrid.instance == null || partCollider == null) return;
        
        Vector3? shipSnapPosition = ShipBuildingGrid.instance.SnapToShipSurface(transform.position, partCollider);
        
        if (shipSnapPosition != null) {
            transform.position = (Vector3)shipSnapPosition;
            ShipBuildingGrid.instance.AttachPartToShip(gameObject);
            return;
        }
        
        Vector3? gridSnapPosition = ShipBuildingGrid.instance.PostionToGridPosition(transform.position);
        if (gridSnapPosition != null) {
            transform.position = (Vector3)gridSnapPosition;
        }
    }
    
    private void Update() {
        if (transform.rotation != lockedRotation) {
            transform.rotation = lockedRotation;
        }
    }
}
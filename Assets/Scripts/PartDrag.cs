using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class PartDrag : MonoBehaviour
{

private Vector3 screenPoint;
private Vector3 offset;

    /// <summary>
    /// When clicked, the object determines its relative position to the mouse for later use in onmousedrag
    /// </summary>
    void OnMouseDown() {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    /// <summary>
    /// When dragged, the object changes its position to follow the mouse
    /// </summary>
    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    /// <summary>
    /// when let go, the object snaps to a grid space if it is in one
    /// </summary>
    void OnMouseUp()
    {
        Vector3? gridSnapPosition = ShipBuildingGrid.instance.PostionToGridPosition(transform.position);
        if (gridSnapPosition != null) {
            transform.position = (Vector3) gridSnapPosition;
        }
    }
}
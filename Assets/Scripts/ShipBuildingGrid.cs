using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipBuildingGrid : MonoBehaviour {
    
    public static ShipBuildingGrid instance {get; private set;}
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject[] spacecraftParts;
    [SerializeField] private GameObject spaceshipPrefab;
    [SerializeField] private GridVisualizer gridVisualizer;
    
    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new Vector3(-2.5f, -4f);
    
    private (int, int) selectedTileCoords;
    private bool someTileSelected = false;
    
    private GameObject spaceship;
    private BoxCollider2D spaceshipCollider;
    private SpriteRenderer spaceshipSpriteRenderer;
    private List<GameObject> attachedParts = new List<GameObject>();
    private List<Transform> shipSnapPoints = new List<Transform>();
    
    private void Awake()
    {
        instance = this;
        grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);
    } 
    
    private void Start() {
        gameInput.OnNumKeyPerformedAction += GameInput_OnNumKeyAction;
        gameInput.OnLeftMouseClickPerformedAction += GameInput_OnLeftMouseClickAction;
    }

    private void CreateSpacecraft() {
        Instantiate(spacecraftPrefab);
        spacecraftPrefab.transform.position = GridCoordinatesToUnityPosition(gridWidth / 2, gridHeight / 2);
        grid.SetValue(gridWidth / 2, gridHeight / 2, partDatabase.GetPartID(spacecraftPrefab));
    }
    
    private void CreateShipSnapPoints() {
        foreach (var snapPoint in shipSnapPoints) {
            if (snapPoint != null) Destroy(snapPoint.gameObject);
        }
        shipSnapPoints.Clear();
        
        if (spaceship == null) return;
        
        Bounds shipBounds;
        if (spaceshipSpriteRenderer != null) {
            shipBounds = spaceshipSpriteRenderer.bounds;
        } else if (spaceshipCollider != null) {
            shipBounds = spaceshipCollider.bounds;
        } else {
            return;
        }
        
        Vector3 shipCenter = spaceship.transform.position;
        float shipHalfWidth = shipBounds.extents.x;
        float shipHalfHeight = shipBounds.extents.y;
        
        int pointsPerSide = 5;
        for (int i = 0; i < pointsPerSide; i++) {
            float t = (float)i / (pointsPerSide - 1);
            float x = shipCenter.x - shipHalfWidth + (shipHalfWidth * 2f * t);
            CreateSnapPoint(new Vector3(x, shipCenter.y + shipHalfHeight, 0), "Top_" + i);
        }
        
        for (int i = 0; i < pointsPerSide; i++) {
            float t = (float)i / (pointsPerSide - 1);
            float x = shipCenter.x - shipHalfWidth + (shipHalfWidth * 2f * t);
            CreateSnapPoint(new Vector3(x, shipCenter.y - shipHalfHeight, 0), "Bottom_" + i);
        }
        
        for (int i = 0; i < pointsPerSide; i++) {
            float t = (float)i / (pointsPerSide - 1);
            float y = shipCenter.y - shipHalfHeight + (shipHalfHeight * 2f * t);
            CreateSnapPoint(new Vector3(shipCenter.x - shipHalfWidth, y, 0), "Left_" + i);
        }
        
        for (int i = 0; i < pointsPerSide; i++) {
            float t = (float)i / (pointsPerSide - 1);
            float y = shipCenter.y - shipHalfHeight + (shipHalfHeight * 2f * t);
            CreateSnapPoint(new Vector3(shipCenter.x + shipHalfWidth, y, 0), "Right_" + i);
        }
    }
    
    private void CreateSnapPoint(Vector3 position, string name) {
        GameObject snapPoint = new GameObject("SnapPoint_" + name);
        snapPoint.transform.SetParent(spaceship.transform);
        snapPoint.transform.position = position;
        shipSnapPoints.Add(snapPoint.transform);
    }
    
    private Sprite CreateRectangleSprite() {
        Texture2D texture = new Texture2D(100, 100);
        Color[] pixels = new Color[100 * 100];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f), 100f);
    }

    private Vector3 GridCoordinatesToUnityPosition((int, int) gridCoords) {
        float x = gridOriginPosition.x + cellSize / 2 + (cellSize * gridCoords.Item1);
        float y = gridOriginPosition.y + cellSize / 2 +(cellSize * gridCoords.Item2);

        return new Vector3(x, y);
    }
    
    private void GameInput_OnNumKeyAction(object sender, GameInput.NumKeyEventArgs e) {
        if (!someTileSelected) return;

        GameObject spacecraftPart = Instantiate(spacecraftParts[e.key - 1]);
        
        spacecraftPart.SetActive(true);
        spacecraftPart.transform.position = GridCoordinatesToUnityPosition(selectedTileCoords);
    }
    
    private void GameInput_OnLeftMouseClickAction(object sender, System.EventArgs e) {
        (int, int) clickCoords;
        grid.GetXY(Mouse.GetMouseWorldPosition(), out clickCoords.Item1, out clickCoords.Item2);

        if (clickCoords.Item1 < 0 || clickCoords.Item2 < 0 ||
            clickCoords.Item1 >= gridWidth || clickCoords.Item2 >= gridHeight) {
                
            someTileSelected = false;
            return;
        }

        someTileSelected = true;
        selectedTileCoords = clickCoords;
    }
    
    public Vector3? PostionToGridPosition(Vector3 originalPosition)
    {
        (int, int) tileCoords;
        grid.GetXY(originalPosition, out tileCoords.Item1, out tileCoords.Item2);

        if (tileCoords.Item1 < 0 || tileCoords.Item2 < 0 ||
            tileCoords.Item1 >= gridWidth || tileCoords.Item2 >= gridHeight) {
            return null;
        }
        return GridCoordinatesToUnityPosition(tileCoords);
    }
    
    // Snaps part to ship surface using snap points
    public Vector3? SnapToShipSurface(Vector3 partPosition, BoxCollider2D partCollider) {
        if (spaceship == null || partCollider == null || shipSnapPoints.Count == 0) return null;
        
        List<Vector3> partSnapPoints = GetPartSnapPoints(partPosition, partCollider);
        
        float snapRange = 1.5f;
        Transform closestShipSnapPoint = null;
        Vector3 closestPartSnapPoint = Vector3.zero;
        float closestDistance = float.MaxValue;
        
        foreach (Transform shipSnapPoint in shipSnapPoints) {
            foreach (Vector3 partSnapPoint in partSnapPoints) {
                float distance = Vector3.Distance(shipSnapPoint.position, partSnapPoint);
                if (distance < closestDistance && distance <= snapRange) {
                    closestDistance = distance;
                    closestShipSnapPoint = shipSnapPoint;
                    closestPartSnapPoint = partSnapPoint;
                }
            }
        }
        
        if (closestShipSnapPoint != null) {
            Vector3 offset = closestShipSnapPoint.position - closestPartSnapPoint;
            return partPosition + offset;
        }
        
        return null;
    }
    
    // Gets snap points at part edges and corners
    private List<Vector3> GetPartSnapPoints(Vector3 partPosition, BoxCollider2D partCollider) {
        List<Vector3> snapPoints = new List<Vector3>();
        Vector2 partSize = partCollider.size;
        float partHalfWidth = partSize.x * 0.5f;
        float partHalfHeight = partSize.y * 0.5f;
        
        snapPoints.Add(new Vector3(partPosition.x, partPosition.y + partHalfHeight, 0));
        snapPoints.Add(new Vector3(partPosition.x, partPosition.y - partHalfHeight, 0));
        snapPoints.Add(new Vector3(partPosition.x - partHalfWidth, partPosition.y, 0));
        snapPoints.Add(new Vector3(partPosition.x + partHalfWidth, partPosition.y, 0));
        
        snapPoints.Add(new Vector3(partPosition.x - partHalfWidth, partPosition.y + partHalfHeight, 0));
        snapPoints.Add(new Vector3(partPosition.x + partHalfWidth, partPosition.y + partHalfHeight, 0));
        snapPoints.Add(new Vector3(partPosition.x - partHalfWidth, partPosition.y - partHalfHeight, 0));
        snapPoints.Add(new Vector3(partPosition.x + partHalfWidth, partPosition.y - partHalfHeight, 0));
        
        return snapPoints;
    }
    
    // Attaches part to ship as a child
    public void AttachPartToShip(GameObject part) {
        if (spaceship == null || part == null) return;
        
        if (!attachedParts.Contains(part)) {
            attachedParts.Add(part);
            Vector3 worldPosition = part.transform.position;
            part.transform.SetParent(spaceship.transform);
            part.transform.position = worldPosition;
        }
    }
    
    // Checks if position overlaps with ship
    public bool WouldOverlapWithShip(Vector3 position, BoxCollider2D partCollider) {
        if (spaceshipCollider == null || partCollider == null) return false;
        
        Bounds shipBounds = spaceshipCollider.bounds;
        Bounds partBounds = new Bounds(position, partCollider.bounds.size);
        
        return shipBounds.Intersects(partBounds);
    }
}

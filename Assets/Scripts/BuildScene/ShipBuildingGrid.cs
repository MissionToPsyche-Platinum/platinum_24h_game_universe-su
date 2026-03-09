using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Class that controls the ship building grid. allows the user to place objects and connect them to the base part.
/// </summary>
public class ShipBuildingGrid : MonoBehaviour {
    public static ShipBuildingGrid Instance { get; private set; }
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject spacecraft;
    [SerializeField] private GridVisualizer gridVisualizer;
    [SerializeField] private GameObject highlightTransform;

    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new(-2.5f, -4f);
    private static readonly Color colorHighlight   = new Color(1f, 1f, 0.3f, 0.4f);
    private static readonly Color colorHighlightInvisible   = new Color(1f, 1f, 0.3f, 0f);
    

    private GameObject selectedPart;
    private (int, int) selectedTileCoords;
    private readonly Dictionary<(int, int), GameObject> placedParts = new();
    private bool someTileSelected = false;
    private SpriteRenderer highlightSprite;
    
    private SpacecraftPartDatabase partDB;
    
    private void Awake() {
        Instance = this;
        
        grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);
        partDB = SpacecraftPartDatabase.Instance;
        highlightSprite = highlightTransform.GetComponent<SpriteRenderer>();
        highlightSprite.color = colorHighlightInvisible;

        CreateSpacecraft();
        gridVisualizer.VisualizeGrid(grid, gridWidth, gridHeight, cellSize, gridOriginPosition);
    } 
    
    private void Start() {
        gameInput.OnDeletePartPerformedAction += GameInput_OnDeletePartPerformedAction;
        gameInput.OnLeftMouseClickPerformedAction += GameInput_OnLeftMouseClickAction;
    }

    private void CreateSpacecraft() {
        spacecraft.transform.position = GridCoordinatesToUnityPosition(gridWidth / 2, gridHeight / 2);

        int baseID = partDB.GetPartID(partDB.GetPartGameObject(0));
        (int, int) baseCoords = (gridWidth / 2, gridHeight / 2);

        // Mark the base tile as occupied in the int grid
        grid.SetValue(baseCoords.Item1, baseCoords.Item2, baseID);

    }

    public void SetGridCellValue((int, int) coordinates, int value) {
        grid.SetValue(coordinates.Item1, coordinates.Item2, value);
    }
    
    public int GetGridCellValue((int, int) coordinates) => grid.GetValue(coordinates);

    public void SetGridCellValueByUnityPosition(Vector3 position, int value) {
        (int, int) coordinates = UnityPositionToGridCoordinates(position);
        
        SetGridCellValue(coordinates, value);
    }
    
    public Vector3 GridCoordinatesToUnityPosition(int x, int y) => GridCoordinatesToUnityPosition((x, y));

    public Vector3 GridCoordinatesToUnityPosition((int, int) gridCoords) {
        float x = gridOriginPosition.x + cellSize / 2 + (cellSize * gridCoords.Item1);
        float y = gridOriginPosition.y + cellSize / 2 + (cellSize * gridCoords.Item2);

        return new Vector3(x, y);
    }

    public (int, int) UnityPositionToGridCoordinates(Vector3 unityPosition) {
        int x;
        int y;
        
        grid.GetXY(unityPosition, out x, out y);

        return (x, y);
    }


    private void GameInput_OnDeletePartPerformedAction(object sender, System.EventArgs e) {
        if (!someTileSelected) return;
        
        DeletePart(selectedTileCoords);
    }

    private void DeletePart((int, int) partCoords) {
        // Find the real part object in this tile
        if (!placedParts.TryGetValue(partCoords, out GameObject partToDelete) || partToDelete == null) return;

        // Don't allow deleting the base/root part (optional safety)
        if (partToDelete == spacecraft) return;

        if (partToDelete.TryGetComponent<Engine>(out _)) {
            AdjustEngineIDsForDeletion(partToDelete);
        }
        
        if(selectedPart == partToDelete) DeselectPart();

        Destroy(partToDelete);
        
        placedParts.Remove(partCoords);
        grid.SetValue(partCoords.Item1, partCoords.Item2, -1);
    }

    private void AdjustEngineIDsForDeletion(GameObject engineToBeDeleted) {
        if (!engineToBeDeleted.TryGetComponent<Engine>(out Engine deletedEngine)) return;
        int engineID = deletedEngine.engineID;
        int totalEngines = Engine.totalEngineCount;

        Engine.totalEngineCount--;

        if (engineID == totalEngines) return;

        foreach (Transform child in spacecraft.transform) {
            if (!child.TryGetComponent(out Engine otherEngine)) continue;

            if (otherEngine.engineID > engineID) otherEngine.engineID--;
        }
    }

    private void GameInput_OnLeftMouseClickAction(object sender, System.EventArgs e) {
        HandleLeftClick();
    }
    
    private void HandleLeftClick() {
        Vector3 mousePosition = Mouse.GetMouseWorldPosition();

        (int, int) clickCoords;
        grid.GetXY(mousePosition, out clickCoords.Item1, out clickCoords.Item2);

        if (CoordinatesAreOutsideGrid(clickCoords)) {
            DeselectPart();
            return;
        }

        Vector3? snapped = PostionToGridPosition(mousePosition);
        if (snapped == null) {
            DeselectPart();
            return;
        }

        highlightTransform.transform.position = snapped.Value;
        highlightSprite.color = colorHighlight;

        someTileSelected = true;
        selectedTileCoords = clickCoords;

        // select actual object tracked in that tile
        if (placedParts.TryGetValue(selectedTileCoords, out GameObject partInTile)) {
            selectedPart = partInTile;
        } else {
            selectedPart = null;
        }
    }

    private void DeselectPart() {
        highlightSprite.color = colorHighlightInvisible;
        someTileSelected = false;
        selectedPart = null;
    }

    public bool CanPlacePart(GameObject partToBePlaced, (int, int) coords) {
        List<string> possibleConnectionsOfPartToBePlaced = partDB.GetSnapableDirections(partToBePlaced);
        int x = coords.Item1;
        int y = coords.Item2;
        
        if (grid.GetValue(coords) != -1) return false; 

        foreach (string snapableDirection in possibleConnectionsOfPartToBePlaced) {
            switch (snapableDirection) {
                case "above":
                    if (PartCanConnect(grid.GetValue((x, y + 1)), "below")) return true;
                    break;
                case "below":
                    if (PartCanConnect(grid.GetValue((x, y - 1)), "above")) return true;
                    break;
                case "left":
                    if (PartCanConnect(grid.GetValue((x - 1, y)), "right")) return true;
                    break;
                case "right":
                    if (PartCanConnect(grid.GetValue((x + 1, y)), "left"))  return true;
                    break;
                default:
                    continue;
            }
        }
        
        return false;
    }

    private void ConnectPartToSpacecraft(GameObject part) {
        FixedJoint2D joint = part.GetComponent<FixedJoint2D>();
        if (joint != null) {
            Rigidbody2D spacecraftRb = spacecraft.GetComponent<Rigidbody2D>();
            joint.connectedBody = spacecraftRb;
            joint.enableCollision = false;
        }
        
        // Make part kinematic during building phase but keep simulation enabled for mouse events
        Rigidbody2D partRb = part.GetComponent<Rigidbody2D>();
        if (partRb != null) {
            partRb.bodyType = RigidbodyType2D.Kinematic;
            partRb.simulated = true; 
        }
    }

    //Ex: If part is bottomEngine, and the connectingDirection is "above" it can connect
    private bool PartCanConnect(int partID, string connectingDirection) {
        if (partID < 0) return false;
        
        List<string> snapableDirections = partDB.GetSnapableDirections(partID);

        return snapableDirections.Contains(connectingDirection);
    }
    
    public Vector3? PostionToGridPosition(Vector3 originalPosition) {
        (int, int) tileCoords;
        grid.GetXY(originalPosition, out tileCoords.Item1, out tileCoords.Item2);

        if (CoordinatesAreOutsideGrid(tileCoords)) return null;
        
        return GridCoordinatesToUnityPosition(tileCoords);
    }

    public void SetSelectedPart(GameObject part) => selectedPart = part;

    public void PlacePartAtCoordinates(GameObject part, (int, int) coordinates) {
        // If something already exists here, destroy it first (true swap)
        if (placedParts.TryGetValue(coordinates, out GameObject existing) && existing != null) {
            // Don't allow swapping the base/root part (optional safety)
            if (existing == spacecraft) return;

            // If we are replacing an engine, adjust IDs first
            if (existing.TryGetComponent<Engine>(out _)){
                AdjustEngineIDsForDeletion(existing);
            }

            Destroy(existing);
            placedParts.Remove(coordinates);
            grid.SetValue(coordinates.Item1, coordinates.Item2, -1);
        }

        // Set grid value
        grid.SetValue(coordinates.Item1, coordinates.Item2, partDB.GetPartID(part));

        // Spawn part
        GameObject spacecraftPart = Instantiate(part, spacecraft.transform);
        spacecraftPart.SetActive(true);
        spacecraftPart.transform.position = GridCoordinatesToUnityPosition(coordinates);

        // Connect physics/joint
        ConnectPartToSpacecraft(spacecraftPart);

        // Track in dictionary
        placedParts[coordinates] = spacecraftPart;

        // Keep selection synced if placing in selected tile
        if (someTileSelected && selectedTileCoords.Equals(coordinates)){
            selectedPart = spacecraftPart;
        }
    }

    private bool CoordinatesAreOutsideGrid((int, int) coordinates) {
        if (coordinates.Item1 < 0 || coordinates.Item2 < 0 ||
            coordinates.Item1 >= gridWidth || coordinates.Item2 >= gridHeight) {
            
            return true;
        }

        return false;
    }


    public void RemovePlacedPartAtWorldPosition(Vector3 worldPos){
        (int, int) coords = UnityPositionToGridCoordinates(worldPos);
        placedParts.Remove(coords);
    }

    public void SetPlacedPartAtWorldPosition(Vector3 worldPos, GameObject partObject){
        (int, int) coords = UnityPositionToGridCoordinates(worldPos);
        placedParts[coords] = partObject;
    }

    public void RemoveDisconnectedParts() {
        for (int i = 0; i < gridWidth; i++) {
            for (int j = 0; j < gridHeight; j++) {
                if(!PartIsConnected((i, j))) DeletePart((i, j));
            }
        }
    }

    public bool PartIsConnected((int, int) coordinates) => PartIsConnectedHelper(coordinates, new HashSet<(int, int)>());

    private bool PartIsConnectedHelper((int, int) coordinates, HashSet<(int, int)> visitedCells) {
        if (!placedParts.ContainsKey(coordinates)) return false;
        
        visitedCells.Add(coordinates);
        
        int partID = GetGridCellValue(coordinates);
        int x = coordinates.Item1;
        int y = coordinates.Item2;
        
        List<string> snapableDirections = partDB.GetSnapableDirections(partID);

        foreach (string dir in snapableDirections) {
            int otherPart;
            switch (dir) {
                case "above":
                    otherPart = GetGridCellValue((x, y + 1));
                    if (otherPart == 0) return true;
                    if (otherPart > 0 && !visitedCells.Contains((x, y + 1))) {
                        if (!PartCanConnect(otherPart, "below")) continue;
                        if (PartIsConnectedHelper((x, y + 1), visitedCells)) return true;
                    }
                    break;
                case "below":
                    otherPart = GetGridCellValue((x, y - 1));
                    if (otherPart == 0) return true;
                    if (otherPart > 0 && !visitedCells.Contains((x, y - 1))) {
                        if (!PartCanConnect(otherPart, "above")) continue;
                        if (PartIsConnectedHelper((x, y - 1), visitedCells)) return true;
                    }
                    break;
                case "left":
                    otherPart = GetGridCellValue((x - 1, y));
                    if (otherPart == 0) return true;
                    if (otherPart > 0 && !visitedCells.Contains((x - 1, y))) {
                        if (!PartCanConnect(otherPart, "right")) continue;
                        if (PartIsConnectedHelper((x - 1, y), visitedCells)) return true;
                    }
                    break;
                case "right":
                    otherPart = GetGridCellValue((x + 1, y));
                    if (otherPart == 0) return true;
                    if (otherPart > 0 && !visitedCells.Contains((x + 1, y))) {
                        if (!PartCanConnect(otherPart, "left")) continue;
                        if (PartIsConnectedHelper((x + 1, y), visitedCells)) return true;
                    }
                    break;
                default:
                    continue;
            }
        }
        
        return false;
    }
    
    
}

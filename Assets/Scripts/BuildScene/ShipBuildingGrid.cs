using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class that controls the ship building grid. allows the user to place objects and connect them to the base part.
/// </summary>
public class ShipBuildingGrid : MonoBehaviour {
    public static ShipBuildingGrid Instance { get; private set; }
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject spacecraft;
    [SerializeField] private GridVisualizer gridVisualizer;
    
    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new(-2.5f, -4f);

    private GameObject selectedPart;
    private (int, int) selectedTileCoords;
    private bool someTileSelected = false;
    
    private SpacecraftPartDatabase partDB;
    
    private void Awake() {
        Instance = this;
        
        grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);
        partDB = SpacecraftPartDatabase.Instance;

        CreateSpacecraft();
        gridVisualizer.VisualizeGrid(grid, gridWidth, gridHeight, cellSize, gridOriginPosition);
    } 
    
    private void Start() {
        gameInput.OnNumKeyPerformedAction += GameInput_OnNumKeyAction;
        gameInput.OnDeletePartPerformedAction += GameInput_OnDeletePartPerformedAction;
        gameInput.OnLeftMouseClickPerformedAction += GameInput_OnLeftMouseClickAction;
    }

    private void CreateSpacecraft() {
        spacecraft.transform.position = GridCoordinatesToUnityPosition(gridWidth / 2, gridHeight / 2);
        grid.SetValue(gridWidth / 2, gridHeight / 2, partDB.GetPartID(partDB.GetPartGameObject(0)));
    }

    public void SetGridCellValue((int, int) coordinates, int value) {
        grid.SetValue(coordinates.Item1, coordinates.Item2, value);
    }

    public void SetGridCellValueByUnityPosition(Vector3 position, int value) {
        (int, int) coordinates = UnityPositionToGridCoordinates(position);
        
        SetGridCellValue(coordinates, value);
    }
    
    private Vector3 GridCoordinatesToUnityPosition(int x, int y) => GridCoordinatesToUnityPosition((x, y));
    
    private Vector3 GridCoordinatesToUnityPosition((int, int) gridCoords) {
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
    
    private void GameInput_OnNumKeyAction(object sender, GameInput.NumKeyEventArgs e) {
        int x = selectedTileCoords.Item1;
        int y = selectedTileCoords.Item2;
        GameObject part = partDB.GetPartGameObject(e.key);
        
        if (!someTileSelected) return;
        if (CanPlacePart(part, (x, y))) PlacePart(part, (x, y));
    }

    private void GameInput_OnDeletePartPerformedAction(object sender, System.EventArgs e) {
        if (partDB.GetPartID(selectedPart) == 2) AdjustEngineIDsForDeletion(selectedPart);
        
        Destroy(selectedPart);
        selectedPart = null;
        
        grid.SetValue(selectedTileCoords.Item1, selectedTileCoords.Item2, -1);
    }

    private void AdjustEngineIDsForDeletion(GameObject engineToBeDeleted) {
        int engineID = engineToBeDeleted.GetComponent<Engine>().engineID;
        Engine.totalEngineCount--;
        
        if (engineID == Engine.totalEngineCount) return;
        //If the above is not true, that means we are not deleting the most recently placed engine. This means that the
        //engine ID's will not be perfectly sequential (ex: we will have engines 1 and 3 but not 2). So we need to fix
        //that with everything below.
        
        foreach (Transform child in spacecraft.transform) {
            Engine otherEngine;
            if (!child.TryGetComponent(out otherEngine)) continue;

            if (otherEngine.engineID > engineID) otherEngine.engineID--;
        }
    }
    
    private void GameInput_OnLeftMouseClickAction(object sender, System.EventArgs e) {
        (int, int) clickCoords;
        grid.GetXY(Mouse.GetMouseWorldPosition(), out clickCoords.Item1, out clickCoords.Item2);

        if (clickCoords.Item1 < 0 || clickCoords.Item2 < 0 || 
            clickCoords.Item1 >= gridWidth || clickCoords.Item2 >= gridHeight) {
                
            someTileSelected = false;
            return;
        }

        if (grid.GetValue(clickCoords.Item1, clickCoords.Item2) == -1) selectedPart = null;
        
        someTileSelected = true;
        selectedTileCoords = clickCoords;
    }

    public bool CanPlacePart(GameObject partToBePlaced, (int, int) coords) {
        List<string> possibleConnectionsOfPartToBePlaced = partDB.GetSnapableDirections(partToBePlaced);
        int x = coords.Item1;
        int y = coords.Item2;
        
        if (grid.GetValue(x, y) != -1) return false; 

        foreach (string snapableDirection in possibleConnectionsOfPartToBePlaced) {
            switch (snapableDirection) {
                case "above":
                    if (PartCanConnect(grid.GetValue(x, y + 1), "below")) return true;
                    break;
                case "below":
                    if (PartCanConnect(grid.GetValue(x, y - 1), "above")) return true;
                    break;
                case "left":
                    if (PartCanConnect(grid.GetValue(x - 1, y), "right")) return true;
                    break;
                case "right":
                    if (PartCanConnect(grid.GetValue(x + 1, y), "left"))  return true;
                    break;
                default:
                    continue;
            }
        }
        
        return false;
    }
    
    private void PlacePart(GameObject part, (int, int) coordinates) {
        grid.SetValue(coordinates.Item1, coordinates.Item2, partDB.GetPartID(part));
        
        GameObject spacecraftPart = Instantiate(part, spacecraft.transform);
        
        spacecraftPart.SetActive(true);
        spacecraftPart.transform.position = GridCoordinatesToUnityPosition(selectedTileCoords);
        
        // Connect the part to the main spacecraft rigidbody
        ConnectPartToSpacecraft(spacecraftPart);
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

    private bool PartCanConnect(int partID, string connectingDirection) {
        if (partID < 0) return false;
        
        List<string> snapableDirections = partDB.GetSnapableDirections(partID);

        return snapableDirections.Contains(connectingDirection);
    }
    
    public Vector3? PostionToGridPosition(Vector3 originalPosition) {
        (int, int) tileCoords;
        grid.GetXY(originalPosition, out tileCoords.Item1, out tileCoords.Item2);

        if (tileCoords.Item1 < 0 || tileCoords.Item2 < 0 ||
            tileCoords.Item1 >= gridWidth || tileCoords.Item2 >= gridHeight) {
            
            return null;
        }
        
        return GridCoordinatesToUnityPosition(tileCoords);
    }

    public void SetSelectedPart(GameObject part) => selectedPart = part;
}

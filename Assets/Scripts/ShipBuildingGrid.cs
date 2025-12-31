using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipBuildingGrid : MonoBehaviour {
    public static ShipBuildingGrid instance { get; private set; }
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject spacecraft;
    [SerializeField] private GridVisualizer gridVisualizer;
    
    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new(-2.5f, -4f);
    
    private (int, int) selectedTileCoords;
    private bool someTileSelected = false;
    
    private void Awake() {
        instance = this;
        grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);

        CreateSpacecraft();
        gridVisualizer.VisualizeGrid(grid, gridWidth, gridHeight, cellSize, gridOriginPosition);
    } 
    
    private void Start() {
        gameInput.OnNumKeyPerformedAction += GameInput_OnNumKeyAction;
        gameInput.OnLeftMouseClickPerformedAction += GameInput_OnLeftMouseClickAction;
    }

    private void CreateSpacecraft() {
        spacecraft.transform.position = GridCoordinatesToUnityPosition(gridWidth / 2, gridHeight / 2);
        grid.SetValue(gridWidth / 2, gridHeight / 2, SpacecraftPartDatabase.Instance.GetPartID(SpacecraftPartDatabase.Instance.GetPartGameObject(0)));
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
        GameObject part = SpacecraftPartDatabase.Instance.GetPartGameObject(e.key);
        
        if (!someTileSelected) return;
        if (CanPlacePart(part, (x, y))) PlacePart(part, (x, y));
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

    public bool CanPlacePart(GameObject partToBePlaced, (int, int) coords) {
        List<string> possibleConnectionsOfPartToBePlaced = SpacecraftPartDatabase.Instance.GetSnapableDirections(partToBePlaced);
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
        grid.SetValue(coordinates.Item1, coordinates.Item2, SpacecraftPartDatabase.Instance.GetPartID(part));
        
        GameObject spacecraftPart = Instantiate(part, spacecraft.transform);
        
        spacecraftPart.SetActive(true);
        spacecraftPart.transform.position = GridCoordinatesToUnityPosition(selectedTileCoords);
    }

    private bool PartCanConnect(int partID, string connectingDirection) {
        if (partID < 0) return false;
        
        List<string> snapableDirections = SpacecraftPartDatabase.Instance.GetSnapableDirections(partID);

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
}

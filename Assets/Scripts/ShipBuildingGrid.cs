using System;
using UnityEngine;

public class ShipBuildingGrid : MonoBehaviour {
    
    public static ShipBuildingGrid instance {get; private set;}
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject[] spacecraftParts;
    
    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new Vector3(-2.5f, -4f);
    
    private (int, int) selectedTileCoords;
    private bool someTileSelected = false;
    private void Awake()
    {
        instance = this;
        grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);
    } 
    
    private void Start() {
        gameInput.OnNumKeyPerformedAction += GameInput_OnNumKeyAction;
        gameInput.OnLeftMouseClickPerformedAction += GameInput_OnLeftMouseClickAction;
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
}

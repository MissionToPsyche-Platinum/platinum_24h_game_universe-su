using System;
using UnityEngine;

public class ShipBuildingGrid : MonoBehaviour {
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject[] spacecraftParts;
    
    private Grid grid;
    private int gridWidth = 5;
    private int gridHeight = 7;
    private float cellSize = 1f;
    private Vector3 gridOriginPosition = new Vector3(-2.5f, -4f);
    
    private (int, int) selectedTileCoords;
    private bool someTileSelected = false;
    private void Awake() => grid = new Grid(gridWidth, gridHeight, cellSize, gridOriginPosition);
    
    private void Start() => gameInput.OnNumKeyPerformedAction += GameInput_OnNumKeyAction;
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) { //Left click
            //grid.SetValue(Mouse.GetMouseWorldPosition(), 56);
            Debug.Log("check1");
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
        
        
    }
    
    private void GameInput_OnNumKeyAction(object sender, GameInput.NumKeyEventArgs e) {
        if (!someTileSelected) return;

        GameObject spacecraftPart = Instantiate(spacecraftParts[e.key - 1]);
        
        spacecraftPart.SetActive(true);
        spacecraftPart.transform.position = GridCoordinatesToUnityPosition(selectedTileCoords);
    }

    private Vector3 GridCoordinatesToUnityPosition((int, int) gridCoords) {
        float x = gridOriginPosition.x + cellSize / 2 + (cellSize * gridCoords.Item1);
        float y = gridOriginPosition.y + cellSize / 2 +(cellSize * gridCoords.Item2);

        return new Vector3(x, y);
    }
    
}

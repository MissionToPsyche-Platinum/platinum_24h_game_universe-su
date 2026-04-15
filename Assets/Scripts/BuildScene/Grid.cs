using UnityEngine;

//Class defines the properties of the building grid. also allows for queries of whether a location is within the grid.

public class Grid {

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = -1;
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 10000f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 10000f);
            }
            
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 10000f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 10000f);
        }
    }

    private Vector3 GetWorldPosition(int x, int y) => new Vector3(x, y) * cellSize + originPosition;

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, int value) {
        if (IsInGridRange(x, y)) gridArray[x, y] = value;
    }

    public void SetValue(Vector3 worldPosition, int value) {
        int x;
        int y;
        
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
    
    public int GetValue((int, int) coordinates) {
        int x = coordinates.Item1;
        int y = coordinates.Item2;
        if(IsInGridRange(x, y)) return gridArray[x, y];
        
        return -2; //returns -2 if out of bounds
    }
    
    public int GetValue(Vector3 worldPosition) {
        int x;
        int y;
        
        GetXY(worldPosition, out x, out y);

        return GetValue((x, y));
    }
    
    private bool IsInGridRange(int x, int y) => (x >= 0 && y >= 0 && x < width && y < height);
    
    public void SaveGridState(bool save = true) {
        SpacecraftPartDatabase partDB = SpacecraftPartDatabase.Instance;
        Debug.Log(save);
        if (save) {
            partDB.savedGridState = gridArray;
            partDB.hasSavedGridState = true;
        } else {
            partDB.savedGridState = null;
            partDB.hasSavedGridState = false;
        }
    }
    
    public void LoadGridState() {
        SpacecraftPartDatabase partDB = SpacecraftPartDatabase.Instance;
        
        if(partDB.hasSavedGridState) gridArray = partDB.savedGridState;
    }
}

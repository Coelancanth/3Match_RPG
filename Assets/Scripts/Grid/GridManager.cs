using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // 单元格预制体

    private Grid gridData; // 网格数据

    void Start()
    {
        InitializeGridData();
        GenerateGridVisuals();
    }

    // 初始化网格数据
    public void InitializeGridData()
    {
        gridData = new Grid(GridConstants.Rows, GridConstants.Columns);
        // 设置初始数据，例如元素或敌人
        gridData.SetCellElement(2, 3, new Element("Fire", 1));
        gridData.SetCellEnemy(4, 4, "Goblin", 50);
    }

    // 动态生成可视化单元格
    void GenerateGridVisuals()
    {
        float cellSize = CalculateCellSize();
        Vector3 gridStart = CalculateGridStart(cellSize);

        for (int row = 0; row < GridConstants.Rows; row++)
        {
            for (int column = 0; column < GridConstants.Columns; column++)
            {
                Vector3 position = gridStart + new Vector3(column * cellSize, row * cellSize, 0);
                CreateGridCellVisual(row, column, position, cellSize);
                //Debug.Log($"Cell {row}, {column} position: {position}");

            }
        }
    }

    // 计算单元格大小
    float CalculateCellSize()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        return Mathf.Min(screenWidth / GridConstants.Columns, screenHeight / GridConstants.Rows);
    }

    // 计算网格起始位置
    Vector3 CalculateGridStart(float cellSize)
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        return new Vector3(
            -screenWidth / 2 + cellSize / 2,
            -screenHeight / 2 + cellSize / 2,
            0
        );
    }

    // 创建单元格视觉
    void CreateGridCellVisual(int row, int column, Vector3 position, float cellSize)
    {
        GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
        GridCellView view = cell.GetComponent<GridCellView>();

        // 调整大小
        Vector3 prefabSize = cell.GetComponent<SpriteRenderer>().bounds.size;
        float uniformScale = Mathf.Min(cellSize / prefabSize.x, cellSize / prefabSize.y);
        cell.transform.localScale = new Vector3(uniformScale, uniformScale, 1);

        // 初始化视图
        GridCell cellData = gridData.GetCell(row, column);
        view.Initialize(row, column, DetermineCellColor(cellData));
        cell.name = $"Cell_{row}_{column}";
    }

    // 根据元素类型决定颜色
    Color DetermineCellColor(GridCell cell)
    {
        if (cell.Element != null)
        {
            switch (cell.Element.Type)
            {
                case "Fire": return Color.red;
                case "Water": return Color.blue;
                case "Grass": return Color.green;
            }
        }
        return Color.white;
    }

        public GridCell GetCell(int row, int column)
    {
        return gridData.GetCell(row, column);
    }
}

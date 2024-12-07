using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // 单元格预制体
    public int rows = 10; // 网格行数
    public int columns = 10; // 网格列数

    private Grid gridData; // 网格数据

    void Start()
    {
        InitializeGridData();
        GenerateGridVisuals();
    }

    // 初始化网格数据
    void InitializeGridData()
    {
        gridData = new Grid();
        // 设置初始数据，例如敌人或元素
        gridData.SetCellElement(2, 3, "Fire", 1, "Active");
        gridData.SetCellEnemy(4, 4, "Goblin", 50);
    }

    // 动态生成可视化单元格
void GenerateGridVisuals()
{
    // 获取屏幕的世界空间大小
    float screenHeight = Camera.main.orthographicSize * 2;
    float screenWidth = screenHeight * Screen.width / Screen.height;

    // 计算单元格的大小
    float cellWidth = screenWidth / columns;
    float cellHeight = screenHeight / rows;
    float cellSize = Mathf.Min(cellWidth, cellHeight);

    // 调整网格起始位置，使网格居中
    Vector3 gridStart = new Vector3(
        -screenWidth / 2 + cellSize / 2,
        -screenHeight / 2 + cellSize / 2,
        0
    );

    // 动态生成单元格
    for (int row = 0; row < rows; row++)
    {
        for (int column = 0; column < columns; column++)
        {
            // 计算单元格的世界空间坐标
            Vector3 position = gridStart + new Vector3(column * cellSize, row * cellSize, 0);

            // 创建单元格对象
            GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);

            // 调整单元格的缩放以匹配尺寸
            Vector3 prefabOriginalSize = cell.GetComponent<SpriteRenderer>().bounds.size;
            float scaleFactorX = cellSize / prefabOriginalSize.x;
            float scaleFactorY = cellSize / prefabOriginalSize.y;

            // 统一按最小比例缩放，避免拉伸
            float uniformScale = Mathf.Min(scaleFactorX, scaleFactorY);
            cell.transform.localScale = new Vector3(uniformScale, uniformScale, 1);

            // 初始化单元格显示
            GridCell cellData = gridData.GetCell(row, column);
            Color cellColor = DetermineCellColor(cellData);
            cell.GetComponent<GridCellView>().Initialize(row, column, cellColor);

            // 设置单元格名称，便于调试
            cell.name = $"Cell_{row}_{column}";
        }
    }
}


    // 根据网格数据设置单元格颜色
    Color DetermineCellColor(GridCell cell)
    {
        if (cell.ElementType == "Fire") return Color.red;
        if (cell.ElementType == "Water") return Color.blue;
        if (cell.ElementType == "Grass") return Color.green;
        return Color.white;
    }
}

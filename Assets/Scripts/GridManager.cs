using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // 单元格预制体
    public int rows = 10; // 网格行数
    public int columns = 10; // 网格列数
    public float cellSize = 1.1f; // 单元格的间距大小

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
        // 可以在这里设置初始数据，例如敌人或元素
        gridData.SetCellElement(2, 3, "Fire", 1, "Active");
        gridData.SetCellEnemy(4, 4, "Goblin", 50);
    }

    // 动态生成可视化单元格
    void GenerateGridVisuals()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // 计算单元格在世界空间的坐标
                Vector3 position = new Vector3(column * cellSize, row * cellSize, 0);

                // 创建单元格对象
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);

                // 初始化单元格的显示
                GridCell cellData = gridData.GetCell(row, column);
                Color cellColor = DetermineCellColor(cellData);
                cell.GetComponent<GridCellView>().Initialize(row, column, cellColor);

                // 给单元格设置名字便于调试
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

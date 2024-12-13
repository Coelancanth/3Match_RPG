using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float cellSpacing = 1.1f; // 替代硬编码的间距
    [SerializeField] private Color[] elementColors; // 元素颜色配置

    public event System.Action<GridCell> OnCellCreated;
    public event System.Action<Element> OnElementSpawned;

    public GameObject cellPrefab; // 单元格预制体

    public Grid gridData; // 网格数据

    



    public DiceManager diceManager;

    void Start()
    {
        InitializeGridData();
        GenerateGridVisuals();
        //gridData.RandomSpawn(5);
        
        
        // 初始化骰子管理器并添加初始骰子
        InitializeDiceManager();

        SpawnDiceGeneratedElements();
    }

    
    // 使用骰子生成元素
    public void SpawnDiceGeneratedElements()
    {
        List<Element> rolledElements = diceManager.RollAllDice();
        List<GridCell> emptyCells = GetEmptyCells();

        for (int i = 0; i < rolledElements.Count && i < emptyCells.Count; i++)
        {
            if(rolledElements[i] != null)
            {
                int randomIndex = Random.Range(0, emptyCells.Count);
                emptyCells[randomIndex].Element = rolledElements[i];
                //emptyCells[i].Element = rolledElements[i];
                OnElementSpawned?.Invoke(rolledElements[i]);
            }
        }
    }

    // 获取所有空白格子
    private List<GridCell> GetEmptyCells()
    {
        List<GridCell> emptyCells = new List<GridCell>();
        for (int row = 0; row < gridData.Rows; row++)
        {
            for (int col = 0; col < gridData.Columns; col++)
            {
                GridCell cell = gridData.GetCell(row, col);
                if (cell.Element == null)
                {
                    emptyCells.Add(cell);
                }
            }
        }
        return emptyCells;
    }

    // 初始化网格数据
    public void InitializeGridData()
    {
        gridData = new Grid(GridConstants.Rows, GridConstants.Columns);
        // 设置初始数据，例如元素或敌人
        //gridData.SetCellElement(0, 0, new Element("Fire", 1));
        //gridData.SetCellEnemy(4, 4, "Goblin", 50);
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
        GridCell gridCell = gridData.GetCell(row, column);
        view.SubscribeToCell(gridCell);

        // 调整大小
        Vector3 prefabSize = cell.GetComponent<SpriteRenderer>().bounds.size;
        float uniformScale = Mathf.Min(cellSize / prefabSize.x, cellSize / prefabSize.y);
        cell.transform.localScale = new Vector3(uniformScale, uniformScale, 1);

        // 初始化视图
        GridCell cellData = gridData.GetCell(row, column);
        view.Initialize(row, column, DetermineCellColor(cellData));


        cell.name = $"Cell_{row}_{column}";

        var cellObject = new GridCell(row, column);
        cellObject.View = view;  // 设置View引用
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



    public List<GridCell> GetMovableRange(GridCell cell, int range = 1)
    {
        List<GridCell> movableCells = new List<GridCell>();

        int[][] directions = new int[][]
        {
            new int[] { 0, 1 },  // 上
            new int[] { 0, -1 }, // 下
            new int[] { 1, 0 },  // 右
            new int[] { -1, 0 }  // 左
        };

        foreach (var dir in directions)
        {
            int newRow = cell.Row + dir[0];
            int newCol = cell.Column + dir[1];
            GridCell neighbor = gridData.GetCell(newRow, newCol);

            if (neighbor != null && neighbor.Element == null) // 示例规则：目标单元格为空
            {
                movableCells.Add(neighbor);
            }
        }

        return movableCells;
    }

public GridCellView GetCellView(int row, int column)
{
    string cellName = $"Cell_{row}_{column}";
    Transform cellTransform = transform.Find(cellName);
    return cellTransform?.GetComponent<GridCellView>();
}

    private void InitializeDiceManager()
    {
        diceManager = new DiceManager();
        // 从配置文件加载初始骰子
        var initialDice = LoadInitialDice();
        foreach(var dice in initialDice)
        {
            diceManager.AddDice(dice);
        }
    }

    private Element CreateRandomElement()
    {
        string[] elementTypes = new[] { "Fire", "Water", "Earth", "Air" };
        string randomType = elementTypes[Random.Range(0, elementTypes.Length)];
        return new Element(randomType, 1);
    }

    public bool IsGridFull()
    {
        return GetEmptyCells().Count == 0;
    }

    public void ClearGrid()
    {
        for (int row = 0; row < gridData.Rows; row++)
        {
            for (int col = 0; col < gridData.Columns; col++)
            {
                gridData.SetCellElement(row, col, null);
            }
        }
    }

    private List<Dice> LoadInitialDice()
    {
        List<Dice> initialDice = new List<Dice>();
        
        // 添加一个1级火焰骰子
        //initialDice.Add(Dice.CreateFireDice(1));
        
        return initialDice;
    }
}

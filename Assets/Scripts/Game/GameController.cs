using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridManager gridManager; // 引用网格管理器
    private int turnCount = 0; // 当前回合数
    private bool isPlayerTurn = true; // 是否为玩家回合

    private Vector3 dragStart; // 鼠标拖曳起点
    private Vector3 dragEnd; // 鼠标拖曳终点
    private bool isDragging = false;

    void Start()
    {
        StartGame();
    }

    // 初始化游戏
    void StartGame()
    {
        Debug.Log("Game Started!");
        gridManager.InitializeGridData(); // 初始化网格数据
        BeginTurn();
    }

    // 开始新回合
    void BeginTurn()
    {
        turnCount++;
        Debug.Log($"Turn {turnCount} begins!");
        isPlayerTurn = true;
    }

    // 游戏的主循环：监听输入
    void Update()
    {
        if (!isPlayerTurn && !isDebugMode) return;

        HandleMouseInput(); // 处理鼠标输入
        HandleKeyboardInput(); // 处理键盘输入
    }

void HandleMouseInput()
{
    // 鼠标按下
    if (Input.GetMouseButtonDown(0))
    {
        dragStart = Input.mousePosition; // 保持屏幕空间的坐标
        isDragging = false;
    }

    // 鼠标拖曳
    if (Input.GetMouseButton(0))
    {
        Vector3 currentPosition = Input.mousePosition;
        if (Vector3.Distance(dragStart, currentPosition) > 5f) // 拖曳的屏幕像素阈值
        {
            isDragging = true;
        }
    }

    // 鼠标释放
    if (Input.GetMouseButtonUp(0))
    {
        dragEnd = Input.mousePosition; // 保持屏幕空间的坐标

        if (isDragging)
        {
            HandleMouseDrag(dragStart, dragEnd);
        }
        else
        {
            GridCell clickedCell = GetClickedCell(dragStart);
            if (clickedCell != null)
            {
                PerformActionOnCell(clickedCell);
            }
        }

        // 重置拖曳标志
        isDragging = false;
    }
}


private bool isDebugMode = false; // 标志是否处于调试模式

void HandleKeyboardInput()
{
    if (Input.GetKeyDown(KeyCode.D)) // 按下 D 键进入或退出调试模式
    {
        isDebugMode = !isDebugMode;
        Debug.Log($"Debug Mode: {(isDebugMode ? "ON" : "OFF")}");
    }

    if (isDebugMode && Input.GetMouseButtonDown(0)) // 调试模式下点击鼠标左键
    {
        Vector3 mousePosition = Input.mousePosition;
        GridCell clickedCell = GetClickedCell(mousePosition);

        if (clickedCell != null)
        {
            ChangeElementInCell(clickedCell);
        }
    }
}

void ChangeElementInCell(GridCell cell)
{
    // 示例逻辑：循环更改元素类型
    string newType;
    if (cell.Element == null)
    {
        newType = "Fire"; // 如果没有元素，初始化为 Fire
    }
    else
    {
        // 根据当前类型设置下一个类型
        switch (cell.Element.Type)
        {
            case "Fire": newType = "Water"; break;
            case "Water": newType = "Grass"; break;
            case "Grass": newType = null; break; // 重置为无元素
            default: newType = "Fire"; break;
        }
    }

    // 更新元素
    cell.Element = newType != null ? new Element(newType, 1) : null;

    // 更新视图
    GridCellView cellView = gridManager.GetCellView(cell.Row, cell.Column);
    if (cellView != null)
    {
        cellView.UpdateElementInfo(cell);
        UpdateCellColor(cellView, cell);
    }

    Debug.Log($"Changed Element in Cell ({cell.Row}, {cell.Column}) to {newType ?? "None"}");
}





void HandleMouseDrag(Vector3 start, Vector3 end)
{
    GridCell startCell = GetClickedCell(start);
    GridCell endCell = GetClickedCell(end);

    if (startCell != null && endCell != null)
    {
        if (startCell.IsMovable() && endCell.Element == null) // 检查拖曳条件
        {
            // 更新元素交换逻辑
            Element tempElement = startCell.Element;
            startCell.Element = endCell.Element;
            endCell.Element = tempElement;

            // 更新视图
            GridCellView startCellView = gridManager.GetCellView(startCell.Row, startCell.Column);
            GridCellView endCellView = gridManager.GetCellView(endCell.Row, endCell.Column);

            // 更新调试信息
            startCellView.UpdateElementInfo(startCell);
            endCellView.UpdateElementInfo(endCell);

            // 更新颜色
            UpdateCellColor(startCellView, startCell);
            UpdateCellColor(endCellView, endCell);

            //Debug.Log($"Moved Element from Cell ({startCell.Row}, {startCell.Column}) to Cell ({endCell.Row}, {endCell.Column})");
        }
        else
        {
            //Debug.Log("Invalid drag: Start cell is not movable or end cell is occupied.");
        }
    }
    else
    {
        //Debug.Log("Invalid drag: One or both cells are null.");
    }
}

void UpdateCellColor(GridCellView cellView, GridCell cell)
{
    SpriteRenderer spriteRenderer = cellView.GetComponentInChildren<SpriteRenderer>();
    if (cell.Element != null)
    {
        spriteRenderer.color = GetColorForElementType(cell.Element.Type);
    }
    else
    {
        spriteRenderer.color = Color.white; // 无元素时使用默认白色
    }
}

Color GetColorForElementType(string elementType)
{
    switch (elementType)
    {
        case "Fire": return Color.red;
        case "Water": return Color.blue;
        case "Grass": return Color.green;
        default: return Color.gray; // 未知类型使用灰色
    }
}



GridCell GetClickedCell(Vector3 screenPosition)
{
    Ray ray = Camera.main.ScreenPointToRay(screenPosition);

    //Debug.DrawRay(ray.origin, ray.direction * 100, Color.green, 2f); // 用于调试的射线可视化

    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
    {
        GridCellView cellView = hit.collider.GetComponent<GridCellView>();
        if (cellView != null)
        {
            //Debug.Log($"Hit Cell: Row {cellView.Row}, Column {cellView.Column}");
            return gridManager.GetCell(cellView.Row, cellView.Column);
        }
        else
        {
            Debug.Log("Hit object does not have GridCellView component.");
        }
    }
    else
    {
        Debug.Log("Raycast did not hit any object.");
    }

    return null;
}


    // 对单元格执行操作
    void PerformActionOnCell(GridCell cell)
    {
        if (cell.Element != null)
        {
            Debug.Log($"Performing action on Element: {cell.Element.Type}, Level {cell.Element.Level}");
            cell.Element.Upgrade(); // 示例：升级元素
        }
    }

    // 结束玩家回合
    void EndPlayerTurn()
    {
        Debug.Log("Player turn ends!");
        isPlayerTurn = false;
        EndTurn();
    }

    // 结束当前回合
    void EndTurn()
    {
        Debug.Log("End of Turn");
        BeginTurn(); // 开始下一回合
    }
}
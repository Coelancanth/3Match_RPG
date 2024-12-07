using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridManager gridManager; // 引用网格管理器
    private int turnCount = 0; // 当前回合数
    private bool isPlayerTurn = true; // 是否为玩家回合

    private Vector3 dragStart; // 鼠标拖曳起点
    private Vector3 dragEnd; // 鼠标拖曳终点

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
        if (!isPlayerTurn) return;

        HandleMouseInput(); // 处理鼠标输入
        HandleKeyboardInput(); // 处理键盘输入
    }

    // 处理鼠标输入
    void HandleMouseInput()
    {
        // 鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            GridCell clickedCell = GetClickedCell(mousePosition);

            if (clickedCell != null)
            {
                Debug.Log($"Mouse Clicked Cell: Row {clickedCell.Row}, Column {clickedCell.Column}");
                PerformActionOnCell(clickedCell);
            }
        }

        // 鼠标拖曳开始
        if (Input.GetMouseButtonDown(0))
        {
            dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragStart.z = 0;
        }

        // 鼠标拖曳结束
        if (Input.GetMouseButtonUp(0))
        {
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEnd.z = 0;

            HandleMouseDrag(dragStart, dragEnd);
        }
    }

    // 处理键盘输入
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed!");
            EndPlayerTurn();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed! Restarting game...");
            StartGame();
        }
    }

    // 处理鼠标拖曳事件
    void HandleMouseDrag(Vector3 start, Vector3 end)
    {
        Debug.Log($"Mouse dragged from {start} to {end}");

        GridCell startCell = GetClickedCell(start);
        GridCell endCell = GetClickedCell(end);

        if (startCell != null && endCell != null)
        {
            Debug.Log($"Dragged from Cell ({startCell.Row}, {startCell.Column}) to Cell ({endCell.Row}, {endCell.Column})");
            // 在此处理拖曳逻辑，例如交换两个单元格的内容
        }
    }

    // 根据鼠标位置获取单元格
    GridCell GetClickedCell(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCellView cellView = hit.collider.GetComponent<GridCellView>();
            if (cellView != null)
            {
                //Debug.Log($"Hit Cell: Row {cellView.Row}, Column {cellView.Column}");
                return gridManager.GetCell(cellView.Row, cellView.Column);
            }
        }

        //Debug.Log("No Cell Hit");
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

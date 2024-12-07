using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridManager gridManager; // 引用网格管理器
    private int turnCount = 0; // 当前回合数
    private bool isPlayerTurn = true; // 是否为玩家回合

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

        // 可以在这里添加额外的回合初始化逻辑，例如生成新元素或触发特定事件。
    }

    // 更新循环，监听玩家输入
    void Update()
    {
        if (isPlayerTurn && Input.GetMouseButtonDown(0)) // 示例：监听鼠标点击事件
        {
            Vector3 moustPosition = Input.mousePosition;
            GridCell clickedCell = GetClickedCell(moustPosition);
            //HandlePlayerAction();
        }
    }

    // 处理玩家操作
    void HandlePlayerAction()
    {
        Debug.Log("Player Action Triggered");
        // 这里可以实现具体的玩家操作逻辑，例如点击一个网格单元格
        // 示例：获取鼠标点击位置
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"Mouse Position: ({mousePosition.x}, {mousePosition.y})");

        GridCell cell = GetClickedCell(mousePosition);

        if (cell != null)
        {
            Debug.Log($"Cell clicked: ({cell.Row}, {cell.Column})");
            // 对选中的单元格执行操作，例如移动元素或触发技能
            PerformActionOnCell(cell);
        }

        // 玩家回合结束
        EndPlayerTurn();
    }

GridCell GetClickedCell(Vector3 mousePosition)
    {

        // 从屏幕坐标转换为 Ray
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);


        // 发射射线检测
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCellView cellView = hit.collider.GetComponent<GridCellView>();
            if (cellView != null)
            {
                Debug.Log($"Hit Cell: Row {cellView.Row}, Column {cellView.Column}");
                return gridManager.GetCell(cellView.Row, cellView.Column);
            }
        }

        Debug.Log("No Cell Hit");
        return null;
    }

    



    // 对单元格执行操作
    void PerformActionOnCell(GridCell cell)
    {
        if (cell.Element != null)
        {
            Debug.Log($"Performing action on element: {cell.Element.Type}, Level {cell.Element.Level}");
            cell.Element.Upgrade(); // 示例：升级元素
        }
    }

    // 结束玩家回合
    void EndPlayerTurn()
    {
        Debug.Log("Player turn ends!");
        isPlayerTurn = false;

        // 可以在这里添加其他逻辑，例如切换到敌人回合
        EndTurn();
    }

    // 结束当前回合
    void EndTurn()
    {
        Debug.Log("End of Turn");
        BeginTurn(); // 开始下一回合
    }
}

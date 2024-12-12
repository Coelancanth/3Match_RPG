using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    public GridManager gridManager; // 引用网格管理器
    private MatchingSystem matchingSystem;
    private int turnCount = 0; // 当前回合数
    private bool isPlayerTurn = true; // 是否为玩家回合

    private Vector3 dragStart; // 鼠标拖曳起点
    private Vector3 dragEnd; // 鼠标拖曳终点
    private bool isDragging = false;

    private IMatchResolutionRule matchResolutionRule;
    private List<MatchingRule> matchingRules;
    

    void Start()
    {
        matchingSystem = new MatchingSystem(gridManager);
        matchResolutionRule = new BasicMatchResolutionRule();
        InitializeMatchingRules();
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
                //PerformActionOnCell(clickedCell);
            }
        }

        // 重置拖曳标志
        isDragging = false;
    }
}


public bool isDebugMode = false; // 标志是否处于调试模式

void HandleKeyboardInput()
{
    //if (Input.GetKeyDown(KeyCode.D)) // 按下 D 键进入或退出调试模式
    //{
        //isDebugMode = !isDebugMode;
        //Debug.Log($"Debug Mode: {(isDebugMode ? "ON" : "OFF")}");
    //}

    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && Input.GetMouseButtonDown(0)) // 检测 Shift 键
   {
       Vector3 mousePosition = Input.mousePosition;
       GridCell clickedCell = GetClickedCell(mousePosition);
       DetectMatching(clickedCell);
        //TriggerDebugFunction(); // 调用特定函数
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

    if (isDebugMode && Input.GetMouseButtonDown(1)) 
    {
        Vector3 mousePosition = Input.mousePosition;
        GridCell clickedCell = GetClickedCell(mousePosition);

        if (clickedCell != null)
        {
            PerformActionOnCell(clickedCell);
        }
    }

    // 按 T 键触发测试
    if (Input.GetKeyDown(KeyCode.T))
    {
        TestFireballEffect();
    }
}

// 新增函数：调试模式下激发的函数
void TriggerDebugFunction()
{
    Debug.Log("Debug function triggered by Shift key.");
    // 在此添加具体逻辑
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


            //gridManager.gridData.RandomSpawn(3);
            DetectMatching(endCell);

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


    // 对单��格执行操作
    void PerformActionOnCell(GridCell cell)
    {
        if (cell.Element != null)
        {
            Debug.Log($"Performing action on Element: {cell.Element.Type}, Level {cell.Element.Value}");
            //cell.Element.Upgrade(); // 示例：升级元素
            int upgradedValue = cell.Element.Value+1;
            cell.Element = new Element(cell.Element.Type, upgradedValue);
            
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






public void DetectMatching(GridCell triggerCell)
{
    if (triggerCell?.Element == null) return;

    var matchedGroups = matchingSystem.FindConnectedGroups();
    var adjacentConnectedGroups = matchingSystem.GetAdjacentConnectedGroups(triggerCell);
    
    // 筛选符合条件的相邻连通组
    var filteredGroups = matchingSystem.FilterAdjacentGroups(triggerCell, adjacentConnectedGroups);
    
    matchResolutionRule.ResolveMatch(filteredGroups, triggerCell);
    //foreach (var group in filteredGroups)
    //{
        //Debug.Log($"筛选后的联通组 - 类型：{group.ElementType}，数量：{group.Count}，总和：{group.Sum}");
        ////matchResolutionRule.ResolveMatch(group.Group, triggerCell);
    //}
}



private void InitializeMatchingRules()
{
    matchingRules = MatchingRuleConfig.GetDefaultRules();
}

// 在 GameController 中添加测试方法
public void TestFireballEffect()
{
    // 1. 创建测试布局
    CreateTestLayout();
    
    // 2. 获取中心位置的格子（1,1）并触发匹配
    //var centerCell = gridManager.GetCell(1, 1);
    //DetectMatching(centerCell);
}

private void CreateTestLayout()
{
    // 清空网格
    //gridManager.InitializeGridData();
    
    // 创建测试布局
    //var layout = new (int, int, string, int)[]
    //{
        //(0, 0, "Fire", 1),
        //(0, 1, "Water", 2),
        //(0, 2, "Fire", 2),
        //(1, 0, "Water", 1),
        //(1, 1, "Fire", 3),    // 这个将升级为火球
        //(1, 2, "Water", 1),
        //(2, 0, "Fire", 1),
        //(2, 1, "Water", 1),
        //(2, 2, "Fire", 1),
    //};
    var layout = new (int, int, string, int)[]
    {
        (0, 0, "Fireball", 1), 
        (0, 1, "Fire", 1),
        (0, 2, "Fire", 1),
        (1, 0, "Fire", 1),
        (1, 1, "Fire", 1),
        (1, 2, "Fire", 1),
        (2, 0, "Water", 1),
        (2, 1, "Water", 1),
        (2, 2, "Water", 1),
    };

    foreach (var (row, col, type, value) in layout)
    {
        //gridManager.GetCell(row, col).Element = new Element(type, value);
        gridManager.gridData.SetCellElement(row, col, new Element(type, value));
    }
}

}

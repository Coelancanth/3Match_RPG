using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    public GridManager gridManager; // 引用网格管理器
    private MatchingSystem matchingSystem;
    private int turnCount = 0; // 当前回合数
    private bool isPlayerTurn = true; // 是否为玩家回合

    private ClickAndDragDetector inputDetector;
    
    private IMatchResolutionRule matchResolutionRule;
    private List<MatchingRule> matchingRules;
    
    private GridCell pendingEffectSource = null;  // 待释放效果的源格子
    private List<GridCell> highlightedCells = new List<GridCell>();  // 当前高亮的格子
    private bool isWaitingForEffectTarget = false;  // 是否正在等待玩家选择效果目标

    [SerializeField] private ElementConfig elementConfig; // 添加这行
    [SerializeField] private EffectConfig effectConfig; 

    void Start()
    {
        matchingSystem = new MatchingSystem(gridManager);
        matchResolutionRule = new BasicMatchResolutionRule();
        InitializeMatchingRules();
        StartGame();
        InitializeInputDetector();
    }

    private void InitializeInputDetector()
    {
        inputDetector = new ClickAndDragDetector();
        
        // 注册事件处理
        inputDetector.OnClick += HandleClick;
        inputDetector.OnDragComplete += HandleDragComplete;
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

        inputDetector.Update();
        HandleKeyboardInput();
    }

    private void HandleClick(Vector3 position)
    {
        var clickedCell = GetClickedCell(position);
        if (clickedCell == null) return;

        if (isWaitingForEffectTarget)
        {
            // 如果在等待效果目标选择状态，且点击了高亮区域内的格子
            if (highlightedCells.Contains(clickedCell))
            {
                HandleEffectTargetSelection(clickedCell);
            }
            return;
        }

        // 处理普通点击逻辑（如果有的话）
    }

    private void HandleDragComplete(Vector3 start, Vector3 end)
    {
        // 如果正在等待效果目标选择，禁止拖拽
        if (isWaitingForEffectTarget) return;

        GridCell startCell = GetClickedCell(start);
        GridCell endCell = GetClickedCell(end);

        if (startCell != null && endCell != null)
        {
            if (startCell.IsMovable() && endCell.Element == null)
            {
                Element tempElement = startCell.Element;
                startCell.Element = endCell.Element;
                endCell.Element = tempElement;

                DetectMatching(endCell);
            }
        }
    }

    // 新增：取消效果选择
    private void CancelEffectTargetSelection()
    {
        ClearHighlightedCells();
        isWaitingForEffectTarget = false;
        pendingEffectSource = null;
    }

    public bool isDebugMode = false; // 标志是否处于调试模式

    void HandleKeyboardInput()
    {
        //if (Input.GetKeyDown(KeyCode.D)) // 按下 D 键进入或退出调试模式
        //{
            //isDebugMode = !isDebugMode;
            //Debug.Log($"Debug Mode: {(isDebugMode ? "ON" : "OFF")}");
        //}

        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && Input.GetMouseButtonDown(0)) // 检测 Shift 键
       //{
           //Vector3 mousePosition = Input.mousePosition;
           //GridCell clickedCell = GetClickedCell(mousePosition);
           //DetectMatching(clickedCell);
            ////TriggerDebugFunction(); // 调用特定函数
        //}

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
                // 新元素交换逻辑
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


        // 对单格执行操作
        void PerformActionOnCell(GridCell cell)
        {
            if (cell.Element != null)
            {
                Debug.Log($"Performing action on Element: {cell.Element.Type}, Level {cell.Element.Value}");
                // 使用 elementConfig 创建升级后的元素
                var upgradedElement = elementConfig.CreateElement(cell.Element.Type, cell.Element.Value + 1);
                Debug.Log($"upgradedElement: {upgradedElement.Type}, Level {upgradedElement.Value}");
                cell.Element = upgradedElement;
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
        if (filteredGroups.Count == 0) return;
        if (triggerCell.Element is ActiveSpecialElement activeElement)
        {
            ShowEffectRange(triggerCell, activeElement.ReachRange);
        }
        
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

    // 显示效果范围的方法
    public void ShowEffectRange(GridCell sourceCell, int range)
    {
        ClearHighlightedCells();
        pendingEffectSource = sourceCell;
        isWaitingForEffectTarget = true;

        var cells = GetCellsInRange(sourceCell, range);
        foreach (var cell in cells)
        {
            cell.IsHighlighted = true;  // 使用属性触发事件
            highlightedCells.Add(cell);
        }
    }

    // 清除高亮显示
    private void ClearHighlightedCells()
    {
        foreach (var cell in highlightedCells)
        {
            cell.IsHighlighted = false;  // 使用属性触发事件
        }
        highlightedCells.Clear();
    }

    // 获取指定范围内的格子
    private List<GridCell> GetCellsInRange(GridCell center, int range)
    {
        List<GridCell> cells = new List<GridCell>();
        int centerRow = center.Row;
        int centerCol = center.Column;

        for (int row = centerRow - range; row <= centerRow + range; row++)
        {
            for (int col = centerCol - range; col <= centerCol + range; col++)
            {
                if (row >= 0 && row < GridConstants.Rows && 
                    col >= 0 && col < GridConstants.Columns)
                {
                    // 计算到中心的曼哈顿距离
                    int distance = Mathf.Abs(row - centerRow) + Mathf.Abs(col - centerCol);
                    if (distance <= range)
                    {
                        //Debug.Log("GetCellsInRange: row = " + row + ", col = " + col);
                        cells.Add(gridManager.GetCell(row, col));
                    }
                }
            }
        }
        return cells;
    }

    // 处理效果目标选择
    private void HandleEffectTargetSelection(GridCell targetCell)
    {
        if (highlightedCells.Contains(targetCell))
        {
            var context = new EffectContext
            {
                GridManager = gridManager,
                SourceCell = pendingEffectSource,
                TargetCell = targetCell,
                SourceElement = pendingEffectSource.Element as ActiveSpecialElement,
            };

            var activeElement = pendingEffectSource.Element as ActiveSpecialElement;
            var effect = effectConfig.CreateEffect(activeElement.EffectID);
            if (effect != null)
            {
                EffectManager.Instance.RegisterEffect(effect);
                EffectManager.Instance.QueueEffect(activeElement.EffectID, context);
                EffectManager.Instance.ProcessEffectQueue();
            }

            ClearHighlightedCells();
            isWaitingForEffectTarget = false;
            pendingEffectSource = null;
        }
    }

}

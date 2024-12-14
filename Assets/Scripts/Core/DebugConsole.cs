using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 命令接口
public interface IConsoleCommand
{
    string Name { get; }
    string Description { get; }
    void Execute(string[] args);
}

// 命令基类
public abstract class ConsoleCommandBase : IConsoleCommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract void Execute(string[] args);
    protected DebugConsole Console { get; private set; }

    public ConsoleCommandBase(DebugConsole console)
    {
        Console = console;
    }
}

public class DebugConsole : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;
    
    [Header("Game References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameController gameController;

    private bool isConsoleActive = false;
    private Dictionary<string, IConsoleCommand> commands;
    private const int MaxLogLines = 50;
    private Queue<string> logHistory;

    #region Initialization

    private void Awake()
    {
        InitializeComponents();
        InitializeCommands();
    }

    private void InitializeComponents()
    {
        logHistory = new Queue<string>();
        commands = new Dictionary<string, IConsoleCommand>();
        
        if (consolePanel != null) consolePanel.SetActive(false);
        if (inputField != null) inputField.onEndEdit.AddListener(OnSubmitCommand);

        // 自动查找组件（如果未手动绑定）
        if (gameController == null) gameController = FindObjectOfType<GameController>();
        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();
    }

    private void InitializeCommands()
    {
        RegisterCommand(new HelpCommand(this));
        RegisterCommand(new GridInfoCommand(this));
        RegisterCommand(new SpawnCommand(this));
        RegisterCommand(new MatchCommand(this));
        RegisterCommand(new ClearCommand(this));
        RegisterCommand(new DiceCommand(this));
        RegisterCommand(new DebugModeCommand(this));
        RegisterCommand(new TestEffectCommand(this));
        RegisterCommand(new TestRangeCommand(this));
        RegisterCommand(new TestModifyCommand(this));
    }

    #endregion

    #region Console UI Management

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }
    }

    private void ToggleConsole()
    {
        isConsoleActive = !isConsoleActive;
        if (consolePanel != null)
        {
            consolePanel.SetActive(isConsoleActive);
            if (isConsoleActive && inputField != null)
            {
                inputField.ActivateInputField();
            }
        }
    }

    #endregion

    #region Command Management

    private void RegisterCommand(IConsoleCommand command)
    {
        commands[command.Name.ToLower()] = command;
    }

    private void OnSubmitCommand(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        string[] parts = input.Trim().Split(' ');
        string commandName = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        ExecuteCommand(commandName, args);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void ExecuteCommand(string commandName, string[] args)
    {
        try
        {
            if (commands.TryGetValue(commandName, out var command))
            {
                command.Execute(args);
            }
            else
            {
                LogOutput($"未知命令: {commandName}");
            }
        }
        catch (System.Exception ex)
        {
            LogError($"执行命令 '{commandName}' 时发生错误: {ex.Message}");
        }
    }

    #endregion

    #region Logging

    public void LogOutput(string message)
    {
        logHistory.Enqueue(message);
        while (logHistory.Count > MaxLogLines)
        {
            logHistory.Dequeue();
        }

        outputText.text = string.Join("\n", logHistory);
    }

    public void LogError(string message)
    {
        LogOutput($"<color=red>错误: {message}</color>");
    }

    public void LogWarning(string message)
    {
        LogOutput($"<color=yellow>警告: {message}</color>");
    }

    public void LogSuccess(string message)
    {
        LogOutput($"<color=green>{message}</color>");
    }

    #endregion

    #region Public Accessors

    public GridManager GetGridManager() => gridManager;
    public GameController GetGameController() => gameController;

    #endregion
}

#region Command Implementations

// Help命令
public class HelpCommand : ConsoleCommandBase
{
    public override string Name => "help";
    public override string Description => "显示所有可用命令";

    public HelpCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        Console.LogOutput("可用命令:");
        Console.LogOutput("help - 显示此帮助信息");
        Console.LogOutput("gridinfo - 显示网格信息");
        Console.LogOutput("spawn [数量] - 生成随机元素");
        Console.LogOutput("match - 手动触发匹配检测");
        Console.LogOutput("clear - 清空控制台");
        Console.LogOutput("debug - 切换调试模式");
        Console.LogOutput("dice相关命令:");
        Console.LogOutput("  rolldice - 投掷所有骰子");
        Console.LogOutput("  cleardice - 清空所有骰子");
        Console.LogOutput("  adddice <类型> [等级] - 添加测试骰子");
        Console.LogOutput("  listdice - 列出所有骰子");
    }
}

// 网格信息命令
public class GridInfoCommand : ConsoleCommandBase
{
    public override string Name => "gridinfo";
    public override string Description => "显示网格信息";

    public GridInfoCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager == null)
        {
            Console.LogError("GridManager未找到");
            return;
        }

        Console.LogOutput("网格信息:");
        for (int row = 0; row < GridConstants.Rows; row++)
        {
            for (int col = 0; col < GridConstants.Columns; col++)
            {
                var cell = gridManager.GetCell(row, col);
                if (cell.Element != null)
                {
                    Console.LogOutput($"格子[{row},{col}]: {cell.Element.Type}, 值:{cell.Element.Value}");
                }
                else
                {
                    Console.LogOutput($"格子[{row},{col}]: 空");
                }
            }
        }
    }
}

// 生成元素命令
public class SpawnCommand : ConsoleCommandBase
{
    public override string Name => "spawn";
    public override string Description => "生成随机元素";

    public SpawnCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager == null)
        {
            Console.LogError("GridManager未找到");
            return;
        }

        int count = args.Length > 0 && int.TryParse(args[0], out var result) ? result : 5;
        gridManager.gridData.RandomSpawn(count);
        Console.LogSuccess($"已生成 {count} 个随机元素");
    }
}

// 匹配检测命令
public class MatchCommand : ConsoleCommandBase
{
    public override string Name => "match";
    public override string Description => "手动触发匹配检测";

    public MatchCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        var gridManager = Console.GetGridManager();
        var gameController = Console.GetGameController();
        
        if (gridManager == null || gameController == null)
        {
            Console.LogError("GridManager或GameController未找到");
            return;
        }

        for (int row = 0; row < GridConstants.Rows; row++)
        {
            for (int col = 0; col < GridConstants.Columns; col++)
            {
                var cell = gridManager.GetCell(row, col);
                if (cell.Element != null)
                {
                    Console.LogOutput($"检测格子[{row},{col}]的匹配");
                    gameController.DetectMatching(cell);
                }
            }
        }
    }
}

// 清空日志命令
public class ClearCommand : ConsoleCommandBase
{
    public override string Name => "clear";
    public override string Description => "清空控制台输出";

    public ClearCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        Console.LogOutput("");
    }
}

// 骰子相关命令
public class DiceCommand : ConsoleCommandBase
{
    public override string Name => "dice";
    public override string Description => "骰子相关操作";

    public DiceCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            ShowDiceHelp();
            return;
        }

        var subCommand = args[0].ToLower();
        var subArgs = args.Length > 1 ? args[1..] : new string[0];

        switch (subCommand)
        {
            case "roll": RollDice(subArgs); break;
            case "clear": ClearDice(subArgs); break;
            case "add": AddDice(subArgs); break;
            case "list": ListDice(subArgs); break;
            default:
                Console.LogError($"未知的骰子命令: {subCommand}");
                ShowDiceHelp();
                break;
        }
    }

    private void ShowDiceHelp()
    {
        Console.LogOutput("骰子命令用法:");
        Console.LogOutput("dice roll - 投掷所有骰子");
        Console.LogOutput("dice clear - 清空所有骰子");
        Console.LogOutput("dice add <类型> [等级] - 添加测试骰子");
        Console.LogOutput("dice list - 列出所有骰子");
    }

    private void RollDice(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager?.diceManager == null)
        {
            Console.LogError("DiceManager未找到");
            return;
        }

        var results = gridManager.diceManager.RollAllDice();
        foreach (var element in results)
        {
            if (element != null)
            {
                Console.LogOutput($"投掷结果: {element.Type} (值:{element.Value})");
            }
        }
        gridManager.SpawnDiceGeneratedElements();
    }

    private void ClearDice(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager?.diceManager == null)
        {
            Console.LogError("DiceManager未找到");
            return;
        }

        gridManager.diceManager = new DiceManager();
        Console.LogSuccess("已清空所有骰子");
    }

    private void AddDice(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager?.diceManager == null)
        {
            Console.LogError("DiceManager未找到");
            return;
        }

        string diceType = args.Length > 0 ? args[0] : "Fire";
        int level = args.Length > 1 && int.TryParse(args[1], out var l) ? l : 1;

        // 创建测试骰子面
        DiceFace[] faces = new DiceFace[6];
        for (int i = 0; i < 6; i++)
        {
            faces[i] = new DiceFace(new Element(diceType, level));
        }

        Dictionary<string, int> supportedElements = new Dictionary<string, int>
        {
            { diceType, 70 },
            { "Normal", 30 }
        };

        var dice = new Dice(diceType, level, faces, supportedElements, 3);
        gridManager.diceManager.AddDice(dice);
        
        Console.LogSuccess($"已添加 {diceType} 骰子 (等级 {level})");
    }

    private void ListDice(string[] args)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager?.diceManager == null)
        {
            Console.LogError("DiceManager未找到");
            return;
        }

        var diceInfo = gridManager.diceManager.GetDiceInfo();
        if (diceInfo.Count == 0)
        {
            Console.LogWarning("当前没有骰子");
            return;
        }

        Console.LogOutput("当前骰子列表:");
        foreach (var info in diceInfo)
        {
            Console.LogOutput(info);
        }
    }
}

// 调试模式命令
public class DebugModeCommand : ConsoleCommandBase
{
    public override string Name => "debug";
    public override string Description => "切换调试模式";

    public DebugModeCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        var gameController = Console.GetGameController();
        if (gameController == null)
        {
            Console.LogError("GameController未找到");
            return;
        }

        gameController.isDebugMode = !gameController.isDebugMode;
        Console.LogSuccess($"调试模式: {(gameController.isDebugMode ? "开启" : "关闭")}");
    }
}

// 测试效果命令
public class TestEffectCommand : ConsoleCommandBase
{
    public override string Name => "test_effect";
    public override string Description => "测试特定效果";

    public TestEffectCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.LogError("请指定要测试的效果类型");
            return;
        }

        switch (args[0].ToLower())
        {
            case "fireball":
                TestFireball();
                break;
            default:
                Console.LogError($"未知的效果类型: {args[0]}");
                break;
        }
    }

    private void TestFireball()
    {
        var gridManager = Console.GetGridManager();
        if (gridManager == null)
        {
            Console.LogError("GridManager未找到");
            return;
        }

        var layout = new (int, int, string, int)[]
        {
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
            gridManager.gridData.SetCellElement(row, col, new Element(type, value));
        }

        gridManager.gridData.GetCell(0, 0).Element = new ActiveSpecialElement(
            "Fireball", 
            1,
            1,
            "effect_fireball",
            1
        );

        Console.LogSuccess("火球测试布局已创建");
    }
}

// 测试范围命令
public class TestRangeCommand : ConsoleCommandBase
{
    public override string Name => "test_range";
    public override string Description => "测试范围效果";

    public TestRangeCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            ShowHelp();
            return;
        }

        if (!System.Enum.TryParse(args[0], true, out RangeShape shape))
        {
            Console.LogError("无效的形状参数");
            return;
        }

        if (!int.TryParse(args[1], out int range))
        {
            Console.LogError("无效的范围参数");
            return;
        }

        ExecuteRangeTest(shape, range);
    }

    private void ShowHelp()
    {
        Console.LogOutput("用法: test_range <形状> <范围>");
        Console.LogOutput("形状: point, line, square, diamond, circle, global");
        Console.LogOutput("范围: 数字 (1-5)");
    }

    private void ExecuteRangeTest(RangeShape shape, int range)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager == null)
        {
            Console.LogError("GridManager未找到");
            return;
        }

        var effectConfig = new EffectConfig.EffectData
        {
            ID = "test_range",
            Name = "测试范围效果",
            Type = EffectType.ElementChange,
            CustomParameters = new Dictionary<string, object>
            {
                { "shape", shape },
                { "range", range }
            }
        };

        var effect = new RangeEliminateEffect(effectConfig);
        var centerPos = new Vector2Int(GridConstants.Rows/2, GridConstants.Columns/2);
        var centerCell = gridManager.GetCell(centerPos.x, centerPos.y);

        var context = new EffectContext
        {
            GridManager = gridManager,
            SourceCell = centerCell
        };

        effect.Execute(context);
        Console.LogSuccess($"在中心位置 ({centerPos.x}, {centerPos.y}) 执行了{shape}形状、范围为{range}的效果");
    }
}

// 测试元素修改命令
public class TestModifyCommand : ConsoleCommandBase
{
    public override string Name => "test_modify";
    public override string Description => "测试元素修改";

    public TestModifyCommand(DebugConsole console) : base(console) { }

    public override void Execute(string[] args)
    {
        if (args.Length < 3)
        {
            ShowHelp();
            return;
        }

        var effectConfig = CreateEffectConfig(args);
        if (effectConfig != null)
        {
            ExecuteModifyTest(effectConfig);
        }
    }

    private void ShowHelp()
    {
        Console.LogOutput("用法: test_modify <修改类型> <范围> <参数>");
        Console.LogOutput("修改类型: type, level, position");
        Console.LogOutput("范围: 数字 (1-5)");
        Console.LogOutput("参数:");
        Console.LogOutput("  type模式: 目标元素类型");
        Console.LogOutput("  level模式: 等级修改值");
        Console.LogOutput("  position模式: x偏移,y偏移");
    }

    private EffectConfig.EffectData CreateEffectConfig(string[] args)
    {
        var config = new EffectConfig.EffectData
        {
            ID = "test_modify",
            Name = "测试元素修改",
            Type = EffectType.ElementChange,
            CustomParameters = new Dictionary<string, object>
            {
                { "shape", RangeShape.Square },
                { "range", int.Parse(args[1]) }
            }
        };

        switch (args[0].ToLower())
        {
            case "type":
                config.CustomParameters["modifyType"] = ElementModifyEffect.ModifyType.ChangeType;
                config.CustomParameters["targetElementType"] = args[2];
                break;
            //case "level":
                //config.CustomParameters["modifyType"] = ElementModifyEffect.ModifyType.ChangeLevel;
                //config.CustomParameters["levelModifier"] = int.Parse(args[2]);
                //break;
            case "position":
                config.CustomParameters["modifyType"] = ElementModifyEffect.ModifyType.ChangePosition;
                var offsets = args[2].Split(',');
                config.CustomParameters["offsetX"] = int.Parse(offsets[0]);
                config.CustomParameters["offsetY"] = int.Parse(offsets[1]);
                break;
            default:
                Console.LogError($"未知的修改类型: {args[0]}");
                return null;
        }

        return config;
    }

    private void ExecuteModifyTest(EffectConfig.EffectData config)
    {
        var gridManager = Console.GetGridManager();
        if (gridManager == null)
        {
            Console.LogError("GridManager未找到");
            return;
        }

        var effect = new ElementModifyEffect(config);
        var context = new EffectContext
        {
            GridManager = gridManager,
            SourceCell = gridManager.GetCell(GridConstants.Rows/2, GridConstants.Columns/2)
        };

        effect.Execute(context);
        Console.LogSuccess("元素修改效果已执行");
    }
}

#endregion

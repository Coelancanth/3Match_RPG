using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private GameObject consolePanel; // 控制台面板
    [SerializeField] private TMP_InputField inputField;   // 命令输入框
    [SerializeField] private TMP_Text outputText;         // 输出日志
    [SerializeField] private GridManager gridManager; // 手动绑定 GridManager
    [SerializeField] private GameController gameController; // 手动绑定 GameController

    private bool isConsoleActive = false;
    private Dictionary<string, System.Action<string[]>> commands; // 命令与对应操作
    private const int MaxLogLines = 50; // 日志最大行数

    void Start()
    {
        // 初始化控制台
        if (consolePanel != null) consolePanel.SetActive(false);
        
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnSubmitCommand);
        }

        commands = new Dictionary<string, System.Action<string[]>>();

        // 注册命令
        RegisterCommand("help", ShowHelp);
        RegisterCommand("gridinfo", DebugGridInfo);
        RegisterCommand("spawn", SpawnElements);
        RegisterCommand("match", TriggerManualMatching);
        RegisterCommand("clear", ClearLog);
        RegisterCommand("rolldice", (args) => {
            var results = gridManager.diceManager.RollAllDice();
            foreach (var element in results)
            {
                if (element != null)
                {
                    LogOutput($"Rolled: {element.Type} (Level {element.Level})" + 
                        (element.SkillID != null ? $", Skill: {element.SkillID}" : ""));
                }
            }
            gridManager.SpawnDiceGeneratedElements();
        });
        RegisterCommand("cleardice", (args) => {
            gridManager.diceManager = new DiceManager();
            LogOutput("Cleared all dice.");
        });
        RegisterCommand("adddice", AddTestDice);
        RegisterCommand("listdice", ListAllDice);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // 按下 "~" 键切换控制台
        {
            ToggleConsole();
        }
    }

    private void ToggleConsole()
    {
        isConsoleActive = !isConsoleActive;
        if (consolePanel != null) consolePanel.SetActive(isConsoleActive);
        if (isConsoleActive && inputField != null)
        {
            inputField.ActivateInputField(); // 激活输入框
        }
    }

    public void OnSubmitCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        // 分割命令和参数
        string[] parts = command.Trim().Split(' ');
        string cmd = parts[0].ToLower();
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        if (commands.TryGetValue(cmd, out var action))
        {
            try
            {
                action.Invoke(args); // 执行命令
            }
            catch (System.Exception ex)
            {
                LogOutput($"Error executing command '{cmd}': {ex.Message}");
            }
        }
        else
        {
            LogOutput($"Unknown command: {cmd}");
        }

        inputField.text = ""; // 清空输入框
        inputField.ActivateInputField(); // 重新激活输入框
    }

    private void RegisterCommand(string command, System.Action<string[]> action)
    {
        commands[command] = action;
    }

    private void LogOutput(string message)
    {
        outputText.text += message + "\n";

        // 限制日志行数
        var lines = outputText.text.Split('\n');
        if (lines.Length > MaxLogLines)
        {
            outputText.text = string.Join("\n", lines[^MaxLogLines..]);
        }
        
        //// 强制 Scroll View 滚动到底部
        //Canvas.ForceUpdateCanvases();
        //var scrollRect = outputText.GetComponentInParent<ScrollRect>();
        //if (scrollRect != null)
        //{
            //scrollRect.verticalNormalizedPosition = 0f; // 滚动到底部
        //}
    }

    private void ClearLog(string[] args)
    {
        outputText.text = "";
    }

    // 命令：显示帮助
    private void ShowHelp(string[] args)
    {
        LogOutput("Available commands:");
        LogOutput("help - Show this help message");
        LogOutput("gridinfo - Display grid information");
        LogOutput("spawn [count] - Spawn random elements");
        LogOutput("match - Manually trigger matching");
        LogOutput("clear - Clear the log output");
    }

    // 命令：输出网格信息
    private void DebugGridInfo(string[] args)
    {
        if (gridManager == null)
        {
            LogOutput("GridManager not assigned in the Inspector.");
            return;
        }

        LogOutput("Grid Information:");
        for (int row = 0; row < GridConstants.Rows; row++)
        {
            for (int col = 0; col < GridConstants.Columns; col++)
            {
                var cell = gridManager.GetCell(row, col);
                if (cell.Element != null)
                {
                    LogOutput($"Cell[{row},{col}]: {cell.Element.Type}, Level {cell.Element.Level}");
                }
                else
                {
                    LogOutput($"Cell[{row},{col}]: Empty");
                }
            }
        }
    }

    // 命令：生成元素
    private void SpawnElements(string[] args)
    {
        if (gridManager == null)
        {
            LogOutput("GridManager not assigned in the Inspector.");
            return;
        }

        int count = args.Length > 0 && int.TryParse(args[0], out var result) ? result : 5;
        gridManager.gridData.RandomSpawn(count);
        LogOutput($"Spawned {count} random elements.");
    }

    // 命令：触发匹配
    private void TriggerManualMatching(string[] args)
    {
        if (gridManager == null || gameController == null)
        {
            LogOutput("GridManager or GameController not assigned in the Inspector.");
            return;
        }

        for (int row = 0; row < GridConstants.Rows; row++)
        {
            for (int col = 0; col < GridConstants.Columns; col++)
            {
                var cell = gridManager.GetCell(row, col);
                if (cell.Element != null)
                {
                    LogOutput($"Triggering matching at Cell[{row},{col}]");
                    gameController.DetectMatching(cell);
                }
            }
        }
    }

    // 添加测试骰子
    private void AddTestDice(string[] args)
    {
        if (gridManager?.diceManager == null)
        {
            LogOutput("DiceManager not found.");
            return;
        }

        string diceType = args.Length > 0 ? args[0] : "Fire";
        int level = args.Length > 1 && int.TryParse(args[1], out var l) ? l : 1;

        // 创建测试用的骰子面
        DiceFace[] faces = new DiceFace[6];
        for (int i = 0; i < 6; i++)
        {
            faces[i] = new DiceFace(new Element(diceType, level, i % 2 == 0 ? $"{diceType.ToLower()}_skill_{i}" : null));
        }

        // 设置元素权重
        Dictionary<string, int> supportedElements = new Dictionary<string, int>
        {
            { diceType, 70 },
            { "Normal", 30 }
        };

        // 创建并添加骰子
        var dice = new Dice(diceType, level, faces, supportedElements, 3);
        gridManager.diceManager.AddDice(dice);
        
        LogOutput($"Added {diceType} dice (Level {level})");
    }

    // 列出所有骰子信息
    private void ListAllDice(string[] args)
    {
        if (gridManager?.diceManager == null)
        {
            LogOutput("DiceManager not found.");
            return;
        }

        var diceInfo = gridManager.diceManager.GetDiceInfo();
        if (diceInfo.Count == 0)
        {
            LogOutput("No dice available.");
            return;
        }

        LogOutput("Current Dice:");
        foreach (var info in diceInfo)
        {
            LogOutput(info);
        }
    }
}

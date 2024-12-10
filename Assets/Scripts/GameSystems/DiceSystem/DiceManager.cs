
/// <summary>
/// Manages all the dice owned by the player and handles dice rolling logic.
/// </summary>
/// <remarks>
/// 1. This class is responsible for:
///    - Managing a collection of dice.
///    - Rolling all dice to generate elements or trigger effects.
/// 2. Usage:
///    - Integrated with GridManager to populate the grid with elements each turn.
///    - Acts as a utility class for managing player dice.
/// 3. Future Extensions:
///    - Support for adding or removing dice dynamically during gameplay.
///    - Introduce dice upgrades and rarity systems.
/// </remarks>

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManager
{
    private List<Dice> diceCollection; // 当前玩家持有的骰子

    public DiceManager()
    {
        diceCollection = new List<Dice>();
    }

    // 添加新骰子
    public void AddDice(Dice dice)
    {
        diceCollection.Add(dice);
    }

    // 掷所有骰子，根据骰子的类型和等级生成元素
    public List<Element> RollAllDice()
    {
        List<Element> results = new List<Element>();
        foreach (var dice in diceCollection)
        {
            var face = dice.RollFace();

            results = results.Concat(dice.RollElement(dice.SpawnNumber)).ToList();
            Debug.Log($"骰子结果 - 面: {face}, 生成元素数量: {dice.SpawnNumber}");
        }
        Debug.Log($"总共生成元素数量: {results.Count}");
        return results;
    }

    // 获取所有骰子信息
    public List<string> GetDiceInfo()
    {
        List<string> info = new List<string>();
        foreach (var dice in diceCollection)
        {
            info.Add($"Type: {dice.Type}, Level: {dice.Level}");
        }
        return info;
    }
}

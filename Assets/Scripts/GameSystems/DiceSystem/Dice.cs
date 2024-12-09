/// <summary>
/// Represents a dice in the game that generates elements based on its type and level.
/// </summary>
/// <remarks>
/// 1. This class is responsible for:
///    - Defining a dice with a specific type (e.g., "Fire") and level.
///    - Rolling to produce elements or trigger specific effects based on its faces.
/// 2. Usage:
///    - Managed by the DiceManager to handle dice rolls in a turn.
/// 3. Future Extensions:
///    - Add special effects for specific dice faces.
///    - Allow dynamic face configurations for more complex dice.
/// </remarks>

public class Dice
{
    public string Type { get; private set; } // 骰子的类型，例如 "Fire", "Water"
    public int Level { get; private set; }   // 骰子的等级，例如 Level 1, 2, etc.
    private string[] faces;                  // 骰子的6个面

    public Dice(string type, int level, string[] faces)
    {
        if (faces.Length != 6)
        {
            throw new System.ArgumentException("A dice must have exactly 6 faces.");
        }
        
        Type = type;
        Level = level;
        this.faces = faces;
    }

    // 投掷骰子，返回随机的面
    public string RollFace()
    {
        return faces[UnityEngine.Random.Range(0, faces.Length)];
    }

    // 投掷骰子，生成与骰子等级和类型一致的 Element
    public Element RollElement()
    {
        return new Element(Type, Level);
    }

    // 升级骰子
    public void Upgrade()
    {
        Level += 1;
    }
}

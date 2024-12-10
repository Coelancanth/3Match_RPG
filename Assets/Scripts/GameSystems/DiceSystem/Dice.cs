using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a dice in the game that generates elements based on its type, level, and faces.
/// Each face is an individual Element, potentially linked to a skill.
/// </summary>
public class Dice
{
    public string Type { get; private set; } // Dice type (e.g., "Fire", "Water")
    public int Level { get; private set; }   // Dice level
    public DiceFace[] Faces { get; private set; } // Dice's 6 faces
    public Dictionary<string, int> SupportedElements { get; private set; } // Supported elements by type and weight (probability)
    public int SpawnNumber { get; private set; } // 骰子每次掷出时生成的元素数量

    /// <summary>
    /// Constructor for initializing the Dice object.
    /// </summary>
    /// <param name="type">Type of the dice (e.g., "Fire")</param>
    /// <param name="level">Level of the dice</param>
    /// <param name="faces">An array of Elements representing the faces of the dice</param>
    /// <param name="supportedElements">A dictionary of supported element types and their probabilities (weight)</param>
    public Dice(string type, int level, DiceFace[] faces, Dictionary<string, int> supportedElements, int spawnNumber)
    {

        Type = type;
        Level = level;
        Faces = faces;
        SupportedElements = supportedElements ?? new Dictionary<string, int>();
        SpawnNumber = spawnNumber;
    }

    /// <summary>
    /// Roll the dice to get a random face.
    /// </summary>
    /// <returns>The rolled Element (Face).</returns>
     public Element RollFace()
    {
        int randomIndex = UnityEngine.Random.Range(0, Faces.Length);
        Element rolledElement = Faces[randomIndex].Element;
        Debug.Log($"骰子投掷结果 - 类型: {Type}, 等级: {Level}, 面: {randomIndex + 1}, 元素: {rolledElement.Type}, 技能: {rolledElement.SkillID ?? "无"}");
        return rolledElement;
    }

public Element[] RollElement(int spawnNumber)
{
    Element[] elements = new Element[spawnNumber];
    
    for(int i = 0; i < spawnNumber; i++)
    {
        // Calculate the total weight
        int totalWeight = 0;
        foreach (var weight in SupportedElements.Values)
        {
            totalWeight += weight;
        }
        
        // Generate a random number
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        //Debug.Log($"骰子生成元素 - 总权重: {totalWeight}, 随机值: {randomValue}");

        // Find the element based on probability
        int cumulativeWeight = 0;
        foreach (var elementType in SupportedElements.Keys)
        {
            cumulativeWeight += SupportedElements[elementType];
            //Debug.Log($"检查元素类型: {elementType}, 累计权重: {cumulativeWeight}");
            
            if (randomValue < cumulativeWeight)
            {
                elements[i] = new Element(elementType, Level);
                Debug.Log($"生成元素 #{i + 1}: 类型 = {elementType}, 等级 = {Level}");
                break;
            }
        }

        // Default case
        if(elements[i] == null)
        {
            elements[i] = new Element(Type, Level);
            Debug.Log($"生成默认元素 #{i + 1}: 类型 = {Type}, 等级 = {Level}");
        }
    }

    return elements;
}


    /// <summary>
    /// Adds or updates the supported element count (weight) for a given element type.
    /// </summary>
    /// <param name="elementType">The type of the element to update</param>
    /// <param name="weight">The weight (probability) for this element type</param>
    public void AddOrUpdateSupportedElement(string elementType, int weight)
    {
        if (SupportedElements.ContainsKey(elementType))
        {
            SupportedElements[elementType] = weight;
        }
        else
        {
            SupportedElements.Add(elementType, weight);
        }
    }

    ///// <summary>
    ///// Creates a fire dice with the specified level.
    ///// </summary>
    ///// <param name="level">The level of the fire dice</param>
    ///// <returns>A new fire dice object</returns>
    //public static Dice CreateFireDice(int level = 1)
    //{
        //// 创建骰子的6个面
        //Element[] faces = new Element[]
        //{
            //new Element("Fire", level),                    // 面1：普通火元素
            //new Element("Fire", level),                    // 面2：普通火元素
            //new Element("Fire", level + 1),                // 面3：高级火元素
            //new Element("Fire", level, "fire_arrow"),      // 面4：火箭技能
            //new Element("Fire", level),                    // 面5：普通火元素
            //new Element("Fire", level, "fire_explosion")   // 面6：爆炸技能
        //};

        //// 设置支持的元素类型及其权重
        //Dictionary<string, int> supportedElements = new Dictionary<string, int>
        //{
            //{ "Fire", 70 },      // 火元素70%的权重
            //{ "Normal", 30 }     // 普通元素30%的权重
        //};

        //return new Dice("Fire", level, faces, supportedElements);
    //}

    /// <summary>
    /// 增强指定的骰子面
    /// </summary>
    /// <param name="faceIndex">要增强的面的索引（0-5）</param>
    /// <param name="enhanceType">增强类型</param>
    /// <returns>增强是否成功</returns>
    public bool EnhanceFace(int faceIndex, EnhanceType enhanceType)
    {
        if (faceIndex < 0 || faceIndex >= Faces.Length)
            return false;

        var face = Faces[faceIndex];
        
        switch (enhanceType)
        {
            case EnhanceType.LevelUp:
                Faces[faceIndex] = new DiceFace(
                    new Element(
                        face.Element.Type,
                        face.Element.Level + 1,
                        face.Element.SkillID
                    )
                );
                break;

            case EnhanceType.AddSkill:
                string newSkillID = GetRandomSkillForType(face.Element.Type);
                Faces[faceIndex] = new DiceFace(
                    new Element(
                        face.Element.Type,
                        face.Element.Level,
                        newSkillID
                    )
                );
                break;

            case EnhanceType.Both:
                newSkillID = GetRandomSkillForType(face.Element.Type);
                Faces[faceIndex] = new DiceFace(
                    new Element(
                        face.Element.Type,
                        face.Element.Level + 1,
                        newSkillID
                    )
                );
                break;
        }

        return true;
    }

    // 增强类型枚举
    public enum EnhanceType
    {
        LevelUp,    // 仅提升等级
        AddSkill,   // 仅添加/更改技能
        Both        // 同时提升等级和技能
    }

    private string GetRandomSkillForType(string elementType)
{
    switch (elementType)
    {
        case "Fire":
            string[] fireSkills = { "fire_arrow", "fire_explosion", "fire_wall" };
            return fireSkills[UnityEngine.Random.Range(0, fireSkills.Length)];
        default:
            return null;
    }
}
}

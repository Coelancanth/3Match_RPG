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
    private Element[] faces;                 // Dice's 6 faces
    public Dictionary<string, int> SupportedElements { get; private set; } // Supported elements by type and weight (probability)

    /// <summary>
    /// Constructor for initializing the Dice object.
    /// </summary>
    /// <param name="type">Type of the dice (e.g., "Fire")</param>
    /// <param name="level">Level of the dice</param>
    /// <param name="faces">An array of Elements representing the faces of the dice</param>
    /// <param name="supportedElements">A dictionary of supported element types and their probabilities (weight)</param>
    public Dice(string type, int level, Element[] faces, Dictionary<string, int> supportedElements)
    {
        if (faces.Length != 6)
        {
            throw new ArgumentException("A dice must have exactly 6 faces.");
        }

        Type = type;
        Level = level;
        this.faces = faces;
        SupportedElements = supportedElements ?? new Dictionary<string, int>();
    }

    /// <summary>
    /// Roll the dice to get a random face.
    /// </summary>
    /// <returns>The rolled Element (Face).</returns>
    public Element RollFace()
    {
        return faces[UnityEngine.Random.Range(0, faces.Length)];
    }

    /// <summary>
    /// Generate an element based on the dice's type, level, and supported elements, with probability-based selection.
    /// </summary>
    /// <returns>A new Element object based on dice type and level.</returns>
    public Element RollElement()
    {
        // Calculate the total weight (sum of all probabilities)
        int totalWeight = 0;
        foreach (var weight in SupportedElements.Values)
        {
            totalWeight += weight;
        }

        // Generate a random number between 0 and the total weight
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        // Find the element based on the probability (weight)
        int cumulativeWeight = 0;
        foreach (var elementType in SupportedElements.Keys)
        {
            cumulativeWeight += SupportedElements[elementType];
            if (randomValue < cumulativeWeight)
            {
                // Generate the element with the selected type and level
                return new Element(elementType, Level);
            }
        }

        // Default case: Return a random element (should not hit this in theory)
        return new Element(Type, Level);
    }

    /// <summary>
    /// Upgrade the dice, increasing its level and optionally enhancing its faces.
    /// </summary>
    public void Upgrade()
    {
        Level += 1;

        // Optionally enhance faces
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = new Element(faces[i].Type, faces[i].Level + 1, faces[i].SkillID);
        }
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
}

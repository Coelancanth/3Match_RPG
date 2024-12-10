using UnityEngine;
public class DiceFace
{
    public Element Element { get; private set; }
    public bool IsFrozen { get; set; }
    
    public DiceFace(Element element)
    {
        Element = element;
        IsFrozen = false;
    }
    
    public DiceFace(string type, int level, string skillID)
    {
        Element = new Element(type, level, skillID);
        IsFrozen = false;
    }
} 
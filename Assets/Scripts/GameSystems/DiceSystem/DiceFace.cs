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
    
    public static DiceFace CreateSpecialFace(string type, int value, int level, string skillID)
    {
        return new DiceFace(new ActiveSpecialElement(type, value, level, skillID));
    }
    
    public static DiceFace CreatePassiveFace(string type, int value, int level, PassiveEffect effect)
    {
        return new DiceFace(new PassiveSpecialElement(type, value, level, effect));
    }
} 
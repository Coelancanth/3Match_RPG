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
    
    
} 
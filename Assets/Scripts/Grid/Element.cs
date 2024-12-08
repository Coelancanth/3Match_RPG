public class Element
{
    public string Type { get; private set; }
    public int Level { get; private set; }

    public Element(string type, int level)
    {
        Type = type;
        Level = level;
    }

    public void Upgrade()
    {
        Level +=1;
    }
}

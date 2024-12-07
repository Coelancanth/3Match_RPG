public class GridCell
{
    public int Row { get; private set; }  // 行坐标
    public int Column { get; private set; }  // 列坐标

    // 元素信息
    public string ElementType { get; set; }  // 元素类型（如"Water", "Fire"）
    public int ElementLevel { get; set; }  // 元素等级
    public string ElementState { get; set; }  // 元素状态（如"Active", "Inactive"）

    // 敌人信息
    public string EnemyType { get; set; }  // 敌人类型
    public int EnemyHealth { get; set; }  // 敌人生命值

    // 地形属性
    public string TerrainType { get; set; }  // 地形类型（如"Grass", "Rock", "Water"）

    public GridCell(int row, int column)
    {
        Row = row;
        Column = column;
        ElementType = "None";
        ElementLevel = 0;
        ElementState = "Empty";
        EnemyType = "None";
        EnemyHealth = 0;
        TerrainType = "Plain";
    }
}

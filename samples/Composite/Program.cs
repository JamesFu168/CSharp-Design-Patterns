// Composite：讓「單一物件」和「一群物件」實作同一個介面，呼叫端不必分辨自己拿到的是哪一種。

var picture = new Group("Picture",
    new Shape("Red Line"),
    new Shape("Blue Circle"),
    new Group("Two Circles",
        new Shape("Black Circle"),
        new Shape("White Circle")),
    new Shape("Green Box"));

// 對整棵樹和對單一葉節點，使用方式完全相同
picture.Draw(depth: 0);
Console.WriteLine($"Total shapes: {picture.ShapeCount}");

IGraphic single = new Shape("Yellow Line");
single.Draw(depth: 0);
Console.WriteLine($"Total shapes: {single.ShapeCount}");

// ---- Component：葉節點與組合節點共同的介面 ----
public interface IGraphic
{
    string Name { get; }
    int ShapeCount { get; }
    void Draw(int depth);
}

// ---- Leaf：沒有子節點 ----
public sealed class Shape(string name) : IGraphic
{
    public string Name => name;
    public int ShapeCount => 1;

    public void Draw(int depth) => Console.WriteLine($"{new string(' ', depth * 2)}- {Name}");
}

// ---- Composite：持有子節點，並把操作轉交給每一個子節點 ----
public sealed class Group(string name, params IGraphic[] children) : IGraphic
{
    private readonly List<IGraphic> _children = [.. children];

    public string Name => name;

    // 不必知道子節點是 Shape 還是另一個 Group，全部加總即可
    public int ShapeCount => _children.Sum(child => child.ShapeCount);

    public void Add(IGraphic child) => _children.Add(child);
    public void Remove(IGraphic child) => _children.Remove(child);

    public void Draw(int depth)
    {
        Console.WriteLine($"{new string(' ', depth * 2)}+ {Name}");
        foreach (IGraphic child in _children)
        {
            child.Draw(depth + 1);
        }
    }
}

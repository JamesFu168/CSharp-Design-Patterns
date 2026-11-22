// Prototype：先準備好「樣板物件」，需要新物件時複製樣板再微調，而不是從頭建立。

// 原型註冊表：把常用的樣板集中管理
var palette = new ColorPalette();
palette.Register("red", new Color(255, 0, 0));
palette.Register("green", new Color(0, 255, 0));

// record 的 with 運算式 = 複製一份，同時改掉指定的屬性
Color red = palette.Clone("red");
Color pink = palette.Clone("red") with { Green = 192, Blue = 203 };
Console.WriteLine($"red  = {red}");
Console.WriteLine($"pink = {pink}");

Color anotherRed = palette.Clone("red");
Console.WriteLine($"Same value: {red == anotherRed}, same instance: {ReferenceEquals(red, anotherRed)}");

// 陷阱：with 是「淺複製」。成員若是可變的參考型別（例如 List），複製品和本尊會共用同一個物件
var sunset = new Gradient("sunset", [new Color(255, 94, 77), new Color(255, 195, 113)]);
Gradient shallow = sunset with { Name = "shallow copy" };
shallow.Stops.Add(new Color(0, 0, 0));
Console.WriteLine($"Added a stop to the shallow copy -> the original now has {sunset.Stops.Count} stops (it started with 2)");

// 解法：需要完全獨立的複製品時，自己提供深複製
Gradient deep = sunset.DeepCopy() with { Name = "deep copy" };
deep.Stops.Clear();
Console.WriteLine($"Cleared the deep copy -> the original still has {sunset.Stops.Count} stops");

// ---- Prototype：record 由編譯器自動產生複製用的建構函式 ----
public sealed record Color(int Red, int Green, int Blue);

public sealed record Gradient(string Name, List<Color> Stops)
{
    // Color 本身不可變，所以只需要複製清單，不必再複製每一個 Color
    public Gradient DeepCopy() => this with { Stops = [.. Stops] };
}

// ---- Prototype Registry ----
public sealed class ColorPalette
{
    private readonly Dictionary<string, Color> _prototypes = new();

    public void Register(string name, Color prototype) => _prototypes[name] = prototype;

    // with { } 不改任何屬性，等於「原樣複製一份」
    public Color Clone(string name) => _prototypes[name] with { };
}

// Flyweight：大量物件有相同的內容時，共用同一個實例；會變動的部分由呼叫端傳入。

const string document = "AAZZBBZB";
var factory = new GlyphFactory();

int pointSize = 10;
foreach (char symbol in document)
{
    Glyph glyph = factory.Get(symbol);
    glyph.Draw(++pointSize);
}

Console.WriteLine($"Characters drawn: {document.Length}, glyph objects created: {factory.Count}");
Console.WriteLine($"Same instance reused: {ReferenceEquals(factory.Get('A'), factory.Get('A'))}");

// ---- Flyweight：只保存「內部狀態」（每個 'A' 都相同的資料），建立後不可變，所以能安全共用 ----
public sealed class Glyph(char symbol, int width, int height)
{
    public char Symbol => symbol;

    // pointSize 是「外部狀態」（每次使用都可能不同），由呼叫端傳入，不存進共用物件
    public void Draw(int pointSize) =>
        Console.WriteLine($"{symbol} (pointsize {pointSize}, glyph {width}x{height})");
}

// ---- FlyweightFactory：同一個符號只建立一次 ----
public sealed class GlyphFactory
{
    private readonly Dictionary<char, Glyph> _glyphs = new();

    public int Count => _glyphs.Count;

    public Glyph Get(char symbol)
    {
        if (!_glyphs.TryGetValue(symbol, out Glyph? glyph))
        {
            glyph = new Glyph(symbol, width: 100, height: 120);
            _glyphs[symbol] = glyph;
        }

        return glyph;
    }
}

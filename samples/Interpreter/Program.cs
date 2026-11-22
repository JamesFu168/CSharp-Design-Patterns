// Interpreter：把一個小型語言的每條文法規則寫成一個物件，再由這些物件合力解讀輸入。

string[] samples = ["MCMXXVIII", "MMXXVI", "XLII"];

foreach (string roman in samples)
{
    Console.WriteLine($"{roman} = {RomanNumeral.Parse(roman)}");
}

// ---- Context：解讀過程中共用的資料（還沒讀完的輸入、目前累計的結果） ----
public sealed class Context(string input)
{
    public string Input { get; set; } = input;
    public int Output { get; set; }
}

// ---- AbstractExpression ----
public interface IExpression
{
    void Interpret(Context context);
}

// ---- TerminalExpression：解讀「一個位數」。
// 千、百、十、個位的規則結構完全相同，只有符號與倍率不同，
// 所以用建構函式參數表達差異，而不是寫四個幾乎一樣的子類別。
public sealed class DigitExpression(
    int multiplier, string one, string? four = null, string? five = null, string? nine = null) : IExpression
{
    public void Interpret(Context context)
    {
        // 9 與 4 的後面不會再接同一個位數的符號，所以解讀完就結束
        if (TryConsume(context, nine, value: 9) || TryConsume(context, four, value: 4))
        {
            return;
        }

        // 5 最多出現一次，後面可以再接 0 到 3 個 1（例如 VIII = 5 + 1 + 1 + 1）
        TryConsume(context, five, value: 5);

        while (TryConsume(context, one, value: 1))
        {
            // 重複到開頭不再是這個符號為止
        }
    }

    // 輸入的開頭符合 symbol 時：累加數值、把已解讀的部分從輸入移除，並回傳 true
    private bool TryConsume(Context context, string? symbol, int value)
    {
        if (symbol is null || !context.Input.StartsWith(symbol, StringComparison.Ordinal))
        {
            return false;
        }

        context.Output += value * multiplier;
        context.Input = context.Input[symbol.Length..];
        return true;
    }
}

public static class RomanNumeral
{
    // 文法：由高位到低位依序解讀
    private static readonly IExpression[] Grammar =
    [
        new DigitExpression(1000, one: "M"),
        new DigitExpression(100, one: "C", four: "CD", five: "D", nine: "CM"),
        new DigitExpression(10, one: "X", four: "XL", five: "L", nine: "XC"),
        new DigitExpression(1, one: "I", four: "IV", five: "V", nine: "IX"),
    ];

    public static int Parse(string roman)
    {
        var context = new Context(roman);

        foreach (IExpression expression in Grammar)
        {
            expression.Interpret(context);
        }

        // 所有規則都跑完還有剩下的字元，代表輸入不符合文法
        if (context.Input.Length > 0)
        {
            throw new FormatException($"'{roman}' is not a valid Roman numeral.");
        }

        return context.Output;
    }
}

# C# Interpreter Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：低 (Low)  
> **原文連結**：[https://www.dofactory.com/net/interpreter-design-pattern](https://www.dofactory.com/net/interpreter-design-pattern)

---

## 定義與概述 (Definition)

給定一個語言，**Interpreter** Design Pattern 會定義其文法（Grammar）的表示方式，以及一個使用該表示方式來直譯（Interpret）該語言中句子的直譯器。

**Interpreter** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Interpreter UML Class Diagram](images/interpreter.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **AbstractExpression** (`Expression`)
  - 宣告一個用於執行直譯（Interpret）操作的介面。
- **TerminalExpression** (`ThousandExpression`, `HundredExpression`, `TenExpression`, `OneExpression`)
  - 實作與文法中之終端符號（Terminal Symbols）相關聯的 `Interpret` 操作。
  - 句子中的每一個終端符號都需要一個對應的實例（Instance）。
- **NonterminalExpression** （未使用）
  - 文法中的每一條規則 `R ::= R1R2...Rn` 都需要一個此類型的類別。
  - 為 `R1` 到 `Rn` 的每個符號維護 `AbstractExpression` 型別的實例變數。
  - 為文法中的非終端符號（Nonterminal Symbols）實作 `Interpret` 操作。`Interpret` 通常會在代表 `R1` 到 `Rn` 的變數上遞迴地呼叫自身。
- **Context** (`Context`)
  - 包含對直譯器而言屬於全域（Global）的資訊。
- **Client** (`InterpreterApp`)
  - 建立（或接收）一個抽象語法樹（Abstract Syntax Tree, AST），用來代表該文法所定義之語言中的某個特定句子。該抽象語法樹是由 `NonterminalExpression` 與 `TerminalExpression` 類別的實例所組裝而成。
  - 呼叫 `Interpret` 操作。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Interpreter Pattern 如何利用已定義的文法，提供能夠處理已剖析陳述式（Parsed Statements）的直譯器。

```csharp
using System;
using System.Collections.Generic;

namespace Interpreter.Structural
{
    /// <summary>
    /// Interpreter Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            Context context = new Context();

            // Usually a tree 

            List<AbstractExpression> list = new List<AbstractExpression>();

            // Populate 'abstract syntax tree' 

            list.Add(new TerminalExpression());
            list.Add(new NonterminalExpression());
            list.Add(new TerminalExpression());
            list.Add(new TerminalExpression());

            // Interpret

            foreach (AbstractExpression exp in list)
            {
                exp.Interpret(context);
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class Context
    {
    }

    /// <summary>
    /// The 'AbstractExpression' abstract class
    /// </summary>

    public abstract class AbstractExpression
    {
        public abstract void Interpret(Context context);
    }

    /// <summary>
    /// The 'TerminalExpression' class
    /// </summary>

    public class TerminalExpression : AbstractExpression
    {
        public override void Interpret(Context context)
        {
            Console.WriteLine("Called Terminal.Interpret()");
        }
    }

    /// <summary>
    /// The 'NonterminalExpression' class
    /// </summary>

    public class NonterminalExpression : AbstractExpression
    {
        public override void Interpret(Context context)
        {
            Console.WriteLine("Called Nonterminal.Interpret()");
        }
    }
}
```

### 輸出結果 (Output)

```text
Called Terminal.Interpret()

  Called Nonterminal.Interpret()

  Called Terminal.Interpret()

  Called Terminal.Interpret()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了如何使用 Interpreter Pattern 將羅馬數字（Roman Numeral）轉換為十進位數字（Decimal）。

```csharp
This is a placeholder code. To be replaced.using System;
using System.Collections.Generic;

namespace Interpreter.RealWorld
{
    /// <summary>
    /// Interpreter Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            string roman = "MCMXXVIII";
            Context context = new Context(roman);

            // Build the 'parse tree'

            List<Expression> tree = new List<Expression>();
            tree.Add(new ThousandExpression());
            tree.Add(new HundredExpression());
            tree.Add(new TenExpression());
            tree.Add(new OneExpression());

            // Interpret

            foreach (Expression exp in tree)
            {
                exp.Interpret(context);
            }

            Console.WriteLine("{0} = {1}",
                roman, context.Output);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class Context
    {
        string input;
        int output;

        // Constructor

        public Context(string input)
        {
            this.input = input;
        }

        public string Input
        {
            get { return input; }
            set { input = value; }
        }

        public int Output
        {
            get { return output; }
            set { output = value; }
        }
    }

    /// <summary>
    /// The 'AbstractExpression' class
    /// </summary>

    public abstract class Expression
    {
        public void Interpret(Context context)
        {
            if (context.Input.Length == 0)
                return;

            if (context.Input.StartsWith(Nine()))
            {
                context.Output += (9 * Multiplier());
                context.Input = context.Input.Substring(2);
            }
            else if (context.Input.StartsWith(Four()))
            {
                context.Output += (4 * Multiplier());
                context.Input = context.Input.Substring(2);
            }
            else if (context.Input.StartsWith(Five()))
            {
                context.Output += (5 * Multiplier());
                context.Input = context.Input.Substring(1);
            }

            while (context.Input.StartsWith(One()))
            {
                context.Output += (1 * Multiplier());
                context.Input = context.Input.Substring(1);
            }
        }

        public abstract string One();
        public abstract string Four();
        public abstract string Five();
        public abstract string Nine();
        public abstract int Multiplier();
    }

    /// <summary>
    /// A 'TerminalExpression' class
    /// <remarks>
    /// Thousand checks for the Roman Numeral M 
    /// </remarks>
    /// </summary>

    public class ThousandExpression : Expression
    {
        public override string One() { return "M"; }
        public override string Four() { return " "; }
        public override string Five() { return " "; }
        public override string Nine() { return " "; }
        public override int Multiplier() { return 1000; }
    }

    /// <summary>
    /// A 'TerminalExpression' class
    /// <remarks>
    /// Hundred checks C, CD, D or CM
    /// </remarks>
    /// </summary>

    public class HundredExpression : Expression
    {
        public override string One() { return "C"; }
        public override string Four() { return "CD"; }
        public override string Five() { return "D"; }
        public override string Nine() { return "CM"; }
        public override int Multiplier() { return 100; }
    }

    /// <summary>
    /// A 'TerminalExpression' class
    /// <remarks>
    /// Ten checks for X, XL, L and XC
    /// </remarks>
    /// </summary>

    public class TenExpression : Expression
    {
        public override string One() { return "X"; }
        public override string Four() { return "XL"; }
        public override string Five() { return "L"; }
        public override string Nine() { return "XC"; }
        public override int Multiplier() { return 10; }
    }

    /// <summary>
    /// A 'TerminalExpression' class
    /// <remarks>
    /// One checks for I, II, III, IV, V, VI, VI, VII, VIII, IX
    /// </remarks>
    /// </summary>

    public class OneExpression : Expression
    {
        public override string One() { return "I"; }
        public override string Four() { return "IV"; }
        public override string Five() { return "V"; }
        public override string Nine() { return "IX"; }
        public override int Multiplier() { return 1; }
    }
}
```

### 輸出結果 (Output)

```text
MCMXXVIII = 1928
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 `record` 型別與集合運算式 `[]`）。這是一個優雅的 C# Interpreter 解決方案。

```csharp
namespace Interpreter.NetOptimized;

using static System.Console;

using System.Collections.Generic;

/// <summary>
/// Interpreter Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Construct the 'parse tree'
        List<Expression> tree = [ new ThousandExpression(), new HundredExpression(),
                                  new TenExpression(), new OneExpression() ];

        // Create the context (i.e. roman value)
        var roman = "MCMXXVIII";
        var context = new Context { Input = roman };

        // Interpret
        tree.ForEach(e => e.Interpret(context));

        WriteLine($"{roman} = {context.Output}");

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Context' class
/// </summary>
public record Context
{
    public string Input { get; set; } = null!;
    public int Output { get; set; }
}

/// <summary>
/// The 'AbstractExpression' abstract class
/// </summary>
public abstract class Expression
{
    public void Interpret(Context context)
    {
        if (context.Input.Length == 0)
            return;

        if (context.Input.StartsWith(Nine()))
        {
            context.Output += (9 * Multiplier());
            context.Input = context.Input.Substring(2);
        }
        else if (context.Input.StartsWith(Four()))
        {
            context.Output += (4 * Multiplier());
            context.Input = context.Input.Substring(2);
        }
        else if (context.Input.StartsWith(Five()))
        {
            context.Output += (5 * Multiplier());
            context.Input = context.Input.Substring(1);
        }

        while (context.Input.StartsWith(One()))
        {
            context.Output += (1 * Multiplier());
            context.Input = context.Input.Substring(1);
        }
    }

    public abstract string One();
    public abstract string Four();
    public abstract string Five();
    public abstract string Nine();
    public abstract int Multiplier();
}

/// <summary>
/// A 'TerminalExpression' class
/// <remarks>
/// Thousand checks for the Roman Numeral M
/// </remarks>
/// </summary>
public class ThousandExpression : Expression
{
    public override string One() => "M";
    public override string Four() => " ";
    public override string Five() => " ";
    public override string Nine() => " ";
    public override int Multiplier() => 1000;
}

/// <summary>
/// A 'TerminalExpression' class
/// <remarks>
/// Hundred checks C, CD, D or CM
/// </remarks>
/// </summary>
public class HundredExpression : Expression
{
    public override string One() => "C";
    public override string Four() => "CD";
    public override string Five() => "D";
    public override string Nine() => "CM";
    public override int Multiplier() => 100;
}

/// <summary>
/// A 'TerminalExpression' class
/// <remarks>
/// Ten checks for X, XL, L and XC
/// </remarks>
/// </summary>
public class TenExpression : Expression
{
    public override string One() => "X";
    public override string Four() => "XL";
    public override string Five() => "L";
    public override string Nine() => "XC";
    public override int Multiplier() => 10;
}

/// <summary>
/// A 'TerminalExpression' class
/// <remarks>
/// One checks for I, II, III, IV, V, VI, VI, VII, VIII, IX
/// </remarks>
/// </summary>
public class OneExpression : Expression
{
    public override string One() => "I";
    public override string Four() => "IV";
    public override string Five() => "V";
    public override string Nine() => "IX";
    public override int Multiplier() => 1;
}
```

### 輸出結果 (Output)

```text
MCMXXVIII = 1928
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **產生過多單一用途的子類別與大量字串配置**：Dofactory 為個、十、百、千位分別建立了 4 個衍生類別（`ThousandExpression` 等），但它們只是覆寫回傳常數字串的 5 個虛擬方法；而且在 `Interpret` 過程中不斷呼叫 `context.Input = context.Input.Substring(...)`，每次都配置新的字串。
- **現代 .NET 11 優化**：將同構的文法規則整合為資料驅動的單一不可變 `RomanRule` `record`，並可搭配 `ReadOnlySpan<char>`（零字串切片配置）來剖析輸入。

### C# 15 / .NET 11 優化重點

- **資料驅動的 `readonly record struct` 文法規則**：消除 4 個重複的衍生類別，直接用 Primary Constructor 定義符號與倍率。
- **`ReadOnlySpan<char>` 零配置剖析**：使用 `StartsWith` 與 Slice 語法 `input[2..]`，在整個直譯過程中**完全不配置任何中間子字串（Zero String Allocation）**！

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Interpreter.ModernNet11;

using static System.Console;

public static class Program
{
    public static void Main()
    {
        const string roman = "MCMXXVIII";
        int result = RomanInterpreter.Interpret(roman);
        WriteLine($"{roman} = {result}");
    }
}

/// <summary>
/// 資料驅動的 TerminalExpression：消除重複的衍生類別
/// </summary>
public readonly record struct RomanExpression(
    string One, string Four, string Five, string Nine, int Multiplier)
{
    // 使用 ReadOnlySpan<char> 與範圍切片 [1..] / [2..]，達成零字串記憶體配置 (Zero-allocation)
    public void Interpret(ref ReadOnlySpan<char> input, ref int output)
    {
        if (input.IsEmpty) return;

        if (input.StartsWith(Nine))
        {
            output += 9 * Multiplier;
            input = input[2..];
        }
        else if (input.StartsWith(Four))
        {
            output += 4 * Multiplier;
            input = input[2..];
        }
        else if (input.StartsWith(Five))
        {
            output += 5 * Multiplier;
            input = input[1..];
        }

        while (input.StartsWith(One))
        {
            output += 1 * Multiplier;
            input = input[1..];
        }
    }
}

public static class RomanInterpreter
{
    private static readonly RomanExpression[] GrammarTree =
    [
        new("M", " ", " ", " ", 1000),
        new("C", "CD", "D", "CM", 100),
        new("X", "XL", "L", "XC", 10),
        new("I", "IV", "V", "IX", 1)
    ];

    public static int Interpret(ReadOnlySpan<char> roman)
    {
        int total = 0;
        foreach (ref readonly var exp in GrammarTree.AsSpan())
        {
            exp.Interpret(ref roman, ref total);
        }
        return total;
    }
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Command](Command.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Iterator →](Iterator.md) |

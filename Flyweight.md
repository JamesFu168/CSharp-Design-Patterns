# C# Flyweight Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：低 (Low)  
> **原文連結**：[https://www.dofactory.com/net/flyweight-design-pattern](https://www.dofactory.com/net/flyweight-design-pattern)

---

## 定義與概述 (Definition)

**Flyweight** Design Pattern 使用共享（Sharing）技術來有效率地支援大量的細粒度（Fine-grained）物件。

**Flyweight** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Flyweight UML Class Diagram](images/flyweight.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Flyweight** (`Character`)
  - 宣告一個介面，讓 Flyweight 物件可以透過此介面接收並處理外部狀態（Extrinsic State）。
- **ConcreteFlyweight** (`CharacterA`, `CharacterB`, ..., `CharacterZ`)
  - 實作 `Flyweight` 介面，並為內部狀態（Intrinsic State，如果有的話）提供儲存空間。`ConcreteFlyweight` 物件必須是可共享的。它所儲存的任何狀態都必須是內部的（Intrinsic），也就是說，它必須獨立於 `ConcreteFlyweight` 物件的脈絡（Context）之外。
- **UnsharedConcreteFlyweight** （未使用）
  - 並非所有的 `Flyweight` 子類別都需要被共享。`Flyweight` 介面「允許」共享，但並不強制要求共享。在 Flyweight 物件結構的某些層級中，`UnsharedConcreteFlyweight` 物件通常會將 `ConcreteFlyweight` 物件作為其子節點（就像 `Row` 與 `Column` 類別一樣）。
- **FlyweightFactory** (`CharacterFactory`)
  - 建立並管理 Flyweight 物件。
  - 確保 Flyweight 物件被適當地共享。當用戶端請求一個 Flyweight 時，`FlyweightFactory` 物件會提供一個既有的實例，若不存在則建立一個新的實例。
- **Client** (`FlyweightApp`)
  - 維護指向一個或多個 Flyweight 的參考。
  - 計算或儲存 Flyweight 的外部狀態（Extrinsic State）。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Flyweight Pattern 如何讓少量的物件能夠被不同的用戶端（Clients）重複共享多次。

```csharp
using System;
using System.Collections.Generic;

namespace Flyweight.Structural
{
    /// <summary>
    /// Flyweight Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Arbitrary extrinsic state

            int extrinsicstate = 22;

            FlyweightFactory factory = new FlyweightFactory();

            // Work with different flyweight instances

            Flyweight fx = factory.GetFlyweight("X");
            fx.Operation(--extrinsicstate);

            Flyweight fy = factory.GetFlyweight("Y");
            fy.Operation(--extrinsicstate);

            Flyweight fz = factory.GetFlyweight("Z");
            fz.Operation(--extrinsicstate);

            UnsharedConcreteFlyweight fu = new
                UnsharedConcreteFlyweight();

            fu.Operation(--extrinsicstate);

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// The 'FlyweightFactory' class
    /// </summary>

    public class FlyweightFactory
    {
        private Dictionary<string, Flyweight> flyweights { get; set; } = new Dictionary<string, Flyweight>();

        // Constructor

        public FlyweightFactory()
        {
            flyweights.Add("X", new ConcreteFlyweight());
            flyweights.Add("Y", new ConcreteFlyweight());
            flyweights.Add("Z", new ConcreteFlyweight());
        }

        public Flyweight GetFlyweight(string key)
        {
            return ((Flyweight)flyweights[key]);
        }
    }

    /// <summary>
    /// The 'Flyweight' abstract class
    /// </summary>

    public abstract class Flyweight
    {
        public abstract void Operation(int extrinsicstate);
    }

    /// <summary>
    /// The 'ConcreteFlyweight' class
    /// </summary>

    public class ConcreteFlyweight : Flyweight
    {
        public override void Operation(int extrinsicstate)
        {
            Console.WriteLine("ConcreteFlyweight: " + extrinsicstate);
        }
    }

    /// <summary>
    /// The 'UnsharedConcreteFlyweight' class
    /// </summary>

    public class UnsharedConcreteFlyweight : Flyweight
    {
        public override void Operation(int extrinsicstate)
        {
            Console.WriteLine("UnsharedConcreteFlyweight: " +
                extrinsicstate);
        }
    }
}
```

### 輸出結果 (Output)

```text
ConcreteFlyweight: 21

  ConcreteFlyweight: 20

  ConcreteFlyweight: 19

  UnsharedConcreteFlyweight: 18
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Flyweight Pattern 如何讓少量的字元（`Character`）物件能夠被一份可能包含大量字元的文件重複共享多次。

```csharp
using System;
using System.Collections.Generic;

namespace Flyweight.RealWorld
{
    /// <summary>
    /// Flyweight Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Build a document with text

            string document = "AAZZBBZB";
            char[] chars = document.ToCharArray();

            CharacterFactory factory = new CharacterFactory();

            // extrinsic state

            int pointSize = 10;

            // For each character use a flyweight object

            foreach (char c in chars)
            {
                pointSize++;
                Character character = factory.GetCharacter(c);
                character.Display(pointSize);
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'FlyweightFactory' class
    /// </summary>

    public class CharacterFactory
    {
        private Dictionary<char, Character> characters = new Dictionary<char, Character>();

        public Character GetCharacter(char key)
        {
            // Uses "lazy initialization"

            Character character = null;

            if (characters.ContainsKey(key))
            {
                character = characters[key];
            }
            else
            {
                switch (key)
                {
                    case 'A': character = new CharacterA(); break;
                    case 'B': character = new CharacterB(); break;
                    //...
                    case 'Z': character = new CharacterZ(); break;
                }
                characters.Add(key, character);
            }
            return character;
        }
    }

    /// <summary>
    /// The 'Flyweight' abstract class
    /// </summary>

    public abstract class Character
    {
        protected char symbol;
        protected int width;
        protected int height;
        protected int ascent;
        protected int descent;
        protected int pointSize;

        public abstract void Display(int pointSize);
    }

    /// <summary>
    /// A 'ConcreteFlyweight' class
    /// </summary>

    public class CharacterA : Character
    {
        // Constructor
        public CharacterA()
        {
            symbol = 'A';
            height = 100;
            width = 120;
            ascent = 70;
            descent = 0;
        }

        public override void Display(int pointSize)
        {
            this.pointSize = pointSize;
            Console.WriteLine(symbol +
                " (pointsize " + this.pointSize + ")");
        }
    }

    /// <summary>
    /// A 'ConcreteFlyweight' class
    /// </summary>

    public class CharacterB : Character
    {
        // Constructor

        public CharacterB()
        {
            symbol = 'B';
            height = 100;
            width = 140;
            ascent = 72;
            descent = 0;
        }

        public override void Display(int pointSize)
        {
            this.pointSize = pointSize;
            Console.WriteLine(this.symbol +
                " (pointsize " + this.pointSize + ")");
        }

    }

    // ... C, D, E, etc.

    /// <summary>
    /// A 'ConcreteFlyweight' class
    /// </summary>

    public class CharacterZ : Character
    {
        // Constructor

        public CharacterZ()
        {
            symbol = 'Z';
            height = 100;
            width = 100;
            ascent = 68;
            descent = 0;
        }

        public override void Display(int pointSize)
        {
            this.pointSize = pointSize;
            Console.WriteLine(this.symbol +
                " (pointsize " + this.pointSize + ")");
        }
    }
}
```

### 輸出結果 (Output)

```text
A (pointsize 11)

  A (pointsize 12)

  Z (pointsize 13)

  Z (pointsize 14)

  B (pointsize 15)

  B (pointsize 16)

  Z (pointsize 17)

  B (pointsize 18)
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如索引子 Indexer、`switch` 運算式與集合初始化）。這是一個優雅的 C# Flyweight 解決方案。

```csharp
namespace Flyweight.NetOptimized;

using System.Collections.Generic;
using static System.Console;

/// <summary>
/// Flyweight Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Build document with text
        var document = "AAZZBBZB";
       
        var factory = new CharacterFactory();

        // extrinsic state
        int pointSize = 10;

        // For each character use a flyweight object
        foreach (char c in document)
        {
            var character = factory.GetCharacter(c);
            character.Display(++pointSize);
        }

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'FlyweightFactory' class
/// </summary>
public class CharacterFactory
{
    private readonly Dictionary<char, Character> characters = [];

    public Character GetCharacter(char key)
    {
        // Uses "lazy initialization"
        Character character;
        if (characters.TryGetValue(key, out Character? value))
        {
            character = value;
        }
        else
        {
            character = key switch
            {
                'A' => new CharacterA(),
                'B' => new CharacterB(),
                // ...
                'Z' => new CharacterZ(),
                _   => null!
            };
            characters.Add(key, character);
        }
        return character;
    }
}

/// <summary>
/// The 'Flyweight' class
/// </summary>
public class Character
{
    protected char symbol;
    protected int width;
    protected int height;
    protected int ascent;
    protected int descent;

    public void Display(int pointSize) =>
       WriteLine($"{symbol} (pointsize {pointSize})");
}

/// <summary>
/// A 'ConcreteFlyweight' class
/// </summary>
public class CharacterA : Character
{
    // Constructor
    public CharacterA()
    {
        symbol = 'A';
        height = 100;
        width = 120;
        ascent = 70;
        descent = 0;
    }
}

/// <summary>
/// A 'ConcreteFlyweight' class
/// </summary>
public class CharacterB : Character
{
    // Constructor
    public CharacterB()
    {
        symbol = 'B';
        height = 100;
        width = 140;
        ascent = 72;
        descent = 0;
    }
}

// ... C, D, E, etc.

/// <summary>
/// A 'ConcreteFlyweight' class
/// </summary>
public class CharacterZ : Character
{
    // Constructor
    public CharacterZ()
    {
        symbol = 'Z';
        height = 100;
        width = 100;
        ascent = 68;
        descent = 0;
    }
}
```

### 輸出結果 (Output)

```text
A (pointsize 11)

    A (pointsize 12)

    Z (pointsize 13)

    Z (pointsize 14)

    B (pointsize 15)

    B (pointsize 16)

    Z (pointsize 17)

    B (pointsize 18)
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Facade](Facade.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Proxy →](Proxy.md) |

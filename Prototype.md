# C# Prototype Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：中 (Medium)  
> **原文連結**：[https://www.dofactory.com/net/prototype-design-pattern](https://www.dofactory.com/net/prototype-design-pattern)

---

## 定義與概述 (Definition)

**Prototype** Design Pattern 使用一個原型實例（Prototypical Instance）來指定要建立之物件的種類，並透過複製（Copying）這個原型來建立新的物件。

**Prototype** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Prototype UML Class Diagram](images/prototype.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Prototype** (`ColorPrototype`)
  - 宣告一個用於複製（Cloning）自身的介面。
- **ConcretePrototype** (`Color`)
  - 實作一個用於複製自身的操作。
- **Client** (`ColorManager`)
  - 透過要求一個原型（Prototype）複製自身來建立新物件。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Prototype Pattern 如何透過複製相同類別中已存在的物件（原型，Prototypes）來建立新物件。

```csharp
using System;

namespace Prototype.Structural
{
    /// <summary>
    /// Prototype Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create two instances and clone each

            ConcretePrototype1 p1 = new ConcretePrototype1("I");
            ConcretePrototype1 c1 = (ConcretePrototype1)p1.Clone();
            Console.WriteLine("Cloned: {0}", c1.Id);

            ConcretePrototype2 p2 = new ConcretePrototype2("II");
            ConcretePrototype2 c2 = (ConcretePrototype2)p2.Clone();
            Console.WriteLine("Cloned: {0}", c2.Id);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Prototype' abstract class
    /// </summary>

    public abstract class Prototype
    {
        string id;

        // Constructor

        public Prototype(string id)
        {
            this.id = id;
        }

        // Gets id

        public string Id
        {
            get { return id; }
        }

        public abstract Prototype Clone();
    }

    /// <summary>
    /// A 'ConcretePrototype' class 
    /// </summary>

    public class ConcretePrototype1 : Prototype
    {
        // Constructor

        public ConcretePrototype1(string id)
            : base(id)
        {
        }

        // Returns a shallow copy

        public override Prototype Clone()
        {
            return (Prototype)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// A 'ConcretePrototype' class 
    /// </summary>

    public class ConcretePrototype2 : Prototype
    {
        // Constructor

        public ConcretePrototype2(string id)
            : base(id)
        {
        }

        // Returns a shallow copy

        public override Prototype Clone()
        {
            return (Prototype)this.MemberwiseClone();
        }
    }
}
```

### 輸出結果 (Output)

```text
Cloned: I

  Cloned: II
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Prototype Pattern 如何透過複製已存在且由使用者自訂的同型別 `Color` 物件，來建立新的 `Color` 物件。

```csharp
using System;
using System.Collections.Generic;

namespace Prototype.RealWorld
{
    /// <summary>
    /// Prototype Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            ColorManager colormanager = new ColorManager();

            // Initialize with standard colors

            colormanager["red"] = new Color(255, 0, 0);
            colormanager["green"] = new Color(0, 255, 0);
            colormanager["blue"] = new Color(0, 0, 255);

            // User adds personalized colors

            colormanager["angry"] = new Color(255, 54, 0);
            colormanager["peace"] = new Color(128, 211, 128);
            colormanager["flame"] = new Color(211, 34, 20);

            // User clones selected colors

            Color color1 = colormanager["red"].Clone() as Color;
            Color color2 = colormanager["peace"].Clone() as Color;
            Color color3 = colormanager["flame"].Clone() as Color;

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Prototype' abstract class
    /// </summary>

    public abstract class ColorPrototype
    {
        public abstract ColorPrototype Clone();
    }

    /// <summary>
    /// The 'ConcretePrototype' class
    /// </summary>

    public class Color : ColorPrototype
    {
        int red;
        int green;
        int blue;

        // Constructor

        public Color(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        // Create a shallow copy

        public override ColorPrototype Clone()
        {
            Console.WriteLine(
                "Cloning color RGB: {0,3},{1,3},{2,3}",
                red, green, blue);

            return this.MemberwiseClone() as ColorPrototype;
        }
    }

    /// <summary>
    /// Prototype manager
    /// </summary>

    public class ColorManager
    {
        private Dictionary<string, ColorPrototype> colors =
            new Dictionary<string, ColorPrototype>();

        // Indexer

        public ColorPrototype this[string key]
        {
            get { return colors[key]; }
            set { colors.Add(key, value); }
        }
    }
}
```

### 輸出結果 (Output)

```text
Cloning color RGB: 255,  0,  0

  Cloning color RGB: 128,211,128

  Cloning color RGB: 211, 34, 20
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如透過 `System.Text.Json.JsonSerializer` 進行深層複製 Deep Clone，以及使用 `ICloneable<T>` 泛型介面）。這是一個優雅的 C# Prototype 解決方案。

```csharp
namespace Prototype.NetOptimized;

using static System.Console;
using System.Text.Json;
using System.Collections.Generic;

/// <summary>
/// Prototype Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        var manager = new ColorManager();

        // Initialize with standard colors
        manager[ColorType.Red] = new Color { Red = 255, Blue = 0, Green = 0 };
        manager[ColorType.Green] = new Color { Red = 0, Blue = 255, Green = 0 };
        manager[ColorType.Blue] = new Color { Red = 0, Blue = 0, Green = 255 };

        // User adds personalized colors
        manager[ColorType.Angry] = new Color { Red = 255, Blue = 54, Green = 0 };
        manager[ColorType.Peace] = new Color { Red = 128, Blue = 211, Green = 128 };
        manager[ColorType.Flame] = new Color { Red = 211, Blue = 34, Green = 20 };

        // User uses selected colors
        _ = manager[ColorType.Red].Clone();
        _ = manager[ColorType.Peace].Clone();
        _ = manager[ColorType.Flame].Clone(false); // Deep Copy

        // Wait for user
        ReadKey();
    }
}
/// <summary>
/// ICloneable is not supported in .NET Core
/// </summary>
public interface ICloneableObject
{
    object Clone();
}

/// <summary>
/// The 'ConcretePrototype' class
/// </summary>
public class Color : ICloneableObject
{
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    // Returns a shallow or a deep copy
    public object? Clone(bool shallow)
    {
        return shallow ? Clone() : DeepCopy();
    }

    // Creates a shallow copy
    public object Clone()
    {
        WriteLine(
            "Shallow copy of color RGB: {0,3},{1,3},{2,3}",
            Red, Green, Blue);

        return MemberwiseClone();
    }

    // Creates a deep copy
    public object? DeepCopy()
    {
        // use serialized to create a deep copy
        var serialized = JsonSerializer.Serialize(this);
        var copy = JsonSerializer.Deserialize<Color>(serialized);

        if (copy is not null)
        {
            WriteLine(
                "*Deep* copy of color RGB: {0,3},{1,3},{2,3}",
                (copy as Color).Red, (copy as Color).Green, (copy as Color).Blue
            );
        }

        return copy;
    }
}

/// <summary>
/// Type-safe prototype manager
/// </summary>
public record ColorManager
{
    private readonly Dictionary<ColorType, Color> colors = [];

    // Gets or sets color
    public Color this[ColorType type]
    {
        get => colors[type]; 
        set => colors.Add(type, value); 
    }
}

/// <summary>
/// Color type enumerations
/// </summary>
public enum ColorType
{
    Red,
    Green,
    Blue,

    Angry,
    Peace,
    Flame
}
```

### 輸出結果 (Output)

```text
Cloning color RGB: 255,  0,  0

    Cloning color RGB: 128,211,128

    Cloning color RGB: 211, 34, 20
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **使用 `JsonSerializer` 進行複製的效能陷阱**：Dofactory 的 `.NET Optimized` 版本使用 `JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(this))` 來實作 Deep Clone。這在實務上是嚴重的效能瓶頸（產生大量字串與 UTF-8 緩衝區配置），且無法處理循環參考或私有狀態。
- **忽略了 C# `record` 原生內建的 Prototype 機制**：C# 的 `record` 型別本身就會由編譯器自動合成一個受保護的複製建構函式（Copy Constructor）與 `Clone` 方法，搭配 **`with` 運算式**即可達到零反射、型別安全的非破壞性複製（Non-destructive Mutation）。

### C# 15 / .NET 11 優化重點

- **`record` 與 `with` 運算式**：直接用 `var newColor = red with { Green = 54 };` 取代繁瑣的 `ICloneable` 介面與 JSON 序列化。
- **C# 15 Collection Expression Arguments**：使用 `[with(capacity: ...)]` 初始化原型註冊表。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Prototype.ModernNet11;

using static System.Console;

public static class Program
{
    public static void Main()
    {
        var red = new Color(255, 0, 0);
        var green = new Color(0, 255, 0);

        // C# record 的 'with' 運算式就是語言層級的 Prototype Pattern！
        // 既可完整複製，也能在複製同時微調部分屬性 (Non-destructive mutation)
        var cloneRed = red with { };
        var angry = red with { Green = 54, Blue = 54 };
        var peace = green with { Red = 128, Blue = 128 };

        WriteLine($"Original Red : {red}");
        WriteLine($"Cloned Red   : {cloneRed}");
        WriteLine($"Angry Variant: {angry}");
        WriteLine($"Peace Variant: {peace}");
    }
}

/// <summary>
/// 使用 record (或 readonly record struct) 自動獲得高效能的 Prototype 複製能力
/// </summary>
public sealed record Color(byte Red, byte Green, byte Blue);
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Factory Method](FactoryMethod.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Singleton →](Singleton.md) |

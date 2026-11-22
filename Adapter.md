# C# Adapter Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/adapter-design-pattern](https://www.dofactory.com/net/adapter-design-pattern)

---

## 定義與概述 (Definition)

**Adapter** Design Pattern 將一個類別的介面轉換成用戶端（Clients）所期望的另一個介面。此 Design Pattern 讓原本因為介面不相容而無法一起運作的類別能夠協同運作。

**Adapter** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Adapter UML Class Diagram](images/adapter.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Target** (`ChemicalCompound`)
  - 定義 `Client` 所使用的特定領域（Domain-specific）介面。
- **Adapter** (`Compound`)
  - 將 `Adaptee` 的介面轉接（Adapts）至 `Target` 介面。
- **Adaptee** (`ChemicalDatabank`)
  - 定義一個需要被轉接的既有介面。
- **Client** (`AdapterApp`)
  - 與符合 `Target` 介面的物件協同運作。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Adapter Pattern 如何將一個類別的介面映射（Maps）到另一個類別上，使它們能夠一起運作。這些不相容的類別可能來自不同的函式庫或框架。

```csharp
using System;

namespace Adapter.Structural
{
    /// <summary>
    /// Adapter Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create adapter and place a request

            Target target = new Adapter();
            target.Request();

            // Wait for user

            Console.ReadKey();
        }

    }

    /// <summary>
    /// The 'Target' class
    /// </summary>

    public class Target
    {
        public virtual void Request()
        {
            Console.WriteLine("Called Target Request()");
        }
    }

    /// <summary>
    /// The 'Adapter' class
    /// </summary>

    public class Adapter : Target
    {
        private Adaptee adaptee = new Adaptee();

        public override void Request()
        {
            // Possibly do some other work
            // and then call SpecificRequest

            adaptee.SpecificRequest();
        }
    }

    /// <summary>
    /// The 'Adaptee' class
    /// </summary>

    public class Adaptee
    {
        public void SpecificRequest()
        {
            Console.WriteLine("Called SpecificRequest()");
        }
    }
}
```

### 輸出結果 (Output)

```text
Called SpecificRequest()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了如何使用一個舊版（Legacy）的化學資料庫（`ChemicalDatabank`）。化學化合物（`Compound`）物件會透過一個 `Adapter` 介面來存取該資料庫。

```csharp
using System;

namespace Adapter.RealWorld
{
    /// <summary>
    /// Adapter Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Non-adapted chemical compound

            Compound unknown = new Compound();
            unknown.Display();

            // Adapted chemical compounds

            Compound water = new RichCompound("Water");
            water.Display();

            Compound benzene = new RichCompound("Benzene");
            benzene.Display();

            Compound ethanol = new RichCompound("Ethanol");
            ethanol.Display();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Target' class
    /// </summary>

    public class Compound
    {
        protected float boilingPoint;
        protected float meltingPoint;
        protected double molecularWeight;
        protected string molecularFormula;

        public virtual void Display()
        {
            Console.WriteLine("\nCompound: Unknown ------ ");
        }
    }

    /// <summary>
    /// The 'Adapter' class
    /// </summary>

    public class RichCompound : Compound
    {
        private string chemical;
        private ChemicalDatabank bank;

        // Constructor

        public RichCompound(string chemical)
        {
            this.chemical = chemical;
        }

        public override void Display()
        {
            // The Adaptee

            bank = new ChemicalDatabank();

            boilingPoint = bank.GetCriticalPoint(chemical, "B");
            meltingPoint = bank.GetCriticalPoint(chemical, "M");
            molecularWeight = bank.GetMolecularWeight(chemical);
            molecularFormula = bank.GetMolecularStructure(chemical);

            Console.WriteLine("\nCompound: {0} ------ ", chemical);
            Console.WriteLine(" Formula: {0}", molecularFormula);
            Console.WriteLine(" Weight : {0}", molecularWeight);
            Console.WriteLine(" Melting Pt: {0}", meltingPoint);
            Console.WriteLine(" Boiling Pt: {0}", boilingPoint);
        }
    }

    /// <summary>
    /// The 'Adaptee' class
    /// </summary>

    public class ChemicalDatabank
    {
        // The databank 'legacy API'

        public float GetCriticalPoint(string compound, string point)
        {
            // Melting Point
            if (point == "M")
            {
                switch (compound.ToLower())
                {
                    case "water": return 0.0f;
                    case "benzene": return 5.5f;
                    case "ethanol": return -114.1f;
                    default: return 0f;
                }
            }

            // Boiling Point

            else
            {
                switch (compound.ToLower())
                {
                    case "water": return 100.0f;
                    case "benzene": return 80.1f;
                    case "ethanol": return 78.3f;
                    default: return 0f;
                }
            }
        }

        public string GetMolecularStructure(string compound)
        {
            switch (compound.ToLower())
            {
                case "water": return "H20";
                case "benzene": return "C6H6";
                case "ethanol": return "C2H5OH";
                default: return "";
            }
        }

        public double GetMolecularWeight(string compound)
        {
            switch (compound.ToLower())
            {
                case "water": return 18.015;
                case "benzene": return 78.1134;
                case "ethanol": return 46.0688;
                default: return 0d;
            }
        }
    }
}
```

### 輸出結果 (Output)

```text
Compound: Unknown ------

  

  Compound: Water ------

   Formula: H20

   Weight : 18.015

   Melting Pt: 0

   Boiling Pt: 100

  

  Compound: Benzene ------

   Formula: C6H6

   Weight : 78.1134

   Melting Pt: 5.5

   Boiling Pt: 80.1

  

  Compound: Alcohol ------

   Formula: C2H6O2

   Weight : 46.0688

   Melting Pt: -114.1

   Boiling Pt: 78.3
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 C# `switch` 運算式 Pattern Matching 與列舉型別 `Chemical`）。這是一個優雅的 C# Adapter 解決方案。

```csharp
namespace Adapter.NetOptimized;

using static System.Console;

/// <summary>
/// Adapter Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Non-adapted chemical compound 
        var unknown = new Compound();
        unknown.Display();

        // Adapted chemical compounds
        var water = new RichCompound(Chemical.Water);
        water.Display();

        var benzene = new RichCompound(Chemical.Benzene);
        benzene.Display();

        var ethanol = new RichCompound(Chemical.Ethanol);
        ethanol.Display();

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Target' class
/// </summary>
public class Compound
{
    public Chemical Chemical { get; protected set; }
    public float BoilingPoint { get; protected set; }
    public float MeltingPoint { get; protected set; }
    public double MolecularWeight { get; protected set; }
    public string? MolecularFormula { get; protected set; }

    public virtual void Display()
    {
        WriteLine("\nCompound: Unknown ------ ");
    }
}

/// <summary>
/// The 'Adapter' class
/// </summary>
public class RichCompound : Compound
{
    private readonly ChemicalDatabank bank = new();

    // Constructor
    public RichCompound(Chemical chemical)
    {
        Chemical = chemical;
    }

    public override void Display()
    {
        // Adaptee request methods
        BoilingPoint = bank.GetCriticalPoint(Chemical, State.Boiling);
        MeltingPoint = bank.GetCriticalPoint(Chemical, State.Melting);
        MolecularWeight = bank.GetMolecularWeight(Chemical);
        MolecularFormula = bank.GetMolecularStructure(Chemical);

        WriteLine($"\nCompound: { Chemical} ------ ");
        WriteLine($" Formula: {MolecularFormula}");
        WriteLine($" Weight : {MolecularWeight}");
        WriteLine($" Melting Pt: {MeltingPoint}");
        WriteLine($" Boiling Pt: {BoilingPoint}");
    }
}

/// <summary>
/// The 'Adaptee' class
/// </summary>
public class ChemicalDatabank
{
    // The databank 'legacy API'
    public float GetCriticalPoint(Chemical compound, State point)
    {
        // Melting Point
        if (point == State.Melting)
        {
            return compound switch
            {
                Chemical.Water => 0.0f,
                Chemical.Benzene => 5.5f,
                Chemical.Ethanol => -114.1f,
                _ => 0f,
            };
        }
        // Boiling Point
        else
        {
            return compound switch
            {
                Chemical.Water => 100.0f,
                Chemical.Benzene => 80.1f,
                Chemical.Ethanol => 78.3f,
                _ => 0f,
            };
        }
    }

    public string GetMolecularStructure(Chemical compound)
    {
        return compound switch
        {
            Chemical.Water => "H20",
            Chemical.Benzene => "C6H6",
            Chemical.Ethanol => "C2H5OH",
            _ => "",
        };
    }

    public double GetMolecularWeight(Chemical compound)
    {
        return compound switch
        {
            Chemical.Water => 18.015d,
            Chemical.Benzene => 78.1134d,
            Chemical.Ethanol => 46.0688d,
            _ => 0d
        };
    }
}


/// <summary>
/// Chemical enumeration
/// </summary>
public enum Chemical
{
    Water,
    Benzene,
    Ethanol
}

/// <summary>
/// State enumeration
/// </summary>
public enum State
{
    Boiling,
    Melting
}
```

### 輸出結果 (Output)

```text
Compound: Unknown ------

    

    Compound: Water ------

     Formula: H20

     Weight : 18.015

     Melting Pt: 0

     Boiling Pt: 100

    

    Compound: Benzene ------

     Formula: C6H6

     Weight : 78.1134

     Melting Pt: 5.5

     Boiling Pt: 80.1

    

    Compound: Alcohol ------

     Formula: C2H6O2

     Weight : 46.0688

     Melting Pt: -114.1

     Boiling Pt: 78.3
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Singleton](Singleton.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Bridge →](Bridge.md) |

# C# Template Method Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：中 (Medium)  
> **原文連結**：[https://www.dofactory.com/net/template-method-design-pattern](https://www.dofactory.com/net/template-method-design-pattern)

---

## 定義與概述 (Definition)

**Template Method** Design Pattern 在一個操作中定義了演算法的骨架（Skeleton），並將某些步驟推遲（Deferring）到子類別中實作。此 Pattern 讓子類別可以在不改變演算法結構的情況下，重新定義演算法的某些特定步驟。

**Template Method** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Template Method UML Class Diagram](images/template.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **AbstractClass** (`DataObject` / `DataAccessor`)
  - 定義抽象的*基本操作（Primitive Operations）*，由具體子類別（Concrete Subclasses）實作這些操作以完成演算法的各個步驟。
  - 實作一個定義演算法骨架的範本方法（Template Method）。此 Template Method 不僅會呼叫基本操作，也會呼叫定義在 `AbstractClass` 或其他物件中的操作。
- **ConcreteClass** (`CustomerDataObject` / `CategoryAccessor`, `ProductAccessor`)
  - 實作基本操作，以執行演算法中專屬於子類別的步驟。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Template Method Pattern 如何提供方法呼叫順序的骨架。一個或多個步驟可以推遲到子類別中，由子類別實作這些步驟，而不會改變整體的呼叫順序。

```csharp
using System;

namespace Template.Structural
{
    /// <summary>
    /// Template Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            AbstractClass aA = new ConcreteClassA();
            aA.TemplateMethod();

            AbstractClass aB = new ConcreteClassB();
            aB.TemplateMethod();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'AbstractClass' abstract class
    /// </summary>

    public abstract class AbstractClass
    {
        public abstract void PrimitiveOperation1();
        public abstract void PrimitiveOperation2();

        // The "Template method"

        public void TemplateMethod()
        {
            PrimitiveOperation1();
            PrimitiveOperation2();
            Console.WriteLine("");
        }
    }

    /// <summary>
    /// A 'ConcreteClass' class
    /// </summary>

    public class ConcreteClassA : AbstractClass
    {
        public override void PrimitiveOperation1()
        {
            Console.WriteLine("ConcreteClassA.PrimitiveOperation1()");
        }

        public override void PrimitiveOperation2()
        {
            Console.WriteLine("ConcreteClassA.PrimitiveOperation2()");
        }
    }

    /// <summary>
    /// A 'ConcreteClass' class
    /// </summary>

    public class ConcreteClassB : AbstractClass
    {
        public override void PrimitiveOperation1()
        {
            Console.WriteLine("ConcreteClassB.PrimitiveOperation1()");
        }

        public override void PrimitiveOperation2()
        {
            Console.WriteLine("ConcreteClassB.PrimitiveOperation2()");
        }
    }
}
```

### 輸出結果 (Output)

```text
ConcreteClassA.PrimitiveOperation1()

  ConcreteClassA.PrimitiveOperation2()

  ConcreteClassB.PrimitiveOperation1()

  ConcreteClassB.PrimitiveOperation2()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了一個名為 `Run()` 的 Template Method，它提供了方法呼叫順序的骨架。這些步驟的實作被推遲到了 `CustomerDataObject` 子類別中，由該子類別實作 `Connect`、`Select`、`Process` 與 `Disconnect` 方法。

```csharp
using System;
using System.Collections.Generic;

namespace Template.RealWorld
{
    /// <summary>
    /// Template Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            DataAccessor categories = new Categories();
            categories.Run(5);

            DataAccessor products = new Products();
            products.Run(3);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'AbstractClass' abstract class
    /// </summary>

    public abstract class DataAccessor
    {
        public abstract void Connect();
        public abstract void Select();
        public abstract void Process(int top);
        public abstract void Disconnect();

        // The 'Template Method' 

        public void Run(int top)
        {
            Connect();
            Select();
            Process(top);
            Disconnect();
        }
    }

    /// <summary>
    /// A 'ConcreteClass' class
    /// </summary>

    public class Categories : DataAccessor
    {
        private List<string> categories;

        public override void Connect()
        {
            categories = new List<string>();
        }

        public override void Select()
        {
            categories.Add("Red");
            categories.Add("Green");
            categories.Add("Blue");
            categories.Add("Yellow");
            categories.Add("Purple");
            categories.Add("White");
            categories.Add("Black");
        }

        public override void Process(int top)
        {
            Console.WriteLine("Categories ---- ");

            for(int i = 0; i < top; i++)
            {
                Console.WriteLine(categories[i]);
            }
            
            Console.WriteLine();
        }

        public override void Disconnect()
        {
            categories.Clear();
        }
    }

    /// <summary>
    /// A 'ConcreteClass' class
    /// </summary>

    public class Products : DataAccessor
    {
        private List<string> products;

        public override void Connect()
        {
            products = new List<string>();
        }

        public override void Select()
        {
            products.Add("Car");
            products.Add("Bike");
            products.Add("Boat");
            products.Add("Truck");
            products.Add("Moped");
            products.Add("Rollerskate");
            products.Add("Stroller");
        }

        public override void Process(int top)
        {
            Console.WriteLine("Products ---- ");

            for (int i = 0; i < top; i++)
            {
                Console.WriteLine(products[i]);
            }

            Console.WriteLine();
        }

        public override void Disconnect()
        {
            products.Clear();
        }
    }
}
```

### 輸出結果 (Output)

```text
Categories ----

  Beverages

  Condiments

  Confections

  Dairy Products

  Grains/Cereals

  Meat/Poultry

  Produce

  Seafood

  

  Products ----

  Chai

  Chang

  Aniseed Syrup

  Chef Anton's Cajun Seasoning

  Chef Anton's Gumbo Mix

  Grandma's Boysenberry Spread

  Uncle Bob's Organic Dried Pears

  Northwoods Cranberry Sauce

  Mishi Kobe Niku
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如泛型 `DataAccessor<T>`、`record` 型別與集合運算式 `[]`）。這是一個優雅的 C# Template Method 解決方案。

```csharp
namespace Template.NetOptimized;

using static System.Console;

/// <summary>
/// Template Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        var categories = new CategoryAccessor();
        categories.Run(5);

        var products = new ProductAccessor();
        products.Run(3);

        // Wait for user
        ReadKey();
    }
}

public record Category
{
    public string CategoryName { get; set; } = null!;
}

public record Product
{
    public string ProductName { get; set; } = null!;
}

/// <summary>
/// The 'AbstractClass' abstract class
/// </summary>
public abstract class DataAccessor<T> where T : class, new()
{
    protected List<T> Items { get; set; } = [];

    public virtual void Connect()
    {
        Items.Clear();
    }
    public abstract void Select();
    public abstract void Process(int top);
    public virtual void Disconnect()
    {
        Items.Clear();
    }

    // The 'Template Method' 
    public void Run(int top)
    {
        Connect();
        Select();
        Process(top);
        Disconnect();
    }
}

/// <summary>
/// A 'ConcreteClass' class
/// </summary>
public class CategoryAccessor : DataAccessor<Category>
{
    public override void Select()
    {
        Items.Add(new() { CategoryName = "Red" });
        Items.Add(new() { CategoryName = "Green" });
        Items.Add(new() { CategoryName = "Blue" });
        Items.Add(new() { CategoryName = "Yellow" });
        Items.Add(new() { CategoryName = "Purple" }); ;
        Items.Add(new() { CategoryName = "White" });
        Items.Add(new() { CategoryName = "Black" });
    }

    public override void Process(int top)
    {
        WriteLine("Categories ---- ");

        for (int i = 0; i < top; i++)
        {
            WriteLine(Items[i].CategoryName);
        }

        WriteLine();
    }
}

/// <summary>
/// A 'ConcreteClass' class
/// </summary>
public class ProductAccessor : DataAccessor<Product>
{
    public override void Select()
    {
        Items.Add(new Product { ProductName = "Car" });
        Items.Add(new Product { ProductName = "Bike" });
        Items.Add(new Product { ProductName = "Boat" });
        Items.Add(new Product { ProductName = "Truck" });
        Items.Add(new Product { ProductName = "Moped" });
        Items.Add(new Product { ProductName = "Rollerskate" });
        Items.Add(new Product { ProductName = "Stroller" });
    }

    public override void Process(int top)
    {
        WriteLine("Products ---- ");

        for (int i = 0; i < top; i++)
        {
            WriteLine(Items[i].ProductName);
        }

        WriteLine();
    }
}
```

### 輸出結果 (Output)

```text
Categories ----

    Beverages

    Condiments

    Confections

    Dairy Products

    Grains/Cereals

    Meat/Poultry

    Produce

    Seafood

    

    Products ----

    Chai

    Chang

    Aniseed Syrup

    Chef Anton's Cajun Seasoning

    Chef Anton's Gumbo Mix

    Grandma's Boysenberry Spread

    Uncle Bob's Organic Dried Pears

    Northwoods Cranberry Sauce

    Mishi Kobe Niku
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **不必要的 `where T : class, new()` 限制與 `null!` 警告壓制**：Dofactory 的 `.NET Optimized` 在 `Category` 與 `Product` 中寫了 `public string CategoryName { get; set; } = null!;`，就是為了遷就 `DataAccessor<T> where T : class, new()` 的無參數建構函式限制。
- **現代 C# 15 改進**：移除無意義的 `new()` 限制，讓 `Category` 與 `Product` 可以直接使用簡潔、不可變且具型別安全的 **`readonly record struct`（或 Primary Constructor `record`）**，並利用 `CollectionsMarshal.AsSpan` 或 `Span` 切片 `Items[..top]` 來處理前 N 筆資料！

### C# 15 / .NET 11 優化重點

- **Primary Constructor `record` 消除 `= null!`**：不再需要為了 `new()` 限制而犧牲 Null 安全性。
- **集合切片（Range / Slice `Items[..top]`）**：在 `Process(int top)` 中直接使用 `foreach (var item in Items[..top])` 取代傳統的 `for (int i = 0; i < top; i++)` 迴圈。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Template.ModernNet11;

using static System.Console;

public static class Program
{
    public static void Main()
    {
        new CategoryAccessor().Run(5);
        new ProductAccessor().Run(3);
    }
}

public readonly record struct Category(string CategoryName);
public readonly record struct Product(string ProductName);

public abstract class DataAccessor<T>
{
    protected List<T> Items { get; set; } = [];

    protected virtual void Connect() => Items.Clear();
    protected abstract void Select();
    protected abstract void Process(int top);
    protected virtual void Disconnect() => Items.Clear();

    // The 'Template Method'
    public void Run(int top)
    {
        Connect();
        Select();
        Process(top);
        Disconnect();
    }
}

public sealed class CategoryAccessor : DataAccessor<Category>
{
    protected override void Select() =>
        Items =
        [
            with(capacity: 7),
            new("Red"), new("Green"), new("Blue"),
            new("Yellow"), new("Purple"), new("White"), new("Black")
        ];

    protected override void Process(int top)
    {
        WriteLine("Categories ---- ");
        // 使用現代 C# Range 切片語法 [..top]
        foreach (var item in Items[..top])
        {
            WriteLine(item.CategoryName);
        }
        WriteLine();
    }
}

public sealed class ProductAccessor : DataAccessor<Product>
{
    protected override void Select() =>
        Items =
        [
            with(capacity: 7),
            new("Car"), new("Bike"), new("Boat"),
            new("Truck"), new("Moped"), new("Rollerskate"), new("Stroller")
        ];

    protected override void Process(int top)
    {
        WriteLine("Products ---- ");
        foreach (var item in Items[..top])
        {
            WriteLine(item.ProductName);
        }
        WriteLine();
    }
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Strategy](Strategy.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Visitor →](Visitor.md) |

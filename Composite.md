# C# Composite Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/composite-design-pattern](https://www.dofactory.com/net/composite-design-pattern)

---

## 定義與概述 (Definition)

**Composite** Design Pattern 將物件組合成樹狀結構（Tree Structures），以表示「部分－整體」（Part-Whole）的階層關係。此 Pattern 讓用戶端（Clients）可以用一致的方式來對待個別物件與物件的組合（Compositions）。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

繪圖程式裡，圖形可以組成群組，群組裡還可以再放群組。如果「單一圖形」與「群組」是兩種毫無關係的型別，處理它們的程式碼會是：

```csharp
// 示意：呼叫端必須分辨單一圖形與群組，每多一層巢狀就多一層迴圈
foreach (object item in picture.Items)
{
    if (item is Shape shape)
    {
        Draw(shape);
    }
    else if (item is Group group)
    {
        foreach (object child in group.Items) { /* 又要再判斷一次 */ }
    }
}
```

巢狀有幾層，判斷就要寫幾層；而每一個要處理圖形的功能（繪製、計數、搬移）都得重複這套判斷。

Composite 讓單一物件與群組實作同一個介面，群組把操作轉交給自己的成員。呼叫端從此不需要分辨兩者。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Composite UML Class Diagram](images/composite.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Component** (`DrawingElement`)
  - 為組合（Composition）中的物件宣告介面。
  - 在適當情況下，實作所有類別共用的介面預設行為。
  - 宣告用於存取及管理其子組件（Child Components）的介面。
  - （選用）定義一個在遞迴結構中存取組件之父節點（Parent）的介面，並在適當時實作它。
- **Leaf** (`PrimitiveElement`)
  - 代表組合中的葉節點（Leaf）物件。葉節點沒有任何子節點。
  - 定義組合中基本（Primitive）物件的行為。
- **Composite** (`CompositeElement`)
  - 定義擁有子節點之組件的行為。
  - 儲存子組件（Child Components）。
  - 實作 `Component` 介面中與子節點相關的操作。
- **Client** (`CompositeApp`)
  - 透過 `Component` 介面來操作組合中的物件。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Composite](samples/Composite)  
> **執行方式**：`dotnet run --project samples/Composite`

**角色對照**：Component = `IGraphic`；Leaf = `Shape`；Composite = `Group`；Client = 最上方的程式碼。

<!-- sample:Composite/Program.cs -->
```csharp
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
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Composite -->
```text
+ Picture
  - Red Line
  - Blue Circle
  + Two Circles
    - Black Circle
    - White Circle
  - Green Box
Total shapes: 5
- Yellow Line
Total shapes: 1
```
<!-- /output -->

### 重點解說

- **遞迴**：`Group.Draw` 會呼叫每個子節點的 `Draw`。子節點如果也是 `Group`，就再往下一層，巢狀幾層都不需要多寫程式碼。
- **第二種操作一樣簡單**：`ShapeCount` 用 `Sum` 加總子節點的數量，不需要知道子節點是哪一種。
- **程式碼的形狀就是樹的形狀**：`params IGraphic[] children` 讓建立群組時可以直接列出成員，巢狀的 `new` 對應巢狀的結構。
- **`Add` 與 `Remove` 只在 `Group` 上**：對 `Shape` 呼叫 `Add` 會是編譯錯誤，而不是執行期才發現的錯誤。

---

## 與 Dofactory 原版的差異

- 原版把 `Add` 與 `Remove` 宣告在 Component 上，葉節點只能在被呼叫時印出 `Cannot add to a PrimitiveElement`，錯誤到執行期才看得到。本版只把這兩個方法放在 `Group`。
- 原版 .NET Optimized 改用單一的泛型 `TreeNode<T>` 類別，已經看不出 Leaf 與 Composite 兩種角色。本版保留三個角色，方便對照 UML。
- 本版新增 `ShapeCount`，示範「顯示」以外的第二種遞迴操作。

---

## .NET 中的對應 (In .NET)

- **`System.Xml.Linq`**：`XElement` 可以包含其他 `XElement`，對單一元素與整棵子樹的操作方式相同。
- **UI 控制項樹**：WinForms、WPF、.NET MAUI 的容器控制項可以包含其他控制項，Blazor 的元件樹也是。
- **`System.Linq.Expressions`**：運算式樹的節點可以包含其他運算式。
- **檔案系統**：`DirectoryInfo` 與 `FileInfo` 共用基底類別 `FileSystemInfo`。

---

## 何時該用、何時不該用

**適合使用**

- 資料天生是樹狀結構（部門與員工、選單、資料夾），而且希望對單一節點與整棵子樹用相同的方式操作。

**不適合使用**

- 階層固定只有兩層：兩個集合就足夠。
- 葉節點與容器能做的事差異很大：硬套同一個介面，會出現一堆「不支援」的方法。

---

## 練習題

1. 在 `IGraphic` 加上 `IEnumerable<string> GetNames()`，回傳自己與所有子孫的名稱（提示：`yield return` 加上遞迴）。
2. 把一個群組加進它自己（`group.Add(group)`）之後呼叫 `Draw` 會發生什麼事？在 `Add` 加上檢查來防止。
3. 新增 `Move(int dx, int dy)` 操作。`Shape` 與 `Group` 各需要寫什麼？

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Composite Pattern 如何允許建立樹狀結構，在該結構中，無論個別節點是葉節點（Leaf Nodes）還是分支（Composite）節點，都能以一致的方式進行存取。

```csharp
using System;
using System.Collections.Generic;

namespace Composite.Structural
{
    /// <summary>
    /// Composite Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a tree structure

            Composite root = new Composite("root");
            root.Add(new Leaf("Leaf A"));
            root.Add(new Leaf("Leaf B"));

            Composite comp = new Composite("Composite X");
            comp.Add(new Leaf("Leaf XA"));
            comp.Add(new Leaf("Leaf XB"));

            root.Add(comp);
            root.Add(new Leaf("Leaf C"));

            // Add and remove a leaf

            Leaf leaf = new Leaf("Leaf D");
            root.Add(leaf);
            root.Remove(leaf);

            // Recursively display tree

            root.Display(1);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Component' abstract class
    /// </summary>

    public abstract class Component
    {
        protected string name;

        // Constructor

        public Component(string name)
        {
            this.name = name;
        }

        public abstract void Add(Component c);
        public abstract void Remove(Component c);
        public abstract void Display(int depth);
    }

    /// <summary>
    /// The 'Composite' class
    /// </summary>

    public class Composite : Component
    {
        List<Component> children = new List<Component>();

        // Constructor

        public Composite(string name)
            : base(name)
        {
        }

        public override void Add(Component component)
        {
            children.Add(component);
        }

        public override void Remove(Component component)
        {
            children.Remove(component);
        }

        public override void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + name);

            // Recursively display child nodes

            foreach (Component component in children)
            {
                component.Display(depth + 2);
            }
        }
    }

    /// <summary>
    /// The 'Leaf' class
    /// </summary>

    public class Leaf : Component
    {
        // Constructor

        public Leaf(string name)
            : base(name)
        {
        }

        public override void Add(Component c)
        {
            Console.WriteLine("Cannot add to a leaf");
        }

        public override void Remove(Component c)
        {
            Console.WriteLine("Cannot remove from a leaf");
        }

        public override void Display(int depth)
        {
            Console.WriteLine(new String('-', depth) + name);
        }
    }
}
```

**輸出結果 (Output)**

```text
-root
---Leaf A
---Leaf B
---Composite X
-----Leaf XA
-----Leaf XB
---Leaf C
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了如何使用 Composite Pattern 來建立一個圖形樹狀結構，該結構由基本節點（線條、圓形等）與組合節點（構成更複雜元素的繪圖元素群組）所組成。

```csharp
using System;
using System.Collections.Generic;

namespace Composite.RealWorld
{
    /// <summary>
    /// Composite Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a tree structure 

            CompositeElement root = new CompositeElement("Picture");
            root.Add(new PrimitiveElement("Red Line"));
            root.Add(new PrimitiveElement("Blue Circle"));
            root.Add(new PrimitiveElement("Green Box"));

            // Create a branch

            CompositeElement comp = new CompositeElement("Two Circles");
            comp.Add(new PrimitiveElement("Black Circle"));
            comp.Add(new PrimitiveElement("White Circle"));
            root.Add(comp);

            // Add and remove a PrimitiveElement

            PrimitiveElement pe = new PrimitiveElement("Yellow Line");
            root.Add(pe);
            root.Remove(pe);

            // Recursively display nodes

            root.Display(1);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Component' Treenode
    /// </summary>

    public abstract class DrawingElement
    {
        protected string name;

        // Constructor

        public DrawingElement(string name)
        {
            this.name = name;
        }

        public abstract void Add(DrawingElement d);
        public abstract void Remove(DrawingElement d);
        public abstract void Display(int indent);
    }

    /// <summary>
    /// The 'Leaf' class
    /// </summary>

    public class PrimitiveElement : DrawingElement
    {
        // Constructor

        public PrimitiveElement(string name)
            : base(name)
        {
        }

        public override void Add(DrawingElement c)
        {
            Console.WriteLine(
                "Cannot add to a PrimitiveElement");
        }

        public override void Remove(DrawingElement c)
        {
            Console.WriteLine(
                "Cannot remove from a PrimitiveElement");
        }

        public override void Display(int indent)
        {
            Console.WriteLine(
                new String('-', indent) + " " + name);
        }
    }

    /// <summary>
    /// The 'Composite' class
    /// </summary>

    public class CompositeElement : DrawingElement
    {
        List<DrawingElement> elements = new List<DrawingElement>();

        // Constructor

        public CompositeElement(string name)
            : base(name)
        {
        }

        public override void Add(DrawingElement d)
        {
            elements.Add(d);
        }

        public override void Remove(DrawingElement d)
        {
            elements.Remove(d);
        }

        public override void Display(int indent)
        {
            Console.WriteLine(new String('-', indent) +
                "+ " + name);

            // Display each child element on this node

            foreach (DrawingElement d in elements)
            {
                d.Display(indent + 2);
            }
        }
    }
}
```

**輸出結果 (Output)**

```text
-+ Picture
--- Red Line
--- Blue Circle
--- Green Box
---+ Two Circles
----- Black Circle
----- White Circle
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如泛型 `TreeNode<T>` 與 `IComparable<T>`）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Composite.NetOptimized;

using static System.Console;

using System;
using System.Collections.Generic;

/// <summary>
/// Composite Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Build a tree of shapes
        var root = new TreeNode<Shape> { Node = new ("Picture") };

        root.Add(new ("Red Line"));
        root.Add(new ("Blue Circle"));
        root.Add(new ("Green Box"));

        var branch = root.Add(new Shape("Two Circles"));
        branch.Add(new ("Black Circle"));
        branch.Add(new ("White Circle"));

        // Add, remove, and add a shape
        var shape = new Shape("Yellow Line");
        root.Add(shape);
        root.Remove(shape);
        root.Add(shape);

        // Display tree using static method
        TreeNode<Shape>.Display(root, 1);

        ReadKey();
    }
}

/// <summary>
/// Generic tree node class
/// </summary>
/// <typeparam name="T">Node type</typeparam>
public class TreeNode<T> where T : IComparable<T>
{

    // Add a child tree node
    public TreeNode<T> Add(T child)
    {
        var newNode = new TreeNode<T> { Node = child };
        Children.Add(newNode);
        return newNode;
    }

    // Remove a child tree node
    public void Remove(T child)
    {
        foreach (var treeNode in Children)
        {
            if (treeNode.Node.CompareTo(child) == 0)
            {
                Children.Remove(treeNode);
                return;
            }
        }
    }

    // Gets or sets the node
    public T Node { get; set; } = default!;

    // Gets treenode children
    public List<TreeNode<T>> Children { get; } = [];

    // Recursively displays node and its children 
    public static void Display(TreeNode<T> node, int indentation)
    {
        var line = new string('-', indentation);
        WriteLine(line + " " + node.Node);

        node.Children.ForEach(n => Display(n, indentation + 1));
    }
}

/// <summary>
/// Shape class
/// <remarks>
/// Implements generic IComparable interface
/// </remarks>
/// </summary>
public class Shape(string name) : IComparable<Shape>
{
    public override string ToString() => name;

    // IComparable<Shape> Member
    public int CompareTo(Shape? other) => (this == other) ? 0 : -1;
}
```

**輸出結果 (Output)**

```text
- Picture
-- Red Line
-- Blue Circle
-- Green Box
-- Two Circles
--- Black Circle
--- White Circle
-- Yellow Line
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Bridge](Bridge.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Decorator →](Decorator.md) |

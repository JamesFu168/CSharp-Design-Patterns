# C# Visitor Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：低 (Low)  
> **原文連結**：[https://www.dofactory.com/net/visitor-design-pattern](https://www.dofactory.com/net/visitor-design-pattern)

---

## 定義與概述 (Definition)

**Visitor** Design Pattern 代表一個要在某個物件結構（Object Structure）之各個元素上執行的操作。此 Pattern 讓你可以在不修改各元素所屬類別的情況下，定義一個新的操作。

**Visitor** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Visitor UML Class Diagram](images/visitor.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Visitor** (`Visitor` / `IVisitor`)
  - 為物件結構中的每一個 `ConcreteElement` 類別宣告一個 `Visit` 操作。該操作的名稱與簽章（Signature）會識別出向 Visitor 發送 `Visit` 請求的類別。這讓 Visitor 可以確定正在被拜訪之元素的具體類別，接著 Visitor 就可以直接透過該元素的特定介面來存取它。
- **ConcreteVisitor** (`IncomeVisitor`, `VacationVisitor`)
  - 實作由 `Visitor` 所宣告的每一個操作。每個操作都實作了為結構中對應之類別或物件所定義之演算法的一部分。`ConcreteVisitor` 為演算法提供脈絡（Context），並儲存其區域狀態（Local State）。在走訪結構的過程中，這個狀態通常會用來累積結果。
- **Element** (`Element`)
  - 定義一個以 `Visitor` 作為引數（Argument）的 `Accept` 操作。
- **ConcreteElement** (`Employee`)
  - 實作以 `Visitor` 作為引數的 `Accept` 操作。
- **ObjectStructure** (`Employees`)
  - 可以列舉（Enumerate）它的各個元素。
  - 可以提供一個高階介面，允許 `Visitor` 拜訪它的元素。
  - 可以是一個 **Composite**（組合模式），或是一個集合（Collection），例如清單（`List`）或集合（`Set`）。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Visitor Pattern 如何讓一個物件走訪（Traverses）一個物件結構，並對該結構中的每個節點執行相同的操作。不同的 Visitor 物件會定義不同的操作。

```csharp
using System;
using System.Collections.Generic;

namespace Visitor.Structural
{
    /// <summary>
    /// Visitor Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup structure

            ObjectStructure o = new ObjectStructure();
            o.Attach(new ConcreteElementA());
            o.Attach(new ConcreteElementB());

            // Create visitor objects

            ConcreteVisitor1 v1 = new ConcreteVisitor1();
            ConcreteVisitor2 v2 = new ConcreteVisitor2();

            // Structure accepting visitors

            o.Accept(v1);
            o.Accept(v2);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Visitor' abstract class
    /// </summary>

    public abstract class Visitor
    {
        public abstract void VisitConcreteElementA(
            ConcreteElementA concreteElementA);
        public abstract void VisitConcreteElementB(
            ConcreteElementB concreteElementB);
    }

    /// <summary>
    /// A 'ConcreteVisitor' class
    /// </summary>

    public class ConcreteVisitor1 : Visitor
    {
        public override void VisitConcreteElementA(
            ConcreteElementA concreteElementA)
        {
            Console.WriteLine("{0} visited by {1}",
                concreteElementA.GetType().Name, this.GetType().Name);
        }

        public override void VisitConcreteElementB(
            ConcreteElementB concreteElementB)
        {
            Console.WriteLine("{0} visited by {1}",
                concreteElementB.GetType().Name, this.GetType().Name);
        }
    }

    /// <summary>
    /// A 'ConcreteVisitor' class
    /// </summary>

    public class ConcreteVisitor2 : Visitor
    {
        public override void VisitConcreteElementA(
            ConcreteElementA concreteElementA)
        {
            Console.WriteLine("{0} visited by {1}",
                concreteElementA.GetType().Name, this.GetType().Name);
        }

        public override void VisitConcreteElementB(
            ConcreteElementB concreteElementB)
        {
            Console.WriteLine("{0} visited by {1}",
                concreteElementB.GetType().Name, this.GetType().Name);
        }
    }

    /// <summary>
    /// The 'Element' abstract class
    /// </summary>

    public abstract class Element
    {
        public abstract void Accept(Visitor visitor);
    }

    /// <summary>
    /// A 'ConcreteElement' class
    /// </summary>

    public class ConcreteElementA : Element
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitConcreteElementA(this);
        }

        public void OperationA()
        {
        }
    }

    /// <summary>
    /// A 'ConcreteElement' class
    /// </summary>

    public class ConcreteElementB : Element
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitConcreteElementB(this);
        }

        public void OperationB()
        {
        }
    }

    /// <summary>
    /// The 'ObjectStructure' class
    /// </summary>

    public class ObjectStructure
    {
        List<Element> elements = new List<Element>();

        public void Attach(Element element)
        {
            elements.Add(element);
        }

        public void Detach(Element element)
        {
            elements.Remove(element);
        }

        public void Accept(Visitor visitor)
        {
            foreach (Element element in elements)
            {
                element.Accept(visitor);
            }
        }
    }
}
```

### 輸出結果 (Output)

```text
ConcreteElementA visited by ConcreteVisitor1

  ConcreteElementB visited by ConcreteVisitor1

  ConcreteElementA visited by ConcreteVisitor2

  ConcreteElementB visited by ConcreteVisitor2
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Visitor Pattern 如何讓兩個物件走訪一份員工（`Employees`）清單，並對每位員工（`Employee`）執行相同的操作。這兩個 Visitor 物件定義了不同的操作——一個用來調整休假天數（`VacationVisitor`），另一個則用來調整薪資收入（`IncomeVisitor`）。

```csharp
using System;
using System.Collections.Generic;

namespace Visitor.RealWorld
{
    /// <summary>
    /// Visitor Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup employee collection

            Employees employee = new Employees();
            employee.Attach(new Clerk());
            employee.Attach(new Director());
            employee.Attach(new President());

            // Employees are 'visited'

            employee.Accept(new IncomeVisitor());
            employee.Accept(new VacationVisitor());

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Visitor' interface
    /// </summary>

    public interface IVisitor
    {
        void Visit(Element element);
    }

    /// <summary>
    /// A 'ConcreteVisitor' class
    /// </summary>

    public class IncomeVisitor : IVisitor
    {
        public void Visit(Element element)
        {
            Employee employee = element as Employee;

            // Provide 10% pay raise

            employee.Income *= 1.10;

            Console.WriteLine("{0} {1}'s new income: {2:C}",
                employee.GetType().Name, employee.Name,
                employee.Income);
        }
    }

    /// <summary>
    /// A 'ConcreteVisitor' class
    /// </summary>

    public class VacationVisitor : IVisitor
    {
        public void Visit(Element element)
        {
            Employee employee = element as Employee;

            // Provide 3 extra vacation days

            employee.VacationDays += 3;

            Console.WriteLine("{0} {1}'s new vacation days: {2}",
                employee.GetType().Name, employee.Name,
                employee.VacationDays);
        }
    }

    /// <summary>
    /// The 'Element' abstract class
    /// </summary>

    public abstract class Element
    {
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// The 'ConcreteElement' class
    /// </summary>

    public class Employee : Element
    {
        private string name;
        private double income;
        private int vacationDays;

        // Constructor

        public Employee(string name, double income,
            int vacationDays)
        {
            this.name = name;
            this.income = income;
            this.vacationDays = vacationDays;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double Income
        {
            get { return income; }
            set { income = value; }
        }

        public int VacationDays
        {
            get { return vacationDays; }
            set { vacationDays = value; }
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// The 'ObjectStructure' class
    /// </summary>

    public class Employees
    {
        private List<Employee> employees = new List<Employee>();

        public void Attach(Employee employee)
        {
            employees.Add(employee);
        }

        public void Detach(Employee employee)
        {
            employees.Remove(employee);
        }

        public void Accept(IVisitor visitor)
        {
            foreach (Employee employee in employees)
            {
                employee.Accept(visitor);
            }
            Console.WriteLine();
        }
    }

    // Three employee types

    public class Clerk : Employee
    {
        // Constructor

        public Clerk()
            : base("Kevin", 25000.0, 14)
        {
        }
    }

    public class Director : Employee
    {
        // Constructor
        public Director()
            : base("Elly", 35000.0, 16)
        {
        }
    }

    public class President : Employee
    {
        // Constructor
        public President()
            : base("Eric", 45000.0, 21)
        {
        }
    }
}
```

### 輸出結果 (Output)

```text
Clerk Kevin's new income: $27,500.00

  Director Elly's new income: $38,500.00

  President Eric's new income: $49,500.00

  

  Clerk Kevin's new vacation days: 17

  Director Elly's new vacation days: 19

  President Eric's new vacation days: 24
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的真實世界情境，但採用了現代化、內建的 .NET 功能（例如 Primary Constructors、泛型 `List<Employee>` 繼承以及 C# Pattern Matching `is Employee employee`）。這是一個優雅的 C# Visitor 解決方案。

```csharp
namespace Visitor.NetOptimized;

using static System.Console;
using System.Collections.Generic;

/// <summary>
/// Visitor Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Setup employee collection
        var employees = new Employees
        {
            new Clerk(),
            new Director(),
            new President()
        };

        // Employees are 'visited'
        employees.Accept(new IncomeVisitor());
        employees.Accept(new VacationVisitor());

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Visitor' interface
/// </summary>
public interface IVisitor
{
    void Visit(Element element);
}

/// <summary>
/// A 'ConcreteVisitor' class
/// </summary>
public class IncomeVisitor : IVisitor
{
    public void Visit(Element element)
    {
        if (element is Employee employee)
        {
            // Provide 10% pay raise
            employee.Income *= 1.10;

            WriteLine($"{employee.GetType().Name} {employee.Name}'s new income: {employee.Income:C}");
        }
    }
}

/// <summary>
/// A 'ConcreteVisitor' class
/// </summary>
public class VacationVisitor : IVisitor
{
    public void Visit(Element element)
    {
        if (element is Employee employee)
        {
            // Provide 3 extra vacation days
            employee.VacationDays += 3;

            WriteLine($"{employee.GetType().Name} {employee.Name}'s new vacation days: {employee.VacationDays}");
        }
    }
}

/// <summary>
/// The 'Element' abstract class
/// </summary>
public abstract class Element
{
    public abstract void Accept(IVisitor visitor);
}

/// <summary>
/// The 'ConcreteElement' class
/// </summary>
public class Employee(string name, double income, int vacationDays) : Element
{
    public string Name { get; set; } = name;
    public double Income { get; set; } = income;
    public int VacationDays { get; set; } = vacationDays;

    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}

/// <summary>
/// The 'ObjectStructure' class
/// </summary>
public class Employees : List<Employee>
{
    public void Attach(Employee employee) => Add(employee);

    public void Detach(Employee employee) => Remove(employee);

    public void Accept(IVisitor visitor)
    {
        ForEach(employee => employee.Accept(visitor));
        WriteLine();
    }
}

// Three employee types

public class Clerk() : Employee("Kevin", 25000.0, 14)
{
}

public class Director() : Employee("Elly", 35000.0, 16)
{
}

public class President() : Employee("Eric", 45000.0, 21)
{
}
```

### 輸出結果 (Output)

```text
Clerk Kevin's new income: $27,500.00
Director Elly's new income: $38,500.00
President Eric's new income: $49,500.00

Clerk Kevin's new vacation days: 17
Director Elly's new vacation days: 19
President Eric's new vacation days: 24
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **傳統 OOP Visitor 的雙重分派（Double Dispatch）樣板過於笨重**：在傳統 C# 中，為了不修改 `Employee` 類別而新增操作，必須定義 `IVisitor`、`Element.Accept(IVisitor)`，並在轉型 `element as Employee` 時喪失編譯期型別檢查。
- **C# 15 `union`（聯合型別）與 `closed`（封閉階層）的革命性突破**：C# 15 原生引入了 **`union`（Union Types）** 與 **`closed` 修飾詞**！這正是函數式語言（如 F# / Rust）用來徹底取代 GoF Visitor Pattern 的核心武器——當型別集合是封閉且已知的，透過 **`switch` 窮舉式模式比對（Exhaustive Pattern Matching）**，不僅不需要寫任何 `Accept(IVisitor)` 樣板，還能讓不同職級（如 `Clerk` 加薪 10%、`Director` 加薪 12%、`President` 加薪 15%）擁有各自專屬的處理邏輯，且一旦新增職級，編譯器會自動強制所有 Visitor 補齊對應分支！

### C# 15 / .NET 11 優化重點

- **C# 15 `union`（Union Types）與 `closed` 階層**：展示 C# 15 最具代表性的新功能，完全免除 `IVisitor.Visit` 與 `Element.Accept` 的雙重分派樣板。
- **編譯期窮舉式模式比對（Exhaustive Pattern Matching）**：不需要 `_ =>` 預設分支，編譯器即可保證所有員工型別皆被正確處理。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Visitor.ModernNet11;

using static System.Console;

public static class Program
{
    public static void Main()
    {
        List<Employee> employees =
        [
            with(capacity: 3),
            new Clerk("Kevin", 25_000.0, 14),
            new Director("Elly", 35_000.0, 16),
            new President("Eric", 45_000.0, 21)
        ];

        // 直接套用不同的 Visitor 操作，無須任何 Accept(IVisitor) 樣板程式碼
        employees.ForEach(EmployeeVisitors.ApplyIncomeRaise);
        WriteLine();
        employees.ForEach(EmployeeVisitors.ApplyExtraVacation);
    }
}

// C# 15 closed 修飾詞（或使用 C# 15 `public union Employee(Clerk, Director, President);`）：
// 宣告封閉的型別階層，讓編譯器對 switch 進行窮舉性檢查 (Exhaustiveness Checking)
public closed abstract class Employee(string name, double income, int vacationDays)
{
    public string Name { get; set; } = name;
    public double Income { get; set; } = income;
    public int VacationDays { get; set; } = vacationDays;
}

public sealed class Clerk(string name, double income, int vacationDays) : Employee(name, income, vacationDays);
public sealed class Director(string name, double income, int vacationDays) : Employee(name, income, vacationDays);
public sealed class President(string name, double income, int vacationDays) : Employee(name, income, vacationDays);

/// <summary>
/// 利用 C# 15 Exhaustive Pattern Matching 實作 Visitor：
/// 不僅能依具體型別執行差異化邏輯，若未來新增第 4 種 Employee 子類別，編譯器會直接提示所有未更新的 switch！
/// </summary>
public static class EmployeeVisitors
{
    public static void ApplyIncomeRaise(Employee employee)
    {
        // 由於 Employee 是 closed 階層，編譯器知道只有這 3 種子類別，無須寫 _ => 分支！
        employee.Income *= employee switch
        {
            Clerk => 1.10,
            Director => 1.10,
            President => 1.10
        };

        WriteLine($"{employee.GetType().Name} {employee.Name}'s new income: {employee.Income:C}");
    }

    public static void ApplyExtraVacation(Employee employee)
    {
        employee.VacationDays += employee switch
        {
            Clerk => 3,
            Director => 3,
            President => 3
        };

        WriteLine($"{employee.GetType().Name} {employee.Name}'s new vacation days: {employee.VacationDays}");
    }
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Template Method](TemplateMethod.md) | [🏠 C# Design Patterns 總覽](Index.md) | [返回 C# Design Patterns 總覽 (Index) →](Index.md) |

# C# Visitor Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：低 (Low)  
> **原文連結**：[https://www.dofactory.com/net/visitor-design-pattern](https://www.dofactory.com/net/visitor-design-pattern)

---

## 定義與概述 (Definition)

**Visitor** Design Pattern 代表一個要在某個物件結構（Object Structure）之各個元素上執行的操作。此 Pattern 讓你可以在不修改各元素所屬類別的情況下，定義一個新的操作。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural 與 Real-world 兩種原版程式碼，以及一份依其風格補上的 .NET Optimized 對照版本。

---

## 問題情境 (Problem)

公司的員工分成三種職級：`Clerk`、`Director`、`President`。人資部門要對所有員工執行「調薪」與「增加休假」，之後還會有更多操作（計算獎金、產生報表），而且每種職級的規則都不同。

- **做法一：把每個操作都寫成員工類別的方法。** 每新增一個操作就要修改三個類別，薪資、休假、報表的邏輯全部擠在員工類別裡。
- **做法二：傳統的 Visitor。** 每個員工類別實作 `Accept(IVisitor)`，每個操作寫成一個 Visitor 類別，為每種職級各提供一個 `Visit` 方法。這樣新增操作不必修改員工類別，但需要相當多固定的程式碼（見附錄的原版範例）。
- **做法三：封閉的型別階層加上 `switch`。** 這是本篇採用的寫法，也是 C# 15 新增的 `closed` 與 `union` 最適合的用途。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Visitor UML Class Diagram](images/visitor.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

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

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Visitor](samples/Visitor)  
> **執行方式**：`dotnet run --project samples/Visitor`

**角色對照**：Element = `Employee`；ConcreteElement = `Clerk`、`Director`、`President`；Visitor = `Payroll` 的每一個方法；ObjectStructure = `employees` 陣列。

<!-- sample:Visitor/Program.cs -->
```csharp
// Visitor：在不修改既有型別的前提下，替「一組固定的型別」新增操作。
// C# 15 可以用 closed 階層（或 union）加上 switch，取代傳統的 Accept / Visit 雙重分派。

Employee[] employees =
[
    new Clerk("Kevin", 25_000m, VacationDays: 14),
    new Director("Elly", 35_000m, VacationDays: 16),
    new President("Eric", 45_000m, VacationDays: 21),
];

// 兩個「操作」都寫在 Employee 階層之外，新增第三個操作也不必修改任何 Employee 類別
foreach (Employee employee in employees)
{
    Employee raised = Payroll.RaiseIncome(employee);
    Console.WriteLine($"{employee.GetType().Name} {employee.Name}: income ${employee.Income:N2} -> ${raised.Income:N2}");
}

Console.WriteLine();

foreach (Employee employee in employees)
{
    Employee rested = Payroll.AddVacation(employee);
    Console.WriteLine($"{employee.GetType().Name} {employee.Name}: vacation {employee.VacationDays} -> {rested.VacationDays} days");
}

Console.WriteLine();

// union：型別之間沒有共同的基底類別時，用 union 宣告「一定是這幾種之一」
Bonus[] bonuses = [new CashBonus(1_500m), new StockBonus("MSFT", 10)];
foreach (Bonus bonus in bonuses)
{
    Console.WriteLine($"Bonus: {Payroll.Describe(bonus)}");
}

// ---- Element：closed 表示「直接子類別只能宣告在同一個組件」，
// 所以編譯器知道 Employee 只可能是下面這三種。closed 類別本身一律是抽象的。
public closed record Employee(string Name, decimal Income, int VacationDays);

// ---- ConcreteElement ----
public sealed record Clerk(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);
public sealed record Director(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);
public sealed record President(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);

public sealed record CashBonus(decimal Amount);
public sealed record StockBonus(string Symbol, int Shares);
public union Bonus(CashBonus, StockBonus);

// ---- Visitor：每個操作是一個方法，用 switch 依實際型別決定行為 ----
public static class Payroll
{
    public static Employee RaiseIncome(Employee employee)
    {
        // 三種型別都列出來了，所以不需要 _ => 分支。
        // 日後新增第四種 Employee 而這裡忘了處理，編譯器會提出警告。
        decimal rate = employee switch
        {
            Clerk => 0.10m,
            Director => 0.12m,
            President => 0.15m,
        };

        // with：複製一份並修改指定屬性，原本的物件不會被改動
        return employee with { Income = employee.Income * (1 + rate) };
    }

    public static Employee AddVacation(Employee employee)
    {
        int extraDays = employee switch
        {
            Clerk => 3,
            Director => 5,
            President => 7,
        };

        return employee with { VacationDays = employee.VacationDays + extraDays };
    }

    public static string Describe(Bonus bonus) => bonus switch
    {
        CashBonus cash => $"cash ${cash.Amount:N2}",
        StockBonus stock => $"{stock.Shares} shares of {stock.Symbol}",
    };
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Visitor -->
```text
Clerk Kevin: income $25,000.00 -> $27,500.00
Director Elly: income $35,000.00 -> $39,200.00
President Eric: income $45,000.00 -> $51,750.00

Clerk Kevin: vacation 14 -> 17 days
Director Elly: vacation 16 -> 21 days
President Eric: vacation 21 -> 28 days

Bonus: cash $1,500.00
Bonus: 10 shares of MSFT
```
<!-- /output -->

### 重點解說

- **`closed`（C# 15）**：標記為 `closed` 的類別，直接衍生的類別只能宣告在同一個組件，所以編譯器知道 `Employee` 只可能是這三種。`closed` 類別本身一律是抽象的，不能再加上 `abstract`。
- **窮舉檢查**：`switch` 列出全部三種型別，所以不需要 `_ =>` 分支。日後新增第四種職級而忘了處理，編譯器會提出警告；本專案把警告視為錯誤，所以會直接建置失敗。
- **`with`**：`employee with { Income = ... }` 會回傳一份修改過的複本，原本的物件不變。因此輸出可以同時顯示調整前後的數值。
- **`union`（C# 15）**：`CashBonus` 與 `StockBonus` 沒有共同的基底類別。`union Bonus(CashBonus, StockBonus)` 宣告一個「一定是這兩種之一」的型別，同樣可以用 `switch` 做窮舉檢查。
- **這種寫法的取捨**：新增「操作」很容易（多寫一個方法），新增「型別」則要修改每一個 `switch`（編譯器會指出所有需要修改的地方）。把操作寫成類別的虛擬方法則正好相反。

---

## 與 Dofactory 原版的差異

- 原版的 Visitor 只有一個 `Visit(Element element)` 方法，在裡面把參數轉型成 `Employee` 之後套用同一條規則，所以三種職級的調薪幅度都是 10%、休假都增加 3 天。它並沒有「依型別執行不同的邏輯」，而這正是 Visitor 存在的理由。本版的每種職級都有自己的規則。
- 原版需要 `IVisitor`、`Element.Accept`、`Employees.Accept` 等固定的程式碼。本版以 `switch` 取代。
- 原版直接修改員工物件。本版回傳新的物件，原始資料不變。
- 原版以 `double` 表示薪資。本版改用 `decimal`。

---

## .NET 中的對應 (In .NET)

- **`System.Linq.Expressions.ExpressionVisitor`**：傳統 Visitor 的代表，EF Core 用它把 LINQ 查詢轉換成 SQL。
- **Roslyn**：`CSharpSyntaxVisitor` 與 `CSharpSyntaxWalker` 用來走訪語法樹，分析器與來源產生器都以此為基礎。
- **型別模式比對**：`switch` 從 C# 7 開始可以依型別分支；C# 15 的 `closed` 與 `union` 補上了「是否涵蓋所有情況」的編譯期檢查。

---

## 何時該用、何時不該用

**適合使用**

- 型別的種類固定，但操作會持續增加，例如語法樹、文件節點、幾何圖形。
- 一個操作需要跨多種型別累積結果。

**不適合使用**

- 型別經常新增：改用虛擬方法或介面，新增型別時才不必修改既有程式碼。
- 只有一兩個操作：直接寫成虛擬方法。
- 型別階層不是你能控制的（無法標記 `closed`）：`switch` 仍然可以用，但必須保留 `_ =>` 分支。

---

## 練習題

1. 新增 `Manager` 職級，只新增 `record`，不修改 `Payroll`。建置專案，閱讀編譯器的訊息。
2. 新增第三個操作 `AnnualCost(Employee)`，回傳年薪加上依職級而定的津貼。確認不需要修改任何員工類別。
3. 把 `Employee` 的 `closed` 改成 `abstract` 再建置，觀察 `switch` 的訊息有什麼變化。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的 Structural 與 Real-world 兩種原版程式碼，以及一份依其風格補上的 .NET Optimized 對照版本，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

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

**輸出結果 (Output)**

```text
ConcreteElementA visited by ConcreteVisitor1
ConcreteElementB visited by ConcreteVisitor1
ConcreteElementA visited by ConcreteVisitor2
ConcreteElementB visited by ConcreteVisitor2
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

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

**輸出結果 (Output)**

```text
Clerk Kevin's new income: $27,500.00
Director Elly's new income: $38,500.00
President Eric's new income: $49,500.00

Clerk Kevin's new vacation days: 17
Director Elly's new vacation days: 19
President Eric's new vacation days: 24
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

> **註**：Dofactory 網站並未公開 Visitor 的 .NET Optimized 程式碼。以下是依該網站同系列範例的風格（繼承 `List<Employee>`、Primary Constructor、`is Employee` 轉型）補上的對照版本，並非 Dofactory 的原始內容。

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的真實世界情境，但採用了現代化、內建的 .NET 功能（例如 Primary Constructors、泛型 `List<Employee>` 繼承以及 C# Pattern Matching `is Employee employee`）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

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

**輸出結果 (Output)**

```text
Clerk Kevin's new income: $27,500.00
Director Elly's new income: $38,500.00
President Eric's new income: $49,500.00

Clerk Kevin's new vacation days: 17
Director Elly's new vacation days: 19
President Eric's new vacation days: 24
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Template Method](TemplateMethod.md) | [🏠 C# Design Patterns 總覽](README.md) | [返回 C# Design Patterns 總覽 (README) →](README.md) |

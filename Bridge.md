# C# Bridge Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：中低 (Medium-Low)  
> **原文連結**：[https://www.dofactory.com/net/bridge-design-pattern](https://www.dofactory.com/net/bridge-design-pattern)

---

## 定義與概述 (Definition)

**Bridge** Design Pattern 將抽象（Abstraction）與其實作（Implementation）解耦（Decouples），使這兩者可以獨立地變化。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

客戶報表有兩種呈現方式（一般、加上編號），資料有兩種來源（記憶體、CSV）。如果只用繼承來表達所有組合，類別會是：

```text
InMemoryCustomerReport    InMemoryNumberedCustomerReport
CsvCustomerReport         CsvNumberedCustomerReport
```

2 × 2 就需要 4 個類別。再加一種來源與一種呈現方式就變成 3 × 3 = 9 個，而且每個類別都混雜著「怎麼取資料」與「怎麼呈現」兩種程式碼。

Bridge 把這兩個會各自增加的面向拆成兩個獨立的階層，再用「一個物件持有另一個物件」把它們接起來。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Bridge UML Class Diagram](images/bridge.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Abstraction** (`BusinessObject`)
  - 定義抽象概念的介面。
  - 維護一個指向 `Implementor` 型別物件的參考（Reference）。
- **RefinedAbstraction** (`CustomersBusinessObject`)
  - 擴充由 `Abstraction` 所定義的介面。
- **Implementor** (`DataObject`)
  - 定義實作類別（Implementation Classes）的介面。這個介面不需要與 `Abstraction` 的介面完全對應；事實上，這兩個介面可以完全不同。通常 `Implementor` 介面只提供基本（Primitive）操作，而 `Abstraction` 則根據這些基本操作來定義更高階的操作。
- **ConcreteImplementor** (`CustomersDataObject`)
  - 實作 `Implementor` 介面並定義其具體實作。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Bridge](samples/Bridge)  
> **執行方式**：`dotnet run --project samples/Bridge`

**角色對照**：Abstraction = `CustomerReport`；RefinedAbstraction = `NumberedCustomerReport`；Implementor = `ICustomerSource`；ConcreteImplementor = `InMemoryCustomerSource`、`CsvCustomerSource`。

<!-- sample:Bridge/Program.cs -->
```csharp
// Bridge：把「要做什麼」（抽象）和「怎麼做」（實作）拆成兩個各自獨立發展的階層。

ICustomerSource chicago = new InMemoryCustomerSource("Chicago", ["Jim Jones", "Samual Jackson", "Allen Good"]);
ICustomerSource taipei = new CsvCustomerSource("Taipei", "Ann Stills;Lisa Giolani");

// 2 種報表 × 2 種資料來源 = 4 種組合，但只需要 2 + 2 個類別
new CustomerReport(chicago).Print();
new NumberedCustomerReport(chicago).Print();
new CustomerReport(taipei).Print();
new NumberedCustomerReport(taipei).Print();

// ---- Abstraction：報表「要呈現什麼」，透過介面使用資料來源 ----
public class CustomerReport(ICustomerSource source)
{
    public void Print()
    {
        Console.WriteLine($"Customer Group: {source.Group}");

        int index = 0;
        foreach (string customer in source.GetCustomers())
        {
            Console.WriteLine(FormatLine(++index, customer));
        }

        Console.WriteLine();
    }

    protected virtual string FormatLine(int index, string customer) => $"  {customer}";
}

// ---- RefinedAbstraction：改變呈現方式，完全不必知道資料從哪裡來 ----
public sealed class NumberedCustomerReport(ICustomerSource source) : CustomerReport(source)
{
    protected override string FormatLine(int index, string customer) => $"  {index}. {customer}";
}

// ---- Implementor：資料「怎麼取得」 ----
public interface ICustomerSource
{
    string Group { get; }
    IEnumerable<string> GetCustomers();
}

// ---- ConcreteImplementor ----
public sealed class InMemoryCustomerSource(string group, IReadOnlyList<string> customers) : ICustomerSource
{
    public string Group => group;
    public IEnumerable<string> GetCustomers() => customers;
}

public sealed class CsvCustomerSource(string group, string csv) : ICustomerSource
{
    public string Group => group;
    public IEnumerable<string> GetCustomers() => csv.Split(';', StringSplitOptions.TrimEntries);
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Bridge -->
```text
Customer Group: Chicago
  Jim Jones
  Samual Jackson
  Allen Good

Customer Group: Chicago
  1. Jim Jones
  2. Samual Jackson
  3. Allen Good

Customer Group: Taipei
  Ann Stills
  Lisa Giolani

Customer Group: Taipei
  1. Ann Stills
  2. Lisa Giolani
```
<!-- /output -->

### 重點解說

- **「橋」就是建構函式的那個參數**：`CustomerReport(ICustomerSource source)`。報表透過介面使用資料來源，兩邊互不知道對方的具體類別。
- **兩個階層各自擴充**：新增資料來源只需要實作 `ICustomerSource`；新增呈現方式只需要繼承 `CustomerReport`。
- **2 + 2 個類別涵蓋 4 種組合**：輸出的四份報表來自同樣四個類別的不同搭配。
- **`protected virtual FormatLine`**：子類別只覆寫「一行怎麼顯示」，列印的流程留在基底類別。

---

## 與 Dofactory 原版的差異

- 原版只有一個 ConcreteImplementor（`CustomersData`）與一個 RefinedAbstraction，看不出「兩個面向各自變化」的效果。本版兩邊各提供兩個。
- 原版的 Implementor 混合了移動游標（`NextRecord`、`PriorRecord`）、新增刪除與顯示（`ShowRecord`）等職責。本版的 Implementor 只負責提供資料，顯示屬於 Abstraction 的工作。

---

## .NET 中的對應 (In .NET)

- **`ILogger` 與 `ILoggerProvider`**：應用程式只使用 `ILogger`（抽象），實際寫到 Console、檔案或雲端服務則由各個提供者（實作）負責，兩邊可以各自增加。
- **ADO.NET**：`DbConnection`、`DbCommand` 是抽象，SQL Server、PostgreSQL、SQLite 的提供者是實作。
- **EF Core**：`DbContext` 與資料庫提供者（`UseSqlServer`、`UseNpgsql`）。

---

## 何時該用、何時不該用

**適合使用**

- 同一個概念有兩個會各自增加的面向。
- 想在執行期更換實作（例如依設定切換資料來源）。

**不適合使用**

- 只有一個面向會變：一般的「介面加實作」就夠了。
- 目前只有一種實作，也看不到第二種：先不要拆。

---

## 練習題

1. 新增第三種資料來源 `JsonCustomerSource`，用 `System.Text.Json` 解析 `["Ann","Bob"]` 這樣的字串。不要修改任何報表類別。
2. 新增第三種報表 `UppercaseCustomerReport`，把姓名轉成大寫。不要修改任何資料來源類別。
3. 如果只用繼承來表達 3 × 3 種組合，總共需要幾個類別？用 Bridge 需要幾個？

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Bridge Pattern 如何將介面與其實作分離（解耦）。實作可以獨立演進，而無須修改使用該物件抽象概念的用戶端（Clients）。

```csharp
using System;

namespace Bridge.Structural
{
    /// <summary>
    /// Bridge Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            Abstraction ab = new RefinedAbstraction();

            // Set implementation and call

            ab.Implementor = new ConcreteImplementorA();
            ab.Operation();

            // Change implemention and call

            ab.Implementor = new ConcreteImplementorB();
            ab.Operation();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Abstraction' class
    /// </summary>

    public class Abstraction
    {
        protected Implementor implementor;

        public Implementor Implementor
        {
            set { implementor = value; }
        }

        public virtual void Operation()
        {
            implementor.Operation();
        }
    }

    /// <summary>
    /// The 'Implementor' abstract class
    /// </summary>

    public abstract class Implementor
    {
        public abstract void Operation();
    }

    /// <summary>
    /// The 'RefinedAbstraction' class
    /// </summary>

    public class RefinedAbstraction : Abstraction
    {
        public override void Operation()
        {
            implementor.Operation();
        }
    }

    /// <summary>
    /// The 'ConcreteImplementorA' class
    /// </summary>

    public class ConcreteImplementorA : Implementor
    {
        public override void Operation()
        {
            Console.WriteLine("ConcreteImplementorA Operation");
        }
    }

    /// <summary>
    /// The 'ConcreteImplementorB' class
    /// </summary>

    public class ConcreteImplementorB : Implementor
    {
        public override void Operation()
        {
            Console.WriteLine("ConcreteImplementorB Operation");
        }
    }
}
```

**輸出結果 (Output)**

```text
ConcreteImplementorA Operation
ConcreteImplementorB Operation
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Bridge Pattern 如何將 `BusinessObject`（商業物件）抽象概念與 `DataObject`（資料物件）中的實作解耦。`DataObject` 的實作可以動態演進，而不需要變更任何用戶端程式碼。

```csharp
using System;
using System.Collections.Generic;

namespace Bridge.RealWorld
{
    /// <summary>
    /// Bridge Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create RefinedAbstraction

            var customers = new Customers();

            // Set ConcreteImplementor

            customers.Data = new CustomersData("Chicago");

            // Exercise the bridge

            customers.Show();
            customers.Next();
            customers.Show();
            customers.Next();
            customers.Show();
            customers.Add("Henry Velasquez");

            customers.ShowAll();

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// The 'Abstraction' class
    /// </summary>

    public class CustomersBase
    {
        private DataObject dataObject;

        public DataObject Data
        {
            set { dataObject = value; }
            get { return dataObject; }
        }

        public virtual void Next()
        {
            dataObject.NextRecord();
        }

        public virtual void Prior()
        {
            dataObject.PriorRecord();
        }

        public virtual void Add(string customer)
        {
            dataObject.AddRecord(customer);
        }

        public virtual void Delete(string customer)
        {
            dataObject.DeleteRecord(customer);
        }

        public virtual void Show()
        {
            dataObject.ShowRecord();
        }

        public virtual void ShowAll()
        {
            dataObject.ShowAllRecords();
        }
    }

    /// <summary>
    /// The 'RefinedAbstraction' class
    /// </summary>

    public class Customers : CustomersBase
    {
        public override void ShowAll()
        {
            // Add separator lines

            Console.WriteLine();
            Console.WriteLine("------------------------");
            base.ShowAll();
            Console.WriteLine("------------------------");
        }
    }

    /// <summary>
    /// The 'Implementor' abstract class
    /// </summary>

    public abstract class DataObject
    {
        public abstract void NextRecord();
        public abstract void PriorRecord();
        public abstract void AddRecord(string name);
        public abstract void DeleteRecord(string name);
        public abstract string GetCurrentRecord();
        public abstract void ShowRecord();
        public abstract void ShowAllRecords();
    }

    /// <summary>
    /// The 'ConcreteImplementor' class
    /// </summary>

    public class CustomersData : DataObject
    {
        private readonly List<string> customers = new List<string>();
        private int current = 0;
        private string city;

        public CustomersData(string city)
        {
            this.city = city;

            // Loaded from a database 

            customers.Add("Jim Jones");
            customers.Add("Samual Jackson");
            customers.Add("Allen Good");
            customers.Add("Ann Stills");
            customers.Add("Lisa Giolani");
        }

        public override void NextRecord()
        {
            if (current <= customers.Count - 1)
            {
                current++;
            }
        }

        public override void PriorRecord()
        {
            if (current > 0)
            {
                current--;
            }
        }

        public override void AddRecord(string customer)
        {
            customers.Add(customer);
        }

        public override void DeleteRecord(string customer)
        {
            customers.Remove(customer);
        }

        public override string GetCurrentRecord()
        {
            return customers[current];
        }

        public override void ShowRecord()
        {
            Console.WriteLine(customers[current]);
        }

        public override void ShowAllRecords()
        {
            Console.WriteLine("Customer City: " + city);

            foreach (string customer in customers)
            {
                Console.WriteLine(" " + customer);
            }
        }
    }
}
```

**輸出結果 (Output)**

```text
Jim Jones
Samual Jackson
Allen Good

------------------------
Customer City: Chicago
 Jim Jones
 Samual Jackson
 Allen Good
 Ann Stills
 Lisa Giolani
 Henry Velasquez
------------------------
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors 與介面 `IDataObject<T>`）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Bridge.NetOptimized;

using static System.Console;

/// <summary>
/// Bridge Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create RefinedAbstraction and
        // set ConcreteImplementor
        var customers = new Customers(
            new CustomersData("Chicago"));

        // Exercise the bridge
        customers.Show();
        customers.Next();
        customers.Show();
        customers.Next();
        customers.Show();

        customers.Add("Henry Velasquez");
        customers.ShowAll();

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Abstraction' class
/// </summary>
public class CustomersBase(IDataObject<string> dataObject)
{
    public virtual void Next() => dataObject.NextRecord();
    public virtual void Prior() => dataObject.PriorRecord();
    public virtual void Add(string name) => dataObject.AddRecord(name);
    public virtual void Delete(string name) => dataObject.DeleteRecord(name);
    public virtual void Show() => dataObject.ShowRecord();
    public virtual void ShowAll() => dataObject.ShowAllRecords();
}

/// <summary>
/// The 'RefinedAbstraction' class
/// </summary>
public class Customers(IDataObject<string> dataObject) : CustomersBase(dataObject)
{
    public override void ShowAll()
    {
        // Add separator lines
        WriteLine();
        WriteLine("------------------------");
        base.ShowAll();
        WriteLine("------------------------");
    }
}

/// <summary>
/// The 'Implementor' interface
/// </summary>
public interface IDataObject<T>
{
    void NextRecord();
    void PriorRecord();
    void AddRecord(T t);
    void DeleteRecord(T t);
    T GetCurrentRecord();
    void ShowRecord();
    void ShowAllRecords();
}

/// <summary>
/// The 'ConcreteImplementor' class
/// </summary>
public class CustomersData : IDataObject<string>
{
    private readonly string city;
    private readonly List<string> customers;
    private int current = 0;

    // Constructor
    public CustomersData(string city)
    {
        this.city = city;

        // Simulate loading from database
        customers =
              ["Jim Jones", "Samual Jackson", "Allan Good",
                "Ann Stills", "Lisa Giolani" ];
    }

    public void NextRecord()
    {
        if (current <= customers.Count - 1)
        {
            current++;
        }
    }

    public void PriorRecord()
    {
        if (current > 0)
        {
            current--;
        }
    }

    public void AddRecord(string customer) =>
        customers.Add(customer);

    public void DeleteRecord(string customer) =>
        customers.Remove(customer);

    public string GetCurrentRecord() =>
        customers[current];

    public void ShowRecord() =>
        WriteLine(customers[current]);

    public void ShowAllRecords()
    {
        WriteLine("Customer Group: " + city);
        customers.ForEach(customer =>
            WriteLine(" " + customer));
    }
}
```

**輸出結果 (Output)**

```text
Jim Jones
Samual Jackson
Allan Good

------------------------
Customer Group: Chicago
 Jim Jones
 Samual Jackson
 Allan Good
 Ann Stills
 Lisa Giolani
 Henry Velasquez
------------------------
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Adapter](Adapter.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Composite →](Composite.md) |

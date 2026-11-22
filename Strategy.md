# C# Strategy Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/strategy-design-pattern](https://www.dofactory.com/net/strategy-design-pattern)

---

## 定義與概述 (Definition)

**Strategy** Design Pattern 定義了一系列的演算法（Algorithms），將每一個演算法封裝起來，並使它們可以互相替換（Interchangeable）。此 Pattern 讓演算法可以獨立於使用它的用戶端（Clients）而變化。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

學生名單需要用不同的方式排序後列印。把所有排序方式寫在同一個方法裡：

```csharp
// 示意：排序方式寫死在使用它的類別裡，每新增一種就要修改這個方法
public void Print(string sortBy)
{
    if (sortBy == "name") { /* 依姓名排序 */ }
    else if (sortBy == "score") { /* 依分數排序 */ }

    /* 列印 */
}
```

- 每新增一種排序方式，就要修改 `Print`。
- 排序的邏輯無法單獨測試，也無法在別的地方重複使用。

Strategy 把「可以互換的做法」抽出來成為獨立的物件，使用它的類別只知道「有一個做法可以呼叫」，不知道也不在乎是哪一種。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Strategy UML Class Diagram](images/strategy.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Strategy** (`SortStrategy`)
  - 宣告所有支援之演算法的共同介面。`Context` 會使用這個介面來呼叫由 `ConcreteStrategy` 所定義的演算法。
- **ConcreteStrategy** (`QuickSort`, `ShellSort`, `MergeSort`)
  - 使用 `Strategy` 介面來實作具體的演算法。
- **Context** (`SortedList`)
  - 以一個 `ConcreteStrategy` 物件來進行組態設定。
  - 維護一個指向 `Strategy` 物件的參考。
  - 可以定義一個讓 `Strategy` 存取其資料的介面。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Strategy](samples/Strategy)  
> **執行方式**：`dotnet run --project samples/Strategy`

**角色對照**：Strategy = `ISortStrategy`（或 `Func<IEnumerable<Student>, IEnumerable<Student>>` 委派）；ConcreteStrategy = `ByName`、`ByScoreDescending`、lambda；Context = `Roster`。

<!-- sample:Strategy/Program.cs -->
```csharp
// Strategy：把「可以互換的做法」抽出來，讓使用它的程式碼不必因為換做法而修改。

Student[] students =
[
    new("Samual", Id: 154, Score: 82),
    new("Jimmy", Id: 487, Score: 91),
    new("Sandra", Id: 655, Score: 77),
    new("Vivek", Id: 133, Score: 91),
    new("Anna", Id: 760, Score: 68),
];

var roster = new Roster(students);

// 做法一：策略是實作介面的物件
roster.Print(new ByName());
roster.Print(new ByScoreDescending());

// 做法二：策略只有一個方法時，直接傳入委派（lambda）就是策略
roster.Print("By id", list => list.OrderBy(student => student.Id));

public sealed record Student(string Name, int Id, int Score);

// ---- Strategy ----
public interface ISortStrategy
{
    string Name { get; }
    IEnumerable<Student> Sort(IEnumerable<Student> students);
}

// ---- ConcreteStrategy ----
public sealed class ByName : ISortStrategy
{
    public string Name => "By name";

    public IEnumerable<Student> Sort(IEnumerable<Student> students) =>
        students.OrderBy(student => student.Name, StringComparer.Ordinal);
}

public sealed class ByScoreDescending : ISortStrategy
{
    public string Name => "By score (high to low)";

    // 同分時再依姓名排序，結果才會固定
    public IEnumerable<Student> Sort(IEnumerable<Student> students) =>
        students.OrderByDescending(student => student.Score)
                .ThenBy(student => student.Name, StringComparer.Ordinal);
}

// ---- Context：只知道「有一個排序策略」，不知道也不在乎是哪一種 ----
public sealed class Roster(IReadOnlyList<Student> students)
{
    public void Print(ISortStrategy strategy) => Print(strategy.Name, strategy.Sort);

    public void Print(string title, Func<IEnumerable<Student>, IEnumerable<Student>> sort)
    {
        Console.WriteLine($"{title}:");
        foreach (Student student in sort(students))
        {
            Console.WriteLine($"  {student.Name,-7} id={student.Id}  score={student.Score}");
        }
    }
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Strategy -->
```text
By name:
  Anna    id=760  score=68
  Jimmy   id=487  score=91
  Samual  id=154  score=82
  Sandra  id=655  score=77
  Vivek   id=133  score=91
By score (high to low):
  Jimmy   id=487  score=91
  Vivek   id=133  score=91
  Samual  id=154  score=82
  Sandra  id=655  score=77
  Anna    id=760  score=68
By id:
  Vivek   id=133  score=91
  Samual  id=154  score=82
  Jimmy   id=487  score=91
  Sandra  id=655  score=77
  Anna    id=760  score=68
```
<!-- /output -->

### 重點解說

- **`Roster` 不知道有哪些排序方式**：新增一種排序方式不需要修改它。
- **兩種寫法其實是同一件事**：接收介面的 `Print` 內部直接轉呼叫接收委派的 `Print`。`strategy.Sort` 這種只寫方法名稱、不加括號的寫法，會把方法轉成委派。
- **什麼時候用介面**：策略需要名稱、有多個方法、或要由 DI 容器注入其他相依物件時。策略只是一個簡單的函式時，傳 lambda 最省事。
- **排序結果要固定**：`ByScoreDescending` 在同分時再依姓名排序，輸出才不會因為原始順序不同而改變。
- **策略不修改輸入**：`OrderBy` 會回傳新的序列，原本的 `students` 順序不變。

---

## 與 Dofactory 原版的差異

- 原版的 `QuickSort`、`ShellSort`、`MergeSort` 三個策略中，只有 `QuickSort` 真的呼叫了 `list.Sort()`，另外兩個只印出一行文字（程式碼裡標註 `not-implemented`），完全看不出更換策略的效果。本版的每個策略都會產生不同的結果。
- 原版把 Context 命名為 `SortedList`，與 `System.Collections.SortedList` 同名。本版改名為 `Roster`。
- 原版的策略直接修改傳入的 `List`。本版回傳新的序列，不改動原始資料。
- 本版補充了以委派作為策略的寫法。

---

## .NET 中的對應 (In .NET)

- **`IComparer<T>` 與 `Comparison<T>`**：`List<T>.Sort(comparer)`、`OrderBy(keySelector, comparer)`。
- **`StringComparer`、`IEqualityComparer<T>`**：告訴 `Dictionary`、`HashSet` 如何比較鍵值。
- **LINQ**：`Where`、`Select`、`OrderBy` 的每一個 lambda 參數，都是傳進去的策略。
- **DI**：以介面注入策略；需要多個策略並存時，使用 Keyed Services 依名稱取用。

---

## 何時該用、何時不該用

**適合使用**

- 同一件事有多種做法，而且需要能夠替換，例如計價規則、驗證方式、輸出格式。
- 想消除依類型分支的 `if` 或 `switch`。
- 想單獨測試每一種做法。

**不適合使用**

- 只有一種做法，也看不到第二種：先不要抽出來。
- 策略只有一行：傳 lambda，不必為它建立介面與類別。

---

## 練習題

1. 新增一個策略：依分數由高到低，只取前三名。
2. 修改 `Roster`，在建構函式接收一個預設策略；`Print()` 不帶參數時使用它。
3. 用 `IComparer<Student>` 重寫「依姓名排序」。`IComparer<T>` 與本範例的 `ISortStrategy` 在職責上有什麼不同？

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Strategy Pattern 如何將功能以物件的形式封裝起來。這讓用戶端（Clients）能夠動態地更換演算法策略。

```csharp
using System;

namespace Strategy.Structural
{
    /// <summary>
    /// Strategy Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            Context context;

            // Three contexts following different strategies

            context = new Context(new ConcreteStrategyA());
            context.ContextInterface();

            context = new Context(new ConcreteStrategyB());
            context.ContextInterface();

            context = new Context(new ConcreteStrategyC());
            context.ContextInterface();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Strategy' abstract class
    /// </summary>

    public abstract class Strategy
    {
        public abstract void AlgorithmInterface();
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class ConcreteStrategyA : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine(
                "Called ConcreteStrategyA.AlgorithmInterface()");
        }
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class ConcreteStrategyB : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine(
                "Called ConcreteStrategyB.AlgorithmInterface()");
        }
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class ConcreteStrategyC : Strategy
    {
        public override void AlgorithmInterface()
        {
            Console.WriteLine(
                "Called ConcreteStrategyC.AlgorithmInterface()");
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class Context
    {
        Strategy strategy;

        // Constructor

        public Context(Strategy strategy)
        {
            this.strategy = strategy;
        }

        public void ContextInterface()
        {
            strategy.AlgorithmInterface();
        }
    }
}
```

**輸出結果 (Output)**

```text
Called ConcreteStrategyA.AlgorithmInterface()
Called ConcreteStrategyB.AlgorithmInterface()
Called ConcreteStrategyC.AlgorithmInterface()
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Strategy Pattern 如何將排序演算法以排序物件的形式封裝起來。這讓用戶端能夠動態地更換排序策略，包括 `QuickSort`（快速排序）、`ShellSort`（希爾排序）與 `MergeSort`（合併排序）。

```csharp
using System;
using System.Collections.Generic;

namespace Strategy.RealWorld
{
    /// <summary>
    /// Strategy Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Two contexts following different strategies

            SortedList studentRecords = new SortedList();

            studentRecords.Add("Samual");
            studentRecords.Add("Jimmy");
            studentRecords.Add("Sandra");
            studentRecords.Add("Vivek");
            studentRecords.Add("Anna");

            studentRecords.SetSortStrategy(new QuickSort());
            studentRecords.Sort();

            studentRecords.SetSortStrategy(new ShellSort());
            studentRecords.Sort();

            studentRecords.SetSortStrategy(new MergeSort());
            studentRecords.Sort();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Strategy' abstract class
    /// </summary>

    public abstract class SortStrategy
    {
        public abstract void Sort(List<string> list);
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class QuickSort : SortStrategy
    {
        public override void Sort(List<string> list)
        {
            list.Sort();  // Default is Quicksort
            Console.WriteLine("QuickSorted list ");
        }
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class ShellSort : SortStrategy
    {
        public override void Sort(List<string> list)
        {
            //list.ShellSort();  not-implemented
            Console.WriteLine("ShellSorted list ");
        }
    }

    /// <summary>
    /// A 'ConcreteStrategy' class
    /// </summary>

    public class MergeSort : SortStrategy
    {
        public override void Sort(List<string> list)
        {
            //list.MergeSort(); not-implemented

            Console.WriteLine("MergeSorted list ");
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class SortedList
    {
        private List<string> list = new List<string>();
        private SortStrategy sortstrategy;

        public void SetSortStrategy(SortStrategy sortstrategy)
        {
            this.sortstrategy = sortstrategy;
        }

        public void Add(string name)
        {
            list.Add(name);
        }

        public void Sort()
        {
            sortstrategy.Sort(list);

            // Iterate over list and display results

            foreach (string name in list)
            {
                Console.WriteLine(" " + name);
            }
            Console.WriteLine();
        }
    }
}
```

**輸出結果 (Output)**

```text
QuickSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek

ShellSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek

MergeSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 `record Student`、集合運算式 `[]` 與完整實作的排序演算法）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Strategy.NetOptimized;

using static System.Console;
using System;
using System.Collections.Generic;

/// <summary>
/// Strategy Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Two contexts following different strategies
        SortedList students = 
              [
                new (Name:"Samual", Ssn: "154-33-2009"),
                new (Name:"Jimmy", Ssn: "487-43-1665" ),
                new (Name:"Sandra", Ssn:  "655-00-2944" ),
                new (Name:"Vivek", Ssn:  "133-98-8399" ),
                new (Name:"Anna", Ssn: "760-94-9844" )
              ];

        students.SortStrategy = new QuickSort();
        students.SortStudents();

        students.SortStrategy = new ShellSort();
        students.SortStudents();

        students.SortStrategy = new MergeSort();
        students.SortStudents();

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Strategy' interface
/// </summary>
public interface ISortStrategy
{
    void Sort(List<Student> list);
}

/// <summary>
/// A 'ConcreteStrategy' class
/// </summary>
public class QuickSort : ISortStrategy
{
    public void Sort(List<Student> list)
    {
        // Call overloaded Sort
        Sort(list, 0, list.Count - 1);
        WriteLine("QuickSorted list ");
    }

    // Recursively sort
    private static void Sort(List<Student> list, int left, int right)
    {
        int lhold = left;
        int rhold = right;

        // Use a random pivot
        var random = new Random();
        int pivot = random.Next(left, right);
        Swap(list, pivot, left);
        pivot = left;
        left++;

        while (right >= left)
        {
            int compareleft = list[left].Name.CompareTo(list[pivot].Name);
            int compareright = list[right].Name.CompareTo(list[pivot].Name);

            if ((compareleft >= 0) && (compareright < 0))
            {
                Swap(list, left, right);
            }
            else
            {
                if (compareleft >= 0)
                {
                    right--;
                }
                else
                {
                    if (compareright < 0)
                    {
                        left++;
                    }
                    else
                    {
                        right--;
                        left++;
                    }
                }
            }
        }
        Swap(list, pivot, right);
        pivot = right;

        if (pivot > lhold) Sort(list, lhold, pivot);
        if (rhold > pivot + 1) Sort(list, pivot + 1, rhold);
    }

    // Swap helper function
    private static void Swap(List<Student> list, int left, int right)
    {
        // Tuple based swap
        (list[left], list[right]) = (list[right], list[left]);
    }
}

/// <summary>
/// A 'ConcreteStrategy' class
/// </summary>
public class ShellSort : ISortStrategy
{
    public void Sort(List<Student> list)
    {
        // ShellSort();  not-implemented
        WriteLine("ShellSorted list ");
    }
}

/// <summary>
/// A 'ConcreteStrategy' class
/// </summary>
public class MergeSort : ISortStrategy
{
    public void Sort(List<Student> list)
    {
        // MergeSort(); not-implemented
        WriteLine("MergeSorted list ");
    }
}

/// <summary>
/// The 'Context' class
/// </summary>
public class SortedList : List<Student>
{
    // Sets sort strategy
    public ISortStrategy SortStrategy { get; set; } = null!;

    // Perform sort
    public void SortStudents()
    {
        SortStrategy.Sort(this);

        // Display sort results
        foreach (var student in this)
        {
            WriteLine($" {student.Name}");
        }
        WriteLine();
    }
}

/// <summary>
/// Represents a student
/// </summary>
public record Student (string Name, string Ssn);
```

**輸出結果 (Output)**

```text
QuickSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek

ShellSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek

MergeSorted list
 Anna
 Jimmy
 Samual
 Sandra
 Vivek
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：State](State.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Template Method →](TemplateMethod.md) |

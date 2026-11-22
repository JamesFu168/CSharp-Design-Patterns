# C# Memento Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：低 (Low)  
> **原文連結**：[https://www.dofactory.com/net/memento-design-pattern](https://www.dofactory.com/net/memento-design-pattern)

---

## 定義與概述 (Definition)

**Memento** Design Pattern 在不違反封裝（Encapsulation）的前提下，捕捉並將物件的內部狀態外部化（Externalizes），以便該物件稍後可以還原（Restored）到此狀態。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

業務人員在編輯潛在客戶的資料時，希望能「復原」到修改之前。最直覺的做法是由外部把欄位抄下來，之後再抄回去：

```csharp
// 示意：由外部一個欄位一個欄位地抄走，之後再抄回來
string savedName = prospect.Name;
string savedPhone = prospect.Phone;
decimal savedBudget = prospect.Budget;
```

- 外部的程式碼必須知道物件有哪些欄位；私有欄位則根本抄不到。
- 物件每新增一個欄位，所有存檔與還原的地方都要跟著修改。
- 物件的內部細節被攤開在外面，破壞了封裝。

Memento 讓物件自己產生一份「快照」交給外部保管。外部只能保管與歸還這份快照，看不到也改不了裡面的內容。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Memento UML Class Diagram](images/memento.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Memento** (`Memento`)
  - 儲存發起者（`Originator`）物件的內部狀態。根據 `Originator` 的決定，Memento 可以儲存所需之多或少的 `Originator` 內部狀態。
  - 防止 `Originator` 以外的其他物件存取其內容。Memento 實際上擁有兩個介面：`Caretaker`（保管者）只能看到 Memento 的窄介面（Narrow Interface）——它只能將 Memento 傳遞給其他物件；相反地，`Originator` 可以看到寬介面（Wide Interface），讓它可以存取將自身還原至先前狀態所需的所有資料。理想情況下，只有產生該 Memento 的 `Originator` 才被允許存取該 Memento 的內部狀態。
- **Originator** (`SalesProspect`)
  - 建立一個包含其目前內部狀態快照（Snapshot）的 Memento。
  - 使用 Memento 來還原其內部狀態。
- **Caretaker** (`Caretaker` / `ProspectMemory`)
  - 負責妥善保管 Memento。
  - 絕不對 Memento 的內容進行操作或檢查。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Memento](samples/Memento)  
> **執行方式**：`dotnet run --project samples/Memento`

**角色對照**：Originator = `SalesProspect`；Memento = `IMemento`（實際內容是 `SalesProspect` 內部的 `Snapshot`）；Caretaker = `ProspectHistory`。

<!-- sample:Memento/Program.cs -->
```csharp
// Memento：把物件的狀態存成快照，之後可以還原；快照的內容只有物件自己看得到。

var prospect = new SalesProspect("Noel van Halen", "(412) 256-0990", 25_000m);
var history = new ProspectHistory();
Console.WriteLine($"Initial : {prospect}");

// 修改之前先存檔
history.Save(prospect);
prospect.Name = "Leo Welch";
prospect.Phone = "(310) 209-7111";
prospect.Budget = 1_000_000m;
Console.WriteLine($"Changed : {prospect}");

// 還原到上一次存檔
history.Undo(prospect);
Console.WriteLine($"Restored: {prospect}");

Console.WriteLine($"Undo again: {history.Undo(prospect)}");

// ---- Memento：對外只是一個沒有任何成員的介面，外界可以保管它，但看不到也改不了內容 ----
public interface IMemento
{
}

// ---- Originator：只有它知道快照裡有什麼，也只有它能建立與讀取快照 ----
public sealed class SalesProspect(string name, string phone, decimal budget)
{
    public string Name { get; set; } = name;
    public string Phone { get; set; } = phone;
    public decimal Budget { get; set; } = budget;

    public IMemento Save() => new Snapshot(Name, Phone, Budget);

    public void Restore(IMemento memento)
    {
        if (memento is not Snapshot snapshot)
        {
            throw new ArgumentException("This memento was not created by a SalesProspect.", nameof(memento));
        }

        // record 可以直接解構成多個變數或屬性
        (Name, Phone, Budget) = snapshot;
    }

    public override string ToString() => $"{Name}, {Phone}, budget ${Budget:N2}";

    // 巢狀的 private 型別：SalesProspect 以外的程式碼完全無法存取它的成員
    private sealed record Snapshot(string Name, string Phone, decimal Budget) : IMemento;
}

// ---- Caretaker：負責保管快照與決定何時還原，但從不查看快照內容 ----
public sealed class ProspectHistory
{
    private readonly Stack<IMemento> _snapshots = new();

    public void Save(SalesProspect prospect) => _snapshots.Push(prospect.Save());

    public bool Undo(SalesProspect prospect)
    {
        if (!_snapshots.TryPop(out IMemento? snapshot))
        {
            return false;   // 沒有可以還原的存檔
        }

        prospect.Restore(snapshot);
        return true;
    }
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Memento -->
```text
Initial : Noel van Halen, (412) 256-0990, budget $25,000.00
Changed : Leo Welch, (310) 209-7111, budget $1,000,000.00
Restored: Noel van Halen, (412) 256-0990, budget $25,000.00
Undo again: False
```
<!-- /output -->

### 重點解說

- **外部看到的是空介面**：`IMemento` 沒有任何成員，`ProspectHistory` 只能把它存起來再交回去。
- **巢狀的 `private record`**：`Snapshot` 宣告在 `SalesProspect` 裡面並標記為 `private`，其他類別連它的名字都用不到，只有 `SalesProspect` 能讀取內容。
- **`record` 的解構**：`(Name, Phone, Budget) = snapshot;` 一次把快照的三個值指派回三個屬性。
- **`Stack` 支援多層復原**：每次 `Save` 推入一份快照，`Undo` 取出最近的一份。
- **`Restore` 會檢查來源**：傳入不是自己建立的 `IMemento` 時丟出例外。

---

## 與 Dofactory 原版的差異

- 原版 .NET Optimized 用 `JsonSerializer` 把整個物件序列化成字串當作快照。這樣只能涵蓋可序列化的公開屬性，還多了序列化的成本；更重要的是 JSON 字串任何人都能讀取與修改，等於放棄了「只有 Originator 能存取快照內容」這個目的。
- 原版 Real-world 的 `Memento` 以公開屬性暴露 `Name`、`Phone`、`Budget`，保管者一樣看得到內容。本版以空介面加上私有巢狀型別隱藏內容。
- 原版的保管者 `ProspectMemory` 只能保存一份快照。本版用 `Stack` 支援多層復原。
- 原版在屬性的 `set` 裡直接 `Console.WriteLine`。本版的屬性沒有副作用，輸出交給主程式。

---

## .NET 中的對應 (In .NET)

- **EF Core 的變更追蹤**：`ChangeTracker` 會保存實體載入時的原始值（`OriginalValues`），用來比對變更或還原。
- **不可變的 `record` 加 `with`**：物件不可變時，舊版本本身就是快照，保留舊的參考即可，不需要另外的 Memento 類別。
- **`IEditableObject`**：資料繫結使用的介面，`BeginEdit` 保存狀態，`CancelEdit` 還原。

---

## 何時該用、何時不該用

**適合使用**

- 需要復原、草稿、或在操作失敗時回到先前的狀態。
- 快照的內容不應該被物件以外的程式碼看到。

**不適合使用**

- 物件很大而且頻繁存檔：記憶體成本高，可以考慮只記錄差異，或改用 Command 的復原機制。
- 物件本身不可變：保留舊的參考就是快照。

---

## 練習題

1. 在 `SalesProspect` 新增 `Email` 屬性並納入快照。`ProspectHistory` 需要修改嗎？
2. 替 `ProspectHistory` 加上 `Redo`。
3. 寫另一個實作 `IMemento` 的類別，把它的物件傳給 `Restore`，觀察會發生什麼事。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Memento Pattern 如何暫時儲存並還原另一個物件的內部狀態。

```csharp
using System;

namespace Memento.Structural
{
    /// <summary>
    /// Memento Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            Originator o = new Originator();
            o.State = "On";

            // Store internal state

            Caretaker c = new Caretaker();
            c.Memento = o.CreateMemento();

            // Continue changing originator

            o.State = "Off";

            // Restore saved state

            o.SetMemento(c.Memento);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Originator' class
    /// </summary>

    public class Originator
    {
        string state;

        public string State
        {
            get { return state; }
            set
            {
                state = value;
                Console.WriteLine("State = " + state);
            }
        }

        // Creates memento 

        public Memento CreateMemento()
        {
            return (new Memento(state));
        }

        // Restores original state

        public void SetMemento(Memento memento)
        {
            Console.WriteLine("Restoring state...");
            State = memento.State;
        }
    }

    /// <summary>
    /// The 'Memento' class
    /// </summary>

    public class Memento
    {
        string state;

        // Constructor

        public Memento(string state)
        {
            this.state = state;
        }

        public string State
        {
            get { return state; }
        }
    }

    /// <summary>
    /// The 'Caretaker' class
    /// </summary>

    public class Caretaker
    {
        Memento memento;

        public Memento Memento
        {
            set { memento = value; }
            get { return memento; }
        }
    }
}
```

**輸出結果 (Output)**

```text
State = On
State = Off
Restoring state...
State = On
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Memento Pattern 如何暫時儲存並隨後還原潛在客戶（`SalesProspect`）的內部狀態。

```csharp
using System;

namespace Memento.RealWorld
{
    /// <summary>
    /// Memento Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            SalesProspect s = new SalesProspect();
            s.Name = "Noel van Halen";
            s.Phone = "(412) 256-0990";
            s.Budget = 25000.0;

            // Store internal state

            ProspectMemory m = new ProspectMemory();
            m.Memento = s.SaveMemento();

            // Continue changing originator

            s.Name = "Leo Welch";
            s.Phone = "(310) 209-7111";
            s.Budget = 1000000.0;

            // Restore saved state

            s.RestoreMemento(m.Memento);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Originator' class
    /// </summary>

    public class SalesProspect
    {
        string name;
        string phone;
        double budget;

        // Gets or sets name

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                Console.WriteLine("Name:   " + name);
            }
        }

        // Gets or sets phone

        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                Console.WriteLine("Phone:  " + phone);
            }
        }

        // Gets or sets budget

        public double Budget
        {
            get { return budget; }
            set
            {
                budget = value;
                Console.WriteLine("Budget: " + budget);
            }
        }

        // Stores memento

        public Memento SaveMemento()
        {
            Console.WriteLine("\nSaving state --\n");
            return new Memento(name, phone, budget);
        }

        // Restores memento

        public void RestoreMemento(Memento memento)
        {
            Console.WriteLine("\nRestoring state --\n");
            Name = memento.Name;
            Phone = memento.Phone;
            Budget = memento.Budget;
        }
    }

    /// <summary>
    /// The 'Memento' class
    /// </summary>

    public class Memento
    {
        string name;
        string phone;
        double budget;

        // Constructor

        public Memento(string name, string phone, double budget)
        {
            this.name = name;
            this.phone = phone;
            this.budget = budget;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public double Budget
        {
            get { return budget; }
            set { budget = value; }
        }
    }

    /// <summary>
    /// The 'Caretaker' class
    /// </summary>

    public class ProspectMemory
    {
        Memento memento;

        public Memento Memento
        {
            set { memento = value; }
            get { return memento; }
        }
    }
}
```

**輸出結果 (Output)**

```text
Name:   Noel van Halen
Phone:  (412) 256-0990
Budget: 25000

Saving state --

Name:   Leo Welch
Phone:  (310) 209-7111
Budget: 1000000

Restoring state --

Name:   Noel van Halen
Phone:  (412) 256-0990
Budget: 25000
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如透過 `System.Text.Json.JsonSerializer` 將狀態序列化與反序列化）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Memento.NetOptimized;

using System.Text.Json;
using static System.Console;

/// <summary>
/// Memento Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Init sales prospect through object initialization
        var s = new SalesProspect
        {
            Name = "Joel van Halen",
            Phone = "(412) 256-0990",
            Budget = 25000.0
        };

        // Store internal state
        var m = new ProspectMemory(s.SaveMemento());

        // Change originator
        s.Name = "Leo Welch";
        s.Phone = "(310) 209-7111";
        s.Budget = 1000000.0;

        // Restore saved state
        s.RestoreMemento(m.Memento);

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Originator' class
/// </summary>
public class SalesProspect
{
    private string name = null!;
    private string phone = null!;
    private double budget;

    // Gets or sets name
    public string Name
    {
        get => name; 
        set
        {
            name = value;
            WriteLine("Name:   " + name);
        }
    }

    // Gets or sets phone
    public string Phone
    {
        get => phone;
        set
        {
            phone = value;
            WriteLine("Phone:  " + phone);
        }
    }

    // Gets or sets budget
    public double Budget
    {
        get => budget;
        set
        {
            budget = value;
            WriteLine("Budget: " + budget);
        }
    }

    // Stores (serializes) memento
    public Memento SaveMemento()
    {
        WriteLine("\nSaving state --\n");

        var memento = new Memento();
        return memento.Serialize(this);
    }

    // Restores (deserializes) memento
    public void RestoreMemento(Memento memento)
    {
        WriteLine("\nRestoring state --\n");

        var s = (SalesProspect)memento.Deserialize();
        Name = s.Name;
        Phone = s.Phone;
        Budget = s.Budget;
    }
}

/// <summary>
/// The 'Memento' class
/// </summary>
public class Memento
{
    private string store = null!;

    public Memento Serialize(object o)
    {
        store = JsonSerializer.Serialize(o);
        return this;
    }

    public object Deserialize()
    {
        return JsonSerializer.Deserialize<SalesProspect>(store)!;
    }
}

/// <summary>
/// The 'Caretaker' class
/// </summary>
public record ProspectMemory (Memento Memento);
```

**輸出結果 (Output)**

```text
Name:   Joel van Halen
Phone:  (412) 256-0990
Budget: 25000

Saving state --

Name:   Leo Welch
Phone:  (310) 209-7111
Budget: 1000000

Restoring state --

Name:   Joel van Halen
Phone:  (412) 256-0990
Budget: 25000
Name:   Joel van Halen
Phone:  (412) 256-0990
Budget: 25000
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Mediator](Mediator.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Observer →](Observer.md) |

# C# Observer Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/observer-design-pattern](https://www.dofactory.com/net/observer-design-pattern)

---

## 定義與概述 (Definition)

**Observer** Design Pattern 在物件之間定義了一對多（One-to-many）的相依關係，使得當一個物件改變狀態時，所有相依於它的物件都會自動收到通知並進行更新。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

股票價格變動時要通知投資人。如果讓 `Stock` 直接呼叫每一個要通知的對象：

```csharp
// 示意：Stock 直接認識每一個要通知的對象
public void SetPrice(decimal price)
{
    _price = price;
    _sorros.Notify(this);
    _berkshire.Notify(this);
    _emailService.SendAlert(this);
}
```

- 每多一個要通知的對象，就要修改 `Stock`。
- `Stock` 相依所有通知對象的類別，無法單獨重複使用。
- 沒有辦法在執行期新增或移除通知對象。

Observer 讓 `Stock` 只負責「發出通知」，由想收到通知的物件自己來訂閱。C# 以 `event` 關鍵字直接支援這個模式。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Observer UML Class Diagram](images/observer.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Subject** (`Stock`)
  - 認識它的觀察者（Observers）。可以有任意數量的 `Observer` 物件同時觀察一個 `Subject`。
  - 提供一個用於附加（Attaching）與分離（Detaching）`Observer` 物件的介面。
- **ConcreteSubject** (`IBM`)
  - 儲存 `ConcreteObserver` 感興趣的狀態。
  - 當其狀態改變時，向它的觀察者發送通知。
- **Observer** (`IInvestor`)
  - 為那些在 `Subject` 狀態改變時應該收到通知的物件定義一個更新介面（Updating Interface）。
- **ConcreteObserver** (`Investor`)
  - 維護一個指向 `ConcreteSubject` 物件的參考。
  - 儲存應該與 `Subject` 保持一致的狀態。
  - 實作 `Observer` 的更新介面，以確保其自身狀態與 `Subject` 的狀態保持一致。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Observer](samples/Observer)  
> **執行方式**：`dotnet run --project samples/Observer`

**角色對照**：Subject = `Stock`；Observer = 任何符合 `EventHandler<PriceChangedEventArgs>` 簽章的方法（這裡是 `Investor.OnPriceChanged`）；通知的內容 = `PriceChangedEventArgs`。

<!-- sample:Observer/Program.cs -->
```csharp
// Observer：一個物件的狀態改變時，自動通知所有「訂閱」它的物件。C# 用 event 內建支援這個模式。

var ibm = new Stock("IBM", 120.00m);
var sorros = new Investor("Sorros");
var berkshire = new Investor("Berkshire");

// 訂閱：+=
ibm.PriceChanged += sorros.OnPriceChanged;
ibm.PriceChanged += berkshire.OnPriceChanged;

ibm.Price = 120.10m;
ibm.Price = 121.00m;
ibm.Price = 121.00m;   // 價格沒有改變，不會通知

// 取消訂閱：-=。不再需要通知時一定要取消，否則 Stock 會一直持有 Investor，造成記憶體洩漏
ibm.PriceChanged -= berkshire.OnPriceChanged;
Console.WriteLine("Berkshire unsubscribed.");

ibm.Price = 120.50m;

// 事件資料：依 .NET 慣例繼承 EventArgs，並設計成唯讀
public sealed class PriceChangedEventArgs(string symbol, decimal oldPrice, decimal newPrice) : EventArgs
{
    public string Symbol { get; } = symbol;
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
}

// ---- Subject：用 event 宣告「可以被訂閱的通知」 ----
public sealed class Stock(string symbol, decimal price)
{
    // event 只允許外部 += 與 -=，外部不能直接觸發，也不能清掉別人的訂閱
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public string Symbol => symbol;

    public decimal Price
    {
        get;
        set
        {
            // field 代表編譯器替這個屬性產生的欄位，不必另外宣告 _price
            if (field == value)
            {
                return;
            }

            decimal oldPrice = field;
            field = value;

            // ?. 的意思是「有人訂閱才呼叫」
            PriceChanged?.Invoke(this, new PriceChangedEventArgs(symbol, oldPrice, value));
        }
    } = price;
}

// ---- Observer：方法簽章符合 EventHandler<PriceChangedEventArgs> 就能訂閱 ----
public sealed class Investor(string name)
{
    public void OnPriceChanged(object? sender, PriceChangedEventArgs e) =>
        Console.WriteLine($"Notified {name}: {e.Symbol} changed from ${e.OldPrice:N2} to ${e.NewPrice:N2}");
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Observer -->
```text
Notified Sorros: IBM changed from $120.00 to $120.10
Notified Berkshire: IBM changed from $120.00 to $120.10
Notified Sorros: IBM changed from $120.10 to $121.00
Notified Berkshire: IBM changed from $120.10 to $121.00
Berkshire unsubscribed.
Notified Sorros: IBM changed from $121.00 to $120.50
```
<!-- /output -->

### 重點解說

- **`event`**：外部只能用 `+=` 訂閱、`-=` 取消訂閱，不能直接觸發事件，也不能清除別人的訂閱。
- **`field` 關鍵字（C# 14）**：在屬性的存取子裡，`field` 代表編譯器替這個屬性產生的欄位，不必另外宣告 `_price`。
- **`?.Invoke`**：沒有任何人訂閱時事件是 `null`，`?.` 會略過呼叫。
- **值沒有改變就不通知**：setter 先比較新舊值，避免發出沒有意義的通知。
- **通知裡帶著資料**：`PriceChangedEventArgs` 包含新舊價格，訂閱者不需要回頭去查詢 `Stock`。
- **記得取消訂閱**：事件會持有訂閱者的參考。訂閱者不再使用卻沒有 `-=`，就無法被回收，這是 .NET 程式常見的記憶體洩漏來源。

---

## 與 Dofactory 原版的差異

- 原版 Structural 與 Real-world 自己維護 `List<IInvestor>` 以及 `Attach`、`Detach`、`Notify` 三個方法。C# 的 `event` 已經內建這三件事。
- 原版 .NET Optimized 已改用 `event`，但把它宣告成 `= null!` 來消除可為 Null 的警告。本版宣告為可為 Null 的型別，並以 `?.Invoke` 呼叫。
- 原版的 `Investor` 有一個從未使用的 `Stock` 屬性，本版移除。
- 原版以 `double` 表示價格。本版改用 `decimal`。
- 原版沒有示範取消訂閱。

---

## .NET 中的對應 (In .NET)

- **`event` 與 `EventHandler<TEventArgs>`**：語言內建的 Observer；`INotifyPropertyChanged` 是資料繫結的基礎。
- **`IObservable<T>` 與 `IObserver<T>`**：把通知視為「隨時間到來的一連串資料」，搭配 Rx.NET 可以對通知做篩選、合併、節流。
- **`IOptionsMonitor<T>.OnChange`**：組態檔變更時收到通知。
- **`CancellationToken.Register`**：取消發生時呼叫指定的回呼。

---

## 何時該用、何時不該用

**適合使用**

- 一對多的通知，而且發出通知的一方不應該知道有哪些接收者。
- 接收者會在執行期增加或減少。

**不適合使用**

- 只有一個固定的接收者：直接呼叫，或傳入一個委派。
- 處理的順序或結果很重要：事件的訂閱者依訂閱順序被呼叫，其中一個丟出例外，後面的就不會被呼叫。
- 事件一個觸發一個，連鎖反應多到難以追蹤流程。

---

## 練習題

1. 用 lambda 新增一個訂閱者：價格低於 120.20 時印出警示訊息。
2. 讓其中一個處理常式丟出例外，觀察後面的訂閱者是否還會收到通知。要怎麼讓訂閱者之間互不影響？（提示：`GetInvocationList()`）
3. 讓 `Investor` 實作 `IDisposable`，在建構函式訂閱、在 `Dispose` 取消訂閱。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Observer Pattern 如何讓已註冊的物件在狀態改變時收到通知並進行更新。

```csharp
using System;
using System.Collections.Generic;

namespace Observer.Structural
{
    /// <summary>
    /// Observer Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Observer pattern

            ConcreteSubject s = new ConcreteSubject();

            s.Attach(new ConcreteObserver(s, "X"));
            s.Attach(new ConcreteObserver(s, "Y"));
            s.Attach(new ConcreteObserver(s, "Z"));

            // Change subject and notify observers

            s.SubjectState = "ABC";
            s.Notify();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subject' abstract class
    /// </summary>

    public abstract class Subject
    {
        private List<Observer> observers = new List<Observer>();

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer o in observers)
            {
                o.Update();
            }
        }
    }

    /// <summary>
    /// The 'ConcreteSubject' class
    /// </summary>

    public class ConcreteSubject : Subject
    {
        private string subjectState;

        // Gets or sets subject state

        public string SubjectState
        {
            get { return subjectState; }
            set { subjectState = value; }
        }
    }

    /// <summary>
    /// The 'Observer' abstract class
    /// </summary>

    public abstract class Observer
    {
        public abstract void Update();
    }

    /// <summary>
    /// The 'ConcreteObserver' class
    /// </summary>

    public class ConcreteObserver : Observer
    {
        private string name;
        private string observerState;
        private ConcreteSubject subject;

        // Constructor

        public ConcreteObserver(
            ConcreteSubject subject, string name)
        {
            this.subject = subject;
            this.name = name;
        }

        public override void Update()
        {
            observerState = subject.SubjectState;
            Console.WriteLine("Observer {0}'s new state is {1}",
                name, observerState);
        }

        // Gets or sets subject

        public ConcreteSubject Subject
        {
            get { return subject; }
            set { subject = value; }
        }
    }
}
```

**輸出結果 (Output)**

```text
Observer X's new state is ABC
Observer Y's new state is ABC
Observer Z's new state is ABC
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Observer Pattern 如何讓已註冊的投資人（`Investor`）在每次股票（`Stock`）價格變動時都收到通知。

```csharp
using System;
using System.Collections.Generic;

namespace Observer.RealWorld
{
    /// <summary>
    /// Observer Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create IBM stock and attach investors

            IBM ibm = new IBM("IBM", 120.00);
            ibm.Attach(new Investor("Sorros"));
            ibm.Attach(new Investor("Berkshire"));

            // Fluctuating prices will notify investors

            ibm.Price = 120.10;
            ibm.Price = 121.00;
            ibm.Price = 120.50;
            ibm.Price = 120.75;

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subject' abstract class
    /// </summary>

    public abstract class Stock
    {
        private string symbol;
        private double price;
        private List<IInvestor> investors = new List<IInvestor>();

        // Constructor

        public Stock(string symbol, double price)
        {
            this.symbol = symbol;
            this.price = price;
        }

        public void Attach(IInvestor investor)
        {
            investors.Add(investor);
        }

        public void Detach(IInvestor investor)
        {
            investors.Remove(investor);
        }

        public void Notify()
        {
            foreach (IInvestor investor in investors)
            {
                investor.Update(this);
            }

            Console.WriteLine("");
        }

        // Gets or sets the price

        public double Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    Notify();
                }
            }
        }

        // Gets the symbol

        public string Symbol
        {
            get { return symbol; }
        }
    }

    /// <summary>
    /// The 'ConcreteSubject' class
    /// </summary>

    public class IBM : Stock
    {
        // Constructor

        public IBM(string symbol, double price)
            : base(symbol, price)
        {
        }
    }

    /// <summary>
    /// The 'Observer' interface
    /// </summary>

    public interface IInvestor
    {
        void Update(Stock stock);
    }

    /// <summary>
    /// The 'ConcreteObserver' class
    /// </summary>

    public class Investor : IInvestor
    {
        private string name;
        private Stock stock;

        // Constructor

        public Investor(string name)
        {
            this.name = name;
        }

        public void Update(Stock stock)
        {
            Console.WriteLine("Notified {0} of {1}'s " +
                "change to {2:C}", name, stock.Symbol, stock.Price);
        }

        // Gets or sets the stock

        public Stock Stock
        {
            get { return stock; }
            set { stock = value; }
        }
    }
}
```

**輸出結果 (Output)**

```text
Notified Sorros of IBM's change to $120.10
Notified Berkshire of IBM's change to $120.10

Notified Sorros of IBM's change to $121.00
Notified Berkshire of IBM's change to $121.00

Notified Sorros of IBM's change to $120.50
Notified Berkshire of IBM's change to $120.50

Notified Sorros of IBM's change to $120.75
Notified Berkshire of IBM's change to $120.75
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 C# 原生的 `event` 與 `EventHandler<ChangeEventArgs>` 委派機制）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Observer.NetOptimized;

using static System.Console;
using System;

/// <summary>
/// Observer Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create IBM stock and attach investors
        var ibm = new IBM(120.00);

        // Attach 'listeners', i.e. Investors
        ibm.Attach(new Investor { Name = "Sorros" });
        ibm.Attach(new Investor { Name = "Berkshire" });

        // Fluctuating prices will notify listening investors
        ibm.Price = 120.10;
        ibm.Price = 121.00;
        ibm.Price = 120.50;
        ibm.Price = 120.75;

        // Wait for user
        ReadKey();
    }
}

// Custom event arguments
public class ChangeEventArgs : EventArgs
{
    // Gets or sets symbol
    public string Symbol { get; set; }

    // Gets or sets price
    public double Price { get; set; }
}

/// <summary>
/// The 'Subject' abstract class
/// </summary>
public abstract class Stock(string symbol, double price)
{
    protected string symbol = symbol;
    protected double price = price;

    // Event
    public event EventHandler<ChangeEventArgs> Change = null!;

    // Invoke the Change event
    public virtual void OnChange(ChangeEventArgs e)
    {
        Change?.Invoke(this, e);
    }

    public void Attach(IInvestor investor)
    {
        Change += investor.Update;
    }

    public void Detach(IInvestor investor)
    {
        Change -= investor.Update;
    }

    // Gets or sets the price
    public double Price
    {
        get => price; 
        set
        {
            if (price != value)
            {
                price = value;
                OnChange(new ChangeEventArgs { Symbol = symbol, Price = price });
                WriteLine("");
            }
        }
    }
}

/// <summary>
/// The 'ConcreteSubject' class
/// </summary>
public class IBM(double price) : Stock("IBM", price)
{
}

/// <summary>
/// The 'Observer' interface
/// </summary>
public interface IInvestor
{
    void Update(object sender, ChangeEventArgs e);
}

/// <summary>
/// The 'ConcreteObserver' class
/// </summary>
public class Investor : IInvestor
{
    // Gets or sets the investor name
    public string Name { get; set; } = null!;

    // Gets or sets the stock
    public Stock Stock { get; set; } = null!;

    public void Update(object sender, ChangeEventArgs e)
    {
        WriteLine("Notified {0} of {1}'s " +
            "change to {2:C}", Name, e.Symbol, e.Price);
    }
}
```

**輸出結果 (Output)**

```text
Notified Sorros of IBM's change to $120.10
Notified Berkshire of IBM's change to $120.10

Notified Sorros of IBM's change to $121.00
Notified Berkshire of IBM's change to $121.00

Notified Sorros of IBM's change to $120.50
Notified Berkshire of IBM's change to $120.50

Notified Sorros of IBM's change to $120.75
Notified Berkshire of IBM's change to $120.75
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Memento](Memento.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：State →](State.md) |

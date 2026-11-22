# C# Chain of Responsibility Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：中低 (Medium-Low)  
> **原文連結**：[https://www.dofactory.com/net/chain-of-responsibility-design-pattern](https://www.dofactory.com/net/chain-of-responsibility-design-pattern)

---

## 定義與概述 (Definition)

**Chain of Responsibility** Design Pattern 藉由給予多個物件處理請求的機會，避免將請求的發送者（Sender）與接收者（Receiver）耦合在一起。此 Pattern 會將接收物件串連成一條鏈（Chain），並沿著這條鏈傳遞請求，直到有一個物件處理它為止。

**Chain of Responsibility** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Chain of Responsibility UML Class Diagram](images/chain.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Handler** (`Approver`)
  - 定義一個用於處理請求的介面。
  - （選用）實作指向後繼者（Successor）的連結。
- **ConcreteHandler** (`Director`, `VicePresident`, `President`)
  - 處理它所負責的請求。
  - 可以存取它的後繼者（Successor）。
  - 如果 `ConcreteHandler` 可以處理該請求，它就會直接處理；否則它會將請求轉發（Forwards）給它的後繼者。
- **Client** (`ChainApp`)
  - 向鏈（Chain）上的某個 `ConcreteHandler` 物件發起請求。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Chain of Responsibility Pattern 如何讓數個串連在一起的物件（即責任鏈，Chain）都有機會回應請求，或是將請求轉交給鏈上的下一個物件。

```csharp
using System;

namespace Chain.Structural
{
    /// <summary>
    /// Chain of Responsibility Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup Chain of Responsibility

            Handler h1 = new ConcreteHandler1();
            Handler h2 = new ConcreteHandler2();
            Handler h3 = new ConcreteHandler3();
            h1.SetSuccessor(h2);
            h2.SetSuccessor(h3);

            // Generate and process request

            int[] requests = { 2, 5, 14, 22, 18, 3, 27, 20 };

            foreach (int request in requests)
            {
                h1.HandleRequest(request);
            }

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// The 'Handler' abstract class
    /// </summary>

    public abstract class Handler
    {
        protected Handler successor;

        public void SetSuccessor(Handler successor)
        {
            this.successor = successor;
        }

        public abstract void HandleRequest(int request);
    }

    /// <summary>
    /// The 'ConcreteHandler1' class
    /// </summary>

    public class ConcreteHandler1 : Handler
    {
        public override void HandleRequest(int request)
        {
            if (request >= 0 && request < 10)
            {
                Console.WriteLine("{0} handled request {1}",
                    this.GetType().Name, request);
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// The 'ConcreteHandler2' class
    /// </summary>

    public class ConcreteHandler2 : Handler
    {
        public override void HandleRequest(int request)
        {
            if (request >= 10 && request < 20)
            {
                Console.WriteLine("{0} handled request {1}",
                    this.GetType().Name, request);
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }

    /// <summary>
    /// The 'ConcreteHandler3' class
    /// </summary>

    public class ConcreteHandler3 : Handler
    {
        public override void HandleRequest(int request)
        {
            if (request >= 20 && request < 30)
            {
                Console.WriteLine("{0} handled request {1}",
                    this.GetType().Name, request);
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }
}
```

### 輸出結果 (Output)

```text
ConcreteHandler1 handled request 2

  ConcreteHandler1 handled request 5

  ConcreteHandler2 handled request 14

  ConcreteHandler3 handled request 22

  ConcreteHandler2 handled request 18

  ConcreteHandler1 handled request 3

  ConcreteHandler3 handled request 27

  ConcreteHandler3 handled request 20
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Chain of Responsibility Pattern 如何讓數個串連在一起的經理與高階主管回應採購請求（`Purchase`），或是將請求轉交給上級主管。每個職位都可以擁有自己的一套規則，決定他們可以核准哪些採購單。

```csharp
using System;

namespace Chain.RealWorld
{
    /// <summary>
    /// Chain of Responsibility Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup Chain of Responsibility

            Approver larry = new Director();
            Approver sam = new VicePresident();
            Approver tammy = new President();

            larry.SetSuccessor(sam);
            sam.SetSuccessor(tammy);

            // Generate and process purchase requests

            Purchase p = new Purchase(2034, 350.00, "Supplies");
            larry.ProcessRequest(p);

            p = new Purchase(2035, 32590.10, "Project X");
            larry.ProcessRequest(p);

            p = new Purchase(2036, 122100.00, "Project Y");
            larry.ProcessRequest(p);

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// The 'Handler' abstract class
    /// </summary>

    public abstract class Approver
    {
        protected Approver successor;

        public void SetSuccessor(Approver successor)
        {
            this.successor = successor;
        }

        public abstract void ProcessRequest(Purchase purchase);
    }

    /// <summary>
    /// The 'ConcreteHandler' class
    /// </summary>

    public class Director : Approver
    {
        public override void ProcessRequest(Purchase purchase)
        {
            if (purchase.Amount < 10000.0)
            {
                Console.WriteLine("{0} approved request# {1}",
                    this.GetType().Name, purchase.Number);
            }
            else if (successor != null)
            {
                successor.ProcessRequest(purchase);
            }
        }
    }

    /// <summary>
    /// The 'ConcreteHandler' class
    /// </summary>

    public class VicePresident : Approver
    {
        public override void ProcessRequest(Purchase purchase)
        {
            if (purchase.Amount < 25000.0)
            {
                Console.WriteLine("{0} approved request# {1}",
                    this.GetType().Name, purchase.Number);
            }
            else if (successor != null)
            {
                successor.ProcessRequest(purchase);
            }
        }
    }

    /// <summary>
    /// The 'ConcreteHandler' class
    /// </summary>

    public class President : Approver
    {
        public override void ProcessRequest(Purchase purchase)
        {
            if (purchase.Amount < 100000.0)
            {
                Console.WriteLine("{0} approved request# {1}",
                    this.GetType().Name, purchase.Number);
            }
            else
            {
                Console.WriteLine(
                    "Request# {0} requires an executive meeting!",
                    purchase.Number);
            }
        }
    }

    /// <summary>
    /// Class holding request details
    /// </summary>

    public class Purchase
    {
        int number;
        double amount;
        string purpose;

        // Constructor

        public Purchase(int number, double amount, string purpose)
        {
            this.number = number;
            this.amount = amount;
            this.purpose = purpose;
        }

        // Gets or sets purchase number

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        // Gets or sets purchase amount

        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        // Gets or sets purchase purpose

        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }
    }
}
```

### 輸出結果 (Output)

```text
Director Larry approved request# 2034

  President Tammy approved request# 2035

  Request# 2036 requires an executive meeting!
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 `record` 型別與簡潔的屬性宣告）。這是一個優雅的 C# Chain of Responsibility 解決方案。

```csharp
namespace Chain.NetOptimized;

using static System.Console;

using System;

/// <summary>
/// Chain of Responsibility Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Setup Chain of Responsibility
        var larry = new Director();
        var sam = new VicePresident();
        var tammy = new President();

        larry.Successor = sam;
        sam.Successor = tammy;

        // Generate and process purchase requests
        var purchase = new Purchase { Number = 2034, Amount = 350.00, Purpose = "Supplies" };
        larry.ProcessRequest(purchase);

        purchase = new Purchase { Number = 2035, Amount = 32590.10, Purpose = "Project X" };
        larry.ProcessRequest(purchase);

        purchase = new Purchase { Number = 2036, Amount = 122100.00, Purpose = "Project Y" };
        larry.ProcessRequest(purchase);

        // Wait for user
        ReadKey();
    }
}
// Purchase event argument holds purchase info
public class PurchaseEventArgs : EventArgs
{
    internal Purchase Purchase { get; init; } = null!;
}

/// <summary>
/// The 'Handler' abstract class
/// </summary>
public abstract class Approver
{
    // Purchase event 
    public EventHandler<PurchaseEventArgs> Purchase;

    // Purchase event handler
    public abstract void PurchaseHandler(object sender, PurchaseEventArgs e);

    // Constructor
    public Approver()
    {
        Purchase += PurchaseHandler!;
    }

    public void ProcessRequest(Purchase purchase)
    {
        OnPurchase(new PurchaseEventArgs { Purchase = purchase });
    }

    // Invoke the Purchase event
    public virtual void OnPurchase(PurchaseEventArgs e)
    {
        Purchase?.Invoke(this, e);
    }

    // Sets or gets the next approver
    public Approver Successor { get; set; } = null!;
}

/// <summary>
/// The 'ConcreteHandler' class
/// </summary>
public class Director : Approver
{
    public override void PurchaseHandler(object sender, PurchaseEventArgs e)
    {
        if (e.Purchase.Amount < 10000.0)
        {
            WriteLine("{0} approved request# {1}",
                this.GetType().Name, e.Purchase.Number);
        }
        else
        {
            Successor?.PurchaseHandler(this, e);
        }
    }
}

/// <summary>
/// The 'ConcreteHandler' class
/// </summary>
public class VicePresident : Approver
{
    public override void PurchaseHandler(object sender, PurchaseEventArgs e)
    {
        if (e.Purchase.Amount < 25000.0)
        {
            WriteLine("{0} approved request# {1}",
                this.GetType().Name, e.Purchase.Number);
        }
        else
        {
            Successor?.PurchaseHandler(this, e);
        }
    }
}

/// <summary>
/// The 'ConcreteHandler' clas
/// </summary>
public class President : Approver
{
    public override void PurchaseHandler(object sender, PurchaseEventArgs e)
    {
        if (e.Purchase.Amount < 100000.0)
        {
            WriteLine("{0} approved request# {1}",
                sender.GetType().Name, e.Purchase.Number);
        }
        else if (Successor != null)
        {
            Successor.PurchaseHandler(this, e);
        }
        else
        {
            WriteLine(
                "Request# {0} requires an executive meeting!",
                e.Purchase.Number);
        }
    }
}

/// <summary>
/// Record with request details
/// </summary>
public record Purchase
{
    public required double Amount { get; init; }
    public required string Purpose { get; init; }
    public required int Number { get; init; }
}
```

### 輸出結果 (Output)

```text
Director Larry approved request# 2034

    President Tammy approved request# 2035

    Request# 2036 requires an executive meeting!
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Proxy](Proxy.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Command →](Command.md) |

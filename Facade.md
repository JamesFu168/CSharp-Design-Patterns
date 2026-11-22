# C# Facade Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/facade-design-pattern](https://www.dofactory.com/net/facade-design-pattern)

---

## 定義與概述 (Definition)

**Facade** Design Pattern 為子系統（Subsystem）中的一組介面提供一個統一的介面。此 Pattern 定義了一個更高階的介面，讓該子系統更容易被使用。

**Facade** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Facade UML Class Diagram](images/facade.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Facade** (`MortgageApplication`)
  - 知道哪些子系統類別負責處理哪個請求。
  - 將用戶端（Client）的請求委派（Delegates）給適當的子系統物件。
- **Subsystem classes** (`Bank`, `Credit`, `Loan`)
  - 實作子系統的功能。
  - 處理由 `Facade` 物件所指派的工作。
  - 對 `Facade` 一無所知，且不保留任何指向它的參考（Reference）。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Facade Pattern 如何為龐大的類別子系統提供一個簡化且一致的介面。

```csharp
using System;

namespace Facade.Structural
{
    /// <summary>
    /// Facade Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            Facade facade = new Facade();

            facade.MethodA();
            facade.MethodB();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subsystem ClassA' class
    /// </summary>

    public class SubSystemOne
    {
        public void MethodOne()
        {
            Console.WriteLine(" SubSystemOne Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassB' class
    /// </summary>

    public class SubSystemTwo
    {
        public void MethodTwo()
        {
            Console.WriteLine(" SubSystemTwo Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassC' class
    /// </summary>

    public class SubSystemThree
    {
        public void MethodThree()
        {
            Console.WriteLine(" SubSystemThree Method");
        }
    }

    /// <summary>
    /// The 'Subsystem ClassD' class
    /// </summary>

    public class SubSystemFour
    {
        public void MethodFour()
        {
            Console.WriteLine(" SubSystemFour Method");
        }
    }

    /// <summary>
    /// The 'Facade' class
    /// </summary>

    public class Facade
    {
        SubSystemOne one;
        SubSystemTwo two;
        SubSystemThree three;
        SubSystemFour four;

        public Facade()
        {
            one = new SubSystemOne();
            two = new SubSystemTwo();
            three = new SubSystemThree();
            four = new SubSystemFour();
        }

        public void MethodA()
        {
            Console.WriteLine("\nMethodA() ---- ");
            one.MethodOne();
            two.MethodTwo();
            four.MethodFour();
        }

        public void MethodB()
        {
            Console.WriteLine("\nMethodB() ---- ");
            two.MethodTwo();
            three.MethodThree();
        }
    }
}
```

### 輸出結果 (Output)

```text
MethodA() ----

  SubSystemOne Method

  SubSystemTwo Method

  SubSystemFour Method

  

  MethodB() ----

  SubSystemTwo Method

  SubSystemThree Method
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼將 Facade Pattern 示範為一個房貸申請（`Mortgage`）物件，它為用來評估申請人信用狀況的龐大類別子系統（包含 `Bank`、`Credit`、`Loan`）提供了一個簡化的介面。

```csharp
using System;

namespace Facade.RealWorld
{
    /// <summary>
    /// Facade Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Facade

            Mortgage mortgage = new Mortgage();

            // Evaluate mortgage eligibility for customer

            Customer customer = new Customer("Ann McKinsey");
            bool eligible = mortgage.IsEligible(customer, 125000);

            Console.WriteLine("\n" + customer.Name +
                    " has been " + (eligible ? "Approved" : "Rejected"));

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subsystem ClassA' class
    /// </summary>

    public class Bank
    {
        public bool HasSufficientSavings(Customer c, int amount)
        {
            Console.WriteLine("Check bank for " + c.Name);
            return true;
        }
    }

    /// <summary>
    /// The 'Subsystem ClassB' class
    /// </summary>

    public class Credit
    {
        public bool HasGoodCredit(Customer c)
        {
            Console.WriteLine("Check credit for " + c.Name);
            return true;
        }
    }

    /// <summary>
    /// The 'Subsystem ClassC' class
    /// </summary>

    public class Loan
    {
        public bool HasNoBadLoans(Customer c)
        {
            Console.WriteLine("Check loans for " + c.Name);
            return true;
        }
    }

    /// <summary>
    /// Customer class
    /// </summary>

    public class Customer
    {
        private string name;

        // Constructor

        public Customer(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }

    /// <summary>
    /// The 'Facade' class
    /// </summary>

    public class Mortgage
    {
        Bank bank = new Bank();
        Loan loan = new Loan();
        Credit credit = new Credit();

        public bool IsEligible(Customer cust, int amount)
        {
            Console.WriteLine("{0} applies for {1:C} loan\n",
                cust.Name, amount);

            bool eligible = true;

            // Check creditworthyness of applicant

            if (!bank.HasSufficientSavings(cust, amount))
            {
                eligible = false;
            }
            else if (!loan.HasNoBadLoans(cust))
            {
                eligible = false;
            }
            else if (!credit.HasGoodCredit(cust))
            {
                eligible = false;
            }

            return eligible;
        }
    }
}
```

### 輸出結果 (Output)

```text
Ann McKinsey applies for $125,000.00 loan

  

  Check bank for Ann McKinsey

  Check loans for Ann McKinsey

  Check credit for Ann McKinsey

  

  Ann McKinsey has been Approved
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 `record` 型別與簡潔的運算式語法）。這是一個優雅的 C# Facade 解決方案。

```csharp
namespace Facade.NetOptimized;

using static System.Console;

/// <summary>
/// Facade Design Pattern
/// </summary>
public class Program
{
    static void Main()
    {
        // Facade
        var mortgage = new Mortgage();

        // Evaluate mortgage eligibility for customer
        var customer = new Customer("Ann McKinsey");
        bool eligible = mortgage.IsEligible(customer, 125000);

        string result = eligible ? "Approved" : "Rejected";
        WriteLine($"\n{customer.Name} has been {result}");


        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Subsystem ClassA' class
/// </summary>
public class Bank
{
    public bool HasSufficientSavings(Customer c, int amount)
    {
        WriteLine($"Check bank for {c.Name}");
        return true;
    }
}

/// <summary>
/// The 'Subsystem ClassB' class
/// </summary>
public class Credit
{
    public bool HasGoodCredit(Customer c)
    {
        WriteLine($"Check credit for {c.Name}");
        return true;
    }
}

/// <summary>
/// The 'Subsystem ClassC' class
/// </summary>
public class Loan
{
    public bool HasNoBadLoans(Customer c)
    {
        WriteLine($"Check loans for {c.Name}");
        return true;
    }
}

/// <summary>
/// The 'Facade' class
/// </summary>
public class Mortgage
{
    private readonly Bank bank = new();
    private readonly Loan loan = new();
    private readonly Credit credit = new();

    public bool IsEligible(Customer cust, int amount)
    {
        WriteLine("{0} applies for {1:C} loan\n",
            cust.Name, amount);

        bool eligible = true;

        // Check creditworthyness of applicant
        if (!bank.HasSufficientSavings(cust, amount))
        {
            eligible = false;
        }
        else if (!loan.HasNoBadLoans(cust))
        {
            eligible = false;
        }
        else if (!credit.HasGoodCredit(cust))
        {
            eligible = false;
        }

        return eligible;
    }
}

/// <summary>
/// Customer class
/// </summary>
public record Customer(string Name);
```

### 輸出結果 (Output)

```text
Ann McKinsey applies for $125,000.00 loan

    

    Check bank for Ann McKinsey

    Check loans for Ann McKinsey

    Check credit for Ann McKinsey

    

    Ann McKinsey has been Approved
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Decorator](Decorator.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Flyweight →](Flyweight.md) |

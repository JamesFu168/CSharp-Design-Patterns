# C# State Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：中 (Medium)  
> **原文連結**：[https://www.dofactory.com/net/state-design-pattern](https://www.dofactory.com/net/state-design-pattern)

---

## 定義與概述 (Definition)

**State** Design Pattern 允許一個物件在其內部狀態改變時改變其行為。該物件看起來就像是改變了它的類別一樣。

**State** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![State UML Class Diagram](images/state.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Context** (`Account`)
  - 定義用戶端（Clients）感興趣的介面。
  - 維護一個定義目前狀態之 `ConcreteState` 子類別的實例（Instance）。
- **State** (`State`)
  - 定義一個介面，用來封裝與 `Context` 之特定狀態相關聯的行為。
- **Concrete State** (`RedState`, `SilverState`, `GoldState`)
  - 每一個子類別都實作一個與 `Context` 某個狀態相關聯的行為。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 State Pattern 如何讓物件根據其內部狀態展現不同的行為，並能在執行期間動態切換狀態物件。

```csharp
using System;

namespace State.Structural
{
    /// <summary>
    /// State Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup context in a state

            var context = new Context(new ConcreteStateA());

            // Issue requests, which toggles state

            context.Request();
            context.Request();
            context.Request();
            context.Request();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'State' abstract class
    /// </summary>

    public abstract class State
    {
        public abstract void Handle(Context context);
    }

    /// <summary>
    /// A 'ConcreteState' class
    /// </summary>

    public class ConcreteStateA : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateB();
        }
    }

    /// <summary>
    /// A 'ConcreteState' class
    /// </summary>

    public class ConcreteStateB : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateA();
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class Context
    {
        State state;

        // Constructor

        public Context(State state)
        {
            this.State = state;
        }

        // Gets or sets the state

        public State State
        {
            get { return state; }
            set
            {
                state = value;
                Console.WriteLine("State: " + state.GetType().Name);
            }
        }

        public void Request()
        {
            state.Handle(this);
        }
    }
}
```

### 輸出結果 (Output)

```text
State: ConcreteStateA

  State: ConcreteStateB

  State: ConcreteStateA

  State: ConcreteStateB

  State: ConcreteStateA
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 State Pattern 如何讓銀行帳戶（`Account`）根據其餘額（Balance）展現不同的行為。行為上的差異被委派（Delegated）給名為 `RedState`、`SilverState` 與 `GoldState` 的 `State` 物件。這些狀態分別代表透支帳戶、新開立帳戶以及信用良好的帳戶。

```csharp
using System;

namespace State.RealWorld
{
    /// <summary>
    /// State Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Open a new account

            Account account = new Account("Jim Johnson");

            // Apply financial transactions

            account.Deposit(500.0);
            account.Deposit(300.0);
            account.Deposit(550.0);
            account.PayInterest();
            account.Withdraw(2000.00);
            account.Withdraw(1100.00);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'State' abstract class
    /// </summary>

    public abstract class State
    {
        protected Account account;
        protected double balance;

        protected double interest;
        protected double lowerLimit;
        protected double upperLimit;

        // Properties

        public Account Account
        {
            get { return account; }
            set { account = value; }
        }

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public abstract void Deposit(double amount);
        public abstract void Withdraw(double amount);
        public abstract void PayInterest();
    }


    /// <summary>
    /// A 'ConcreteState' class
    /// <remarks>
    /// Red indicates that account is overdrawn 
    /// </remarks>
    /// </summary>

    public class RedState : State
    {
        private double serviceFee;

        // Constructor

        public RedState(State state)
        {
            this.balance = state.Balance;
            this.account = state.Account;
            Initialize();
        }

        private void Initialize()
        {
            // Should come from a datasource

            interest = 0.0;
            lowerLimit = -100.0;
            upperLimit = 0.0;
            serviceFee = 15.00;
        }

        public override void Deposit(double amount)
        {
            balance += amount;
            StateChangeCheck();
        }

        public override void Withdraw(double amount)
        {
            amount = amount - serviceFee;
            Console.WriteLine("No funds available for withdrawal!");
        }

        public override void PayInterest()
        {
            // No interest is paid
        }

        private void StateChangeCheck()
        {
            if (balance > upperLimit)
            {
                account.State = new SilverState(this);
            }
        }
    }

    /// <summary>
    /// A 'ConcreteState' class
    /// <remarks>
    /// Silver indicates a non-interest bearing state
    /// </remarks>
    /// </summary>

    public class SilverState : State
    {
        // Overloaded constructors

        public SilverState(State state) :
            this(state.Balance, state.Account)
        {
        }

        public SilverState(double balance, Account account)
        {
            this.balance = balance;
            this.account = account;
            Initialize();
        }

        private void Initialize()
        {
            // Should come from a datasource
            interest = 0.0;
            lowerLimit = 0.0;
            upperLimit = 1000.0;
        }

        public override void Deposit(double amount)
        {
            balance += amount;
            StateChangeCheck();
        }

        public override void Withdraw(double amount)
        {
            balance -= amount;
            StateChangeCheck();
        }

        public override void PayInterest()
        {
            balance += interest * balance;
            StateChangeCheck();
        }

        private void StateChangeCheck()
        {
            if (balance < lowerLimit)
            {
                account.State = new RedState(this);
            }
            else if (balance > upperLimit)
            {
                account.State = new GoldState(this);
            }
        }
    }

    /// <summary>
    /// A 'ConcreteState' class
    /// <remarks>
    /// Gold indicates an interest bearing state
    /// </remarks>
    /// </summary>

    public class GoldState : State
    {
        // Overloaded constructors
        public GoldState(State state)
            : this(state.Balance, state.Account)
        {
        }

        public GoldState(double balance, Account account)
        {
            this.balance = balance;
            this.account = account;
            Initialize();
        }

        private void Initialize()
        {
            // Should come from a database
            interest = 0.05;
            lowerLimit = 1000.0;
            upperLimit = 10000000.0;
        }

        public override void Deposit(double amount)
        {
            balance += amount;
            StateChangeCheck();
        }

        public override void Withdraw(double amount)
        {
            balance -= amount;
            StateChangeCheck();
        }

        public override void PayInterest()
        {
            balance += interest * balance;
            StateChangeCheck();
        }

        private void StateChangeCheck()
        {
            if (balance < 0.0)
            {
                account.State = new RedState(this);
            }
            else if (balance < lowerLimit)
            {
                account.State = new SilverState(this);
            }
        }
    }

    /// <summary>
    /// The 'Context' class
    /// </summary>

    public class Account
    {
        private State state;
        private string owner;

        // Constructor

        public Account(string owner)
        {
            // New accounts are 'Silver' by default
            this.owner = owner;
            this.state = new SilverState(0.0, this);
        }

        public double Balance
        {
            get { return state.Balance; }
        }

        public State State
        {
            get { return state; }
            set { state = value; }
        }

        public void Deposit(double amount)
        {
            state.Deposit(amount);
            Console.WriteLine("Deposited {0:C} --- ", amount);
            Console.WriteLine(" Balance = {0:C}", this.Balance);
            Console.WriteLine(" Status  = {0}",
                this.State.GetType().Name);
            Console.WriteLine("");
        }

        public void Withdraw(double amount)
        {
            state.Withdraw(amount);
            Console.WriteLine("Withdrew {0:C} --- ", amount);
            Console.WriteLine(" Balance = {0:C}", this.Balance);
            Console.WriteLine(" Status  = {0}\n",
                this.State.GetType().Name);
        }

        public void PayInterest()
        {
            state.PayInterest();
            Console.WriteLine("Interest Paid --- ");
            Console.WriteLine(" Balance = {0:C}", this.Balance);
            Console.WriteLine(" Status  = {0}\n",
                this.State.GetType().Name);
        }
    }
}
```

### 輸出結果 (Output)

```text
Deposited $500.00 ---

   Balance = $500.00

   Status  = SilverState

  

  

  Deposited $300.00 ---

   Balance = $800.00

   Status  = SilverState

  

  

  Deposited $550.00 ---

   Balance = $1,350.00

   Status  = GoldState

  

  

  Interest Paid ---

   Balance = $1,417.50

   Status  = GoldState

  

  Withdrew $2,000.00 ---

   Balance = ($582.50)

   Status  = RedState

  

  No funds available for withdrawal!

  Withdrew $1,100.00 ---

   Balance = ($582.50)

   Status  = RedState
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能。這是一個優雅的 C# State 解決方案。

```csharp
namespace State.NetOptimized;

using static System.Console;

/// <summary>
/// State Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Open a new account
        var account = new Account("Jim Johnson");

        // Apply financial transactions
        account.Deposit(500.0);
        account.Deposit(300.0);
        account.Deposit(550.0);
        account.PayInterest();
        account.Withdraw(2000.00);
        account.Withdraw(1100.00);

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'State' abstract class
/// </summary>
public abstract class State
{
    protected double interest;
    protected double lowerLimit;
    protected double upperLimit;

    // Gets or sets the account
    public Account Account { get; set; } = null!;

    // Gets or sets the balance
    public double Balance { get; set; }

    public abstract void Deposit(double amount);
    public abstract void Withdraw(double amount);
    public abstract void PayInterest();
}

/// <summary>
/// A 'ConcreteState' class
/// <remarks>
/// Red state indicates account is overdrawn 
/// </remarks>
/// </summary>
public class RedState : State
{
    private double serviceFee;

    // Constructor
    public RedState(State state)
    {
        Balance = state.Balance;
        Account = state.Account;
        Initialize();
    }

    private void Initialize()
    {
        // Should come from a datasource
        interest = 0.0;
        lowerLimit = -100.0;
        upperLimit = 0.0;
        serviceFee = 15.00;
    }

    public override void Deposit(double amount)
    {
        Balance += amount;
        StateChangeCheck();
    }

    public override void Withdraw(double amount)
    {
        amount -= serviceFee;
        WriteLine("No funds available for withdrawal!");
    }

    public override void PayInterest()
    {
        // No interest is paid
    }

    private void StateChangeCheck()
    {
        if (Balance > upperLimit)
        {
            Account.State = new SilverState(this);
        }
    }
}

/// <summary>
/// A 'ConcreteState' class
/// <remarks>
/// Silver state is non-interest bearing state
/// </remarks>
/// </summary>
public class SilverState : State
{
    // Overloaded constructors

    public SilverState(State state) :
        this(state.Balance, state.Account)
    {
    }

    public SilverState(double balance, Account account)
    {
        Balance = balance;
        Account = account;
        Initialize();
    }

    private void Initialize()
    {
        // Should come from a datasource
        interest = 0.0;
        lowerLimit = 0.0;
        upperLimit = 1000.0;
    }

    public override void Deposit(double amount)
    {
        Balance += amount;
        StateChangeCheck();
    }

    public override void Withdraw(double amount)
    {
        Balance -= amount;
        StateChangeCheck();
    }

    public override void PayInterest()
    {
        Balance += interest * Balance;
        StateChangeCheck();
    }

    private void StateChangeCheck()
    {
        if (Balance < lowerLimit)
        {
            Account.State = new RedState(this);
        }
        else if (Balance > upperLimit)
        {
            Account.State = new GoldState(this);
        }
    }
}

/// <summary>
/// A 'ConcreteState' class
/// <remarks>
/// Gold incidates interest bearing state
/// </remarks>
/// </summary>
public class GoldState : State
{
    // Overloaded constructors
    public GoldState(State state)
        : this(state.Balance, state.Account)
    {
    }

    public GoldState(double balance, Account account)
    {
        Balance = balance;
        Account = account;
        Initialize();
    }

    private void Initialize()
    {
        // Should come from a database
        interest = 0.05;
        lowerLimit = 1000.0;
        upperLimit = 10000000.0;
    }

    public override void Deposit(double amount)
    {
        Balance += amount;
        StateChangeCheck();
    }

    public override void Withdraw(double amount)
    {
        Balance -= amount;
        StateChangeCheck();
    }

    public override void PayInterest()
    {
        Balance += interest * Balance;
        StateChangeCheck();
    }

    private void StateChangeCheck()
    {
        if (Balance < 0.0)
        {
            Account.State = new RedState(this);
        }
        else if (Balance < lowerLimit)
        {
            Account.State = new SilverState(this);
        }
    }
}

/// <summary>
/// The 'Context' class
/// </summary>
public class Account
{
    private readonly string owner;

    // Constructor
    public Account(string owner)
    {
        // New accounts are 'Silver' by default
        this.owner = owner;
        this.State = new SilverState(0.0, this);
    }

    // Gets the balance
    public double Balance
    {
        get { return State.Balance; }
    }

    // Gets or sets state
    public State State { get; set; }

    public void Deposit(double amount)
    {
        State.Deposit(amount);
        WriteLine($"Owner: {owner}");
        WriteLine("Deposited {0:C} --- ", amount);
        WriteLine(" Balance = {0:C}", this.Balance);
        WriteLine(" Status  = {0}", this.State.GetType().Name);
        WriteLine("");
    }

    public void Withdraw(double amount)
    {
        State.Withdraw(amount);
        WriteLine($"Owner: {owner}"); 
        WriteLine("Withdrew {0:C} --- ", amount);
        WriteLine(" Balance = {0:C}", this.Balance);
        WriteLine(" Status  = {0}\n", this.State.GetType().Name);
    }

    public void PayInterest()
    {
        State.PayInterest();
        WriteLine("Interest Paid --- ");
        WriteLine(" Balance = {0:C}", this.Balance);
        WriteLine(" Status  = {0}\n", this.State.GetType().Name);
    }
}
```

### 輸出結果 (Output)

```text
Deposited $500.00 ---

     Balance = $500.00

     Status  = SilverState

    

    

    Deposited $300.00 ---

     Balance = $800.00

     Status  = SilverState

    

    

    Deposited $550.00 ---

     Balance = $1,350.00

     Status  = GoldState

    

    

    Interest Paid ---

     Balance = $1,417.50

     Status  = GoldState

    

    Withdrew $2,000.00 ---

     Balance = ($582.50)

     Status  = RedState

    

    No funds available for withdrawal!

    Withdrew $1,100.00 ---

     Balance = ($582.50)

     Status  = RedState
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Observer](Observer.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Strategy →](Strategy.md) |

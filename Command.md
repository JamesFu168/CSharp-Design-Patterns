# C# Command Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/command-design-pattern](https://www.dofactory.com/net/command-design-pattern)

---

## 定義與概述 (Definition)

**Command** Design Pattern 將請求封裝為一個物件，從而讓你可以用不同的請求來參數化（Parameterize）用戶端、將請求排入佇列（Queue）或記錄日誌（Log），以及支援可復原（Undoable）的操作。

**Command** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Command UML Class Diagram](images/command.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Command** (`Command`)
  - 宣告一個用於執行操作的介面。
- **ConcreteCommand** (`CalculatorCommand`)
  - 定義接收者（`Receiver`）物件與動作之間的綁定（Binding）。
  - 透過呼叫 `Receiver` 上對應的操作來實作 `Execute`。
- **Client** (`CommandApp`)
  - 建立一個 `ConcreteCommand` 物件並設定其接收者（Receiver）。
- **Invoker** (`User`)
  - 要求該命令（Command）執行請求。
- **Receiver** (`Calculator`)
  - 知道如何執行與完成該請求相關的各種操作。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Command Pattern 如何將請求儲存為物件，讓用戶端（Clients）能夠執行或重播（Playback）這些請求。

```csharp
using System;

namespace Command.Structural
{
    /// <summary>
    /// Command Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create receiver, command, and invoker

            Receiver receiver = new Receiver();
            Command command = new ConcreteCommand(receiver);
            Invoker invoker = new Invoker();

            // Set and execute command

            invoker.SetCommand(command);
            invoker.ExecuteCommand();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Command' abstract class
    /// </summary>

    public abstract class Command
    {
        protected Receiver receiver;

        // Constructor

        public Command(Receiver receiver)
        {
            this.receiver = receiver;
        }

        public abstract void Execute();
    }

    /// <summary>
    /// The 'ConcreteCommand' class
    /// </summary>

    public class ConcreteCommand : Command
    {
        // Constructor

        public ConcreteCommand(Receiver receiver) :
            base(receiver)
        {
        }

        public override void Execute()
        {
            receiver.Action();
        }
    }

    /// <summary>
    /// The 'Receiver' class
    /// </summary>

    public class Receiver
    {
        public void Action()
        {
            Console.WriteLine("Called Receiver.Action()");
        }
    }

    /// <summary>
    /// The 'Invoker' class
    /// </summary>

    public class Invoker
    {
        Command command;

        public void SetCommand(Command command)
        {
            this.command = command;
        }

        public void ExecuteCommand()
        {
            command.Execute();
        }
    }
}
```

### 輸出結果 (Output)

```text
Called Receiver.Action()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了應用於簡易計算機的 Command Pattern，並支援無限次數的復原（Undo）與重做（Redo）。請注意，在 C# 中 `operator` 是保留關鍵字，在其前方加上 `@` 前綴（即 `@operator`）即可將其作為識別字（Identifier）使用。

```csharp
using System;
using System.Collections.Generic;

namespace Command.RealWorld
{
    /// <summary>
    /// Command Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create user and let her compute

            User user = new User();

            // User presses calculator buttons

            user.Compute('+', 100);
            user.Compute('-', 50);
            user.Compute('*', 10);
            user.Compute('/', 2);

            // Undo 4 commands

            user.Undo(4);

            // Redo 3 commands

            user.Redo(3);

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Command' abstract class
    /// </summary>

    public abstract class Command
    {
        public abstract void Execute();
        public abstract void UnExecute();
    }

    /// <summary>
    /// The 'ConcreteCommand' class
    /// </summary>

    public class CalculatorCommand : Command
    {
        char @operator;
        int operand;
        Calculator calculator;

        // Constructor

        public CalculatorCommand(Calculator calculator,
            char @operator, int operand)
        {
            this.calculator = calculator;
            this.@operator = @operator;
            this.operand = operand;
        }

        // Gets operator

        public char Operator
        {
            set { @operator = value; }
        }

        // Get operand

        public int Operand
        {
            set { operand = value; }
        }

        // Execute new command

        public override void Execute()
        {
            calculator.Operation(@operator, operand);
        }

        // Unexecute last command

        public override void UnExecute()
        {
            calculator.Operation(Undo(@operator), operand);
        }

        // Returns opposite operator for given operator

        private char Undo(char @operator)
        {
            switch (@operator)
            {
                case '+': return '-';
                case '-': return '+';
                case '*': return '/';
                case '/': return '*';
                default:
                    throw new
             ArgumentException("@operator");
            }
        }
    }

    /// <summary>
    /// The 'Receiver' class
    /// </summary>

    public class Calculator
    {
        int curr = 0;

        public void Operation(char @operator, int operand)
        {
            switch (@operator)
            {
                case '+': curr += operand; break;
                case '-': curr -= operand; break;
                case '*': curr *= operand; break;
                case '/': curr /= operand; break;
            }
            Console.WriteLine(
                "Current value = {0,3} (following {1} {2})",
                curr, @operator, operand);
        }
    }

    /// <summary>
    /// The 'Invoker' class
    /// </summary>

    public class User
    {
        // Initializers

        Calculator calculator = new Calculator();
        List<Command> commands = new List<Command>();
        int current = 0;

        public void Redo(int levels)
        {
            Console.WriteLine("\n---- Redo {0} levels ", levels);
            // Perform redo operations
            for (int i = 0; i < levels; i++)
            {
                if (current < commands.Count - 1)
                {
                    Command command = commands[current++];
                    command.Execute();
                }
            }
        }

        public void Undo(int levels)
        {
            Console.WriteLine("\n---- Undo {0} levels ", levels);
            
            // Perform undo operations

            for (int i = 0; i < levels; i++)
            {
                if (current > 0)
                {
                    Command command = commands[--current] as Command;
                    command.UnExecute();
                }
            }
        }

        public void Compute(char @operator, int operand)
        {
            // Create command operation and execute it

            Command command = new CalculatorCommand(calculator, @operator, operand);
            command.Execute();

            // Add command to undo list

            commands.Add(command);
            current++;
        }
    }
}
```

### 輸出結果 (Output)

```text
Current value = 100 (following + 100)

  Current value =  50 (following - 50)

  Current value = 500 (following * 10)

  Current value = 250 (following / 2)

  

  ---- Undo 4 levels

  Current value = 500 (following * 2)

  Current value =  50 (following / 10)

  Current value = 100 (following + 50)

  Current value =   0 (following - 100)

  

  ---- Redo 3 levels

  Current value = 100 (following + 100)

  Current value =  50 (following - 50)

  Current value = 500 (following * 10)
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors 與 `switch` 運算式）。這是一個優雅的 C# Command 解決方案。

```csharp
namespace Command.NetOptimized;

using static System.Console;

using System;
using System.Collections.Generic;

/// <summary>
/// Command Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create user and let her compute
        var user = new User();

        // Issue several compute commands
        user.Compute('+', 100);
        user.Compute('-', 50);
        user.Compute('*', 10);
        user.Compute('/', 2);

        // Undo 4 commands
        user.Undo(4);

        // Redo 3 commands
        user.Redo(3);

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Command' interface
/// </summary>
public interface ICommand
{
    void Execute();
    void UnExecute();
}

/// <summary>
/// The 'ConcreteCommand' class
/// </summary>
public class CalculatorCommand( Calculator calculator, 
                                char @operator, 
                                int operand) : ICommand
{

    // Sets operator
    public char Operator { set => @operator = value; }

    // Sets operand
    public int Operand { set => operand = value; }

    // Execute command
    public void Execute()
    {
        calculator.Operation(@operator, operand);
    }

    // Unexecute command
    public void UnExecute()
    {
        calculator.Operation(Undo(@operator), operand);
    }

    // Return opposite operator for given operator
    private static char Undo(char @operator)
    {
        return @operator switch
        {
            '+' => '-',
            '-' => '+',
            '*' => '/',
            '/' => '*',
            _ => throw new ArgumentException("@operator"),
        };
    }
}

/// <summary>
/// The 'Receiver' class
/// </summary>
public class Calculator
{
    private int current = 0;

    // Perform operation for given operator and operand
    public void Operation(char @operator, int operand)
    {
        switch (@operator)
        {
            case '+': current += operand; break;
            case '-': current -= operand; break;
            case '*': current *= operand; break;
            case '/': current /= operand; break;
        }
        WriteLine(
            "Current value = {0,3} (following {1} {2})",
            current, @operator, operand);
    }
}

/// <summary>
/// The 'Invoker' class
/// </summary>
public class User
{
    private readonly Calculator calculator = new();
    private readonly List<ICommand> commands = [];
    private int current = 0;

    // Redo original commands
    public void Redo(int levels)
    {
        WriteLine($"\n---- Redo {levels} levels ");

        // Perform redo operations
        for (int i = 0; i < levels; i++)
        {
            if (current < commands.Count - 1)
            {
                commands[current++].Execute();
            }
        }
    }

    // Undo prior commands
    public void Undo(int levels)
    {
        WriteLine($"\n---- Undo {levels} levels ");

        // Perform undo operations
        for (int i = 0; i < levels; i++)
        {
            if (current > 0)
            {
                commands[--current].UnExecute();
            }
        }
    }

    // Compute new value given operator and operand
    public void Compute(char @operator, int operand)
    {
        // Create command operation and execute it
        var command = new CalculatorCommand(calculator, @operator, operand);
        command.Execute();

        // Add command to undo list
        commands.Add(command);
        current++;
    }
}
```

### 輸出結果 (Output)

```text
Current value = 100 (following + 100)

    Current value =  50 (following - 50)

    Current value = 500 (following * 10)

    Current value = 250 (following / 2)

    

    ---- Undo 4 levels

    Current value = 500 (following * 2)

    Current value =  50 (following / 10)

    Current value = 100 (following + 50)

    Current value =   0 (following - 100)

    

    ---- Redo 3 levels

    Current value = 100 (following + 100)

    Current value =  50 (following - 50)

    Current value = 500 (following * 10)
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Chain of Responsibility](ChainOfResponsibility.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Interpreter →](Interpreter.md) |

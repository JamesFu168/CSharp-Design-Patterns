# C# Proxy Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/proxy-design-pattern](https://www.dofactory.com/net/proxy-design-pattern)

---

## 定義與概述 (Definition)

**Proxy** Design Pattern 為另一個物件提供一個替身（Surrogate）或佔位符（Placeholder），以控制對該物件的存取。

**Proxy** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Proxy UML Class Diagram](images/proxy.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Proxy** (`MathProxy`)
  - 維護一個參考（Reference），讓 Proxy 可以存取真實主體（`RealSubject`）。如果 `RealSubject` 與 `Subject` 的介面相同，`Proxy` 可以直接指向 `Subject`。
  - 提供一個與 `Subject` 完全相同的介面，以便在任何需要真實主體的地方都可以用 Proxy 來替代。
  - 控制對真實主體的存取，並可能負責建立與刪除它。
  - 其他職責取決於 Proxy 的類型：
    - *遠端代理（Remote Proxies）*：負責對請求及其引數（Arguments）進行編碼，並將編碼後的請求發送給位於不同位址空間（Address Space）中的真實主體。
    - *虛擬代理（Virtual Proxies）*：可以快取（Cache）關於真實主體的額外資訊，以便延遲對它的實際存取。例如，動機（Motivation）中的 `ImageProxy` 會快取真實圖片的尺寸範圍。
    - *保護代理（Protection Proxies）*：檢查呼叫者是否具備執行該請求所需的存取權限。
- **Subject** (`IMath`)
  - 為 `RealSubject` 與 `Proxy` 定義共同介面，使任何預期使用 `RealSubject` 的地方都可以使用 `Proxy`。
- **RealSubject** (`Math`)
  - 定義 `Proxy` 所代表的真實物件。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Proxy Pattern 如何提供一個代表物件（Proxy），用來控制對另一個相似物件的存取。

```csharp
using System;

namespace Proxy.Structural
{
    /// <summary>
    /// Proxy Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create proxy and request a service

            Proxy proxy = new Proxy();
            proxy.Request();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subject' abstract class
    /// </summary>

    public abstract class Subject
    {
        public abstract void Request();
    }

    /// <summary>
    /// The 'RealSubject' class
    /// </summary>

    public class RealSubject : Subject
    {
        public override void Request()
        {
            Console.WriteLine("Called RealSubject.Request()");
        }
    }

    /// <summary>
    /// The 'Proxy' class
    /// </summary>

    public class Proxy : Subject
    {
        private RealSubject realSubject;

        public override void Request()
        {
            // Use 'lazy initialization'

            if (realSubject == null)
            {
                realSubject = new RealSubject();
            }

            realSubject.Request();
        }
    }
}
```

### 輸出結果 (Output)

```text
Called RealSubject.Request()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了針對 `Math` 物件的 Proxy Pattern，該物件由一個 `MathProxy` 物件所代表。

```csharp
using System;

namespace Proxy.RealWorld
{
    /// <summary>
    /// Proxy Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create math proxy

            MathProxy proxy = new MathProxy();

            // Do the math

            Console.WriteLine("4 + 2 = " + proxy.Add(4, 2));
            Console.WriteLine("4 - 2 = " + proxy.Sub(4, 2));
            Console.WriteLine("4 * 2 = " + proxy.Mul(4, 2));
            Console.WriteLine("4 / 2 = " + proxy.Div(4, 2));

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Subject interface
    /// </summary>

    public interface IMath
    {
        double Add(double x, double y);
        double Sub(double x, double y);
        double Mul(double x, double y);
        double Div(double x, double y);
    }

    /// <summary>
    /// The 'RealSubject' class
    /// </summary>

    public class Math : IMath
    {
        public double Add(double x, double y) { return x + y; }
        public double Sub(double x, double y) { return x - y; }
        public double Mul(double x, double y) { return x * y; }
        public double Div(double x, double y) { return x / y; }
    }

    /// <summary>
    /// The 'Proxy Object' class
    /// </summary>

    public class MathProxy : IMath
    {
        private Math math = new Math();

        public double Add(double x, double y)
        {
            return math.Add(x, y);
        }
        public double Sub(double x, double y)
        {
            return math.Sub(x, y);
        }
        public double Mul(double x, double y)
        {
            return math.Mul(x, y);
        }
        public double Div(double x, double y)
        {
            return math.Div(x, y);
        }
    }
}
```

### 輸出結果 (Output)

```text
4 + 2 = 6

  4 - 2 = 2

  4 * 2 = 8

  4 / 2 = 2
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如運算式主體方法 Expression-bodied Methods 與字串插值）。這是一個優雅的 C# Proxy 解決方案。

```csharp
namespace Proxy.NetOptimized;

using static System.Console;
using System.Threading;

/// <summary>
/// Proxy Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create math proxy
        var proxy = new MathProxy();

        // Do the math
        WriteLine($"4 + 2 = {proxy.Add(4, 2)}");
        WriteLine($"4 - 2 = {proxy.Sub(4, 2)}");
        WriteLine($"4 * 2 = {proxy.Mul(4, 2)}");
        WriteLine($"4 / 2 = {proxy.Div(4, 2)}");

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Subject' interface
/// </summary>
public interface IMath
{
    double Add(double x, double y);
    double Sub(double x, double y);
    double Mul(double x, double y);
    double Div(double x, double y);
}

/// <summary>
/// The 'RealSubject' class
/// </summary>
public class Math : IMath
{
    public double Add(double x, double y) => x + y;
    public double Sub(double x, double y) => x - y;
    public double Mul(double x, double y) => x * y;
    public double Div(double x, double y) => x / y;
}

/// <summary>
/// The remote 'Proxy Object' class
/// </summary>
public class MathProxy : IMath
{
    private readonly Math math = new();

    public double Add(double x, double y)
    {
        Thread.Sleep(800);
        return math.Add(x, y);
    }

    public double Sub(double x, double y)
    {
        Thread.Sleep(800);
        return math.Sub(x, y);
    }

    public double Mul(double x, double y)
    {
        Thread.Sleep(800);
        return math.Mul(x, y);
    }

    public double Div(double x, double y)
    {
        Thread.Sleep(800);
        return math.Div(x, y);
    }
}
```

### 輸出結果 (Output)

```text
4 + 2 = 6

    4 - 2 = 2

    4 * 2 = 8

    4 / 2 = 2
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Flyweight](Flyweight.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Chain of Responsibility →](ChainOfResponsibility.md) |

# C# Singleton Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/singleton-design-pattern](https://www.dofactory.com/net/singleton-design-pattern)

---

## 定義與概述 (Definition)

**Singleton** Design Pattern 確保一個類別只有一個實例（Instance），並提供一個存取它的全域存取點（Global Point of Access）。

**Singleton** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Singleton UML Class Diagram](images/Singleton.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Singleton** (`LoadBalancer`)
  - 定義一個 `Instance` 操作，讓用戶端（Clients）可以存取其唯一的實例。`Instance` 是一個類別層級（靜態，`static`）的操作。
  - 負責建立並維護其自身唯一的實例。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Singleton Pattern 如何確保該類別只能建立單一實例（即 Singleton）。

```csharp
using System;

namespace Singleton.Structural
{
    /// <summary>
    /// Singleton Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Constructor is protected -- cannot use new

            Singleton s1 = Singleton.Instance();
            Singleton s2 = Singleton.Instance();

            // Test for same instance

            if (s1 == s2)
            {
                Console.WriteLine("Objects are the same instance");
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Singleton' class
    /// </summary>

    public class Singleton
    {
        static Singleton instance;

        // Constructor is 'protected'

        protected Singleton()
        {
        }

        public static Singleton Instance()
        {
            // Uses lazy initialization.
            // Note: this is not thread safe.
            if (instance == null)
            {
                instance = new Singleton();
            }

            return instance;
        }
    }
}
```

### 輸出結果 (Output)

```text
Objects are the same instance
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼將 Singleton Pattern 示範為一個負載平衡（`LoadBalancer`）物件。該類別只能建立單一實例（Singleton），因為伺服器可能會動態地上線或離線，且每個請求都必須透過這唯一一個掌握（Web）伺服器叢集（Farm）狀態的物件來處理。

```csharp
using System;
using System.Collections.Generic;

namespace Singleton.RealWorld
{
    /// <summary>
    /// Singleton Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            LoadBalancer b1 = LoadBalancer.GetLoadBalancer();
            LoadBalancer b2 = LoadBalancer.GetLoadBalancer();
            LoadBalancer b3 = LoadBalancer.GetLoadBalancer();
            LoadBalancer b4 = LoadBalancer.GetLoadBalancer();

            // Same instance?

            if (b1 == b2 && b2 == b3 && b3 == b4)
            {
                Console.WriteLine("Same instance\n");
            }

            // Load balance 15 server requests

            LoadBalancer balancer = LoadBalancer.GetLoadBalancer();
            for (int i = 0; i < 15; i++)
            {
                string server = balancer.Server;
                Console.WriteLine("Dispatch Request to: " + server);
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Singleton' class
    /// </summary>

    public class LoadBalancer
    {
        static LoadBalancer instance;
        List<string> servers = new List<string>();
        Random random = new Random();

        // Lock synchronization object

        private static object locker = new object();

        // Constructor (protected)

        protected LoadBalancer()
        {
            // List of available servers
            servers.Add("ServerI");
            servers.Add("ServerII");
            servers.Add("ServerIII");
            servers.Add("ServerIV");
            servers.Add("ServerV");
        }

        public static LoadBalancer GetLoadBalancer()
        {
            // Support multithreaded applications through
            // 'Double checked locking' pattern which (once
            // the instance exists) avoids locking each
            // time the method is invoked

            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new LoadBalancer();
                    }
                }
            }

            return instance;
        }

        // Simple, but effective random load balancer

        public string Server
        {
            get
            {
                int r = random.Next(servers.Count);
                return servers[r].ToString();
            }
        }
    }
}
```

### 輸出結果 (Output)

```text
Same instance

  

  ServerIII

  ServerII

  ServerI

  ServerII

  ServerI

  ServerIII

  ServerI

  ServerIII

  ServerIV

  ServerII

  ServerII

  ServerIII

  ServerIV

  ServerII

  ServerIV
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化、內建的 .NET 功能（例如 `sealed` 類別、唯讀靜態欄位初始化與 `record` 型別）。這是一個優雅的 Singleton 解決方案，其執行緒安全（Thread Safety）由編譯器直接保證。

```csharp
namespace Singleton.NetOptimized;

using static System.Console;
using System;
using System.Collections.Generic;

public class Program
{
    /// <summary>
    /// Singleton Design Pattern
    /// </summary>
    public static void Main()
    {
        var b1 = LoadBalancer.GetLoadBalancer();
        var b2 = LoadBalancer.GetLoadBalancer();
        var b3 = LoadBalancer.GetLoadBalancer();
        var b4 = LoadBalancer.GetLoadBalancer();

        // Confirm these are the same instance
        if (b1 == b2 && b2 == b3 && b3 == b4)
        {
            WriteLine("Same instance\n");
        }

        var balancer = LoadBalancer.GetLoadBalancer();

        // Load balance 15 requests for a server
        for (int i = 0; i < 15; i++)
        {
            var server = balancer.NextServer.Name;
            WriteLine("Dispatch request to: " + server);
        }

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Singleton' class
/// </summary>
public sealed class LoadBalancer
{
    // Static members are 'eagerly initialized', that is, 
    // immediately when class is loaded for the first time.
    // .NET guarantees thread safety for static initialization
    private static readonly LoadBalancer instance = new();

    private readonly List<Server> servers;
    private readonly Random random = new();

    // Note: constructor is 'private'
    private LoadBalancer()
    {
        // Load list of available servers

        servers = [
                    new(Name: "ServerI", Ip: "120.14.220.18"),
                    new(Name: "ServerII", Ip: "120.14.220.19" ),
                    new(Name: "ServerIII", Ip: "120.14.220.20" ),
                    new(Name: "ServerIV", Ip: "120.14.220.21" ),
                    new(Name: "ServerV", Ip: "120.14.220.22" )
                  ];
    }

    public static LoadBalancer GetLoadBalancer()
    {
        return instance;
    }

    // Simple, but effective load balancer
    public Server NextServer
    {
        get => servers[random.Next(servers.Count)];
    }
}

/// <summary>
/// Represents a server machine
/// </summary>
public record Server (string Name, string Ip);
```

### 輸出結果 (Output)

```text
Same instance

  

  ServerIV

  ServerIV

  ServerIII

  ServerV

  ServerII

  ServerV

  ServerII

  ServerII

  ServerI

  ServerIV

  ServerIV

  ServerII

  ServerI

  ServerV

  ServerIV
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **全域靜態耦合與可測試性問題**：手刻 `LoadBalancer.GetLoadBalancer()` 會造成呼叫端與具體靜態類別緊密耦合，難以在單元測試（Unit Testing）中替換為 Mock 或 Fake。
- **執行緒安全與隨機數產生器**：Dofactory 範例中在 Singleton 內部使用 `new Random()`（在舊版 .NET 中非執行緒安全）。在現代 .NET 中應使用執行緒安全的 `Random.Shared`。
- **實務首選 DI 容器**：在 .NET 11 實務專案中，應優先透過 `services.AddSingleton<ILoadBalancer, LoadBalancer>()` 交由 DI 容器管理生命週期。

### C# 15 / .NET 11 優化重點

- **`Random.Shared`（執行緒安全隨機存取）**：取代手動實例化 `new Random()`。
- **C# 15 Collection Expression Arguments (`with(...)`)** 與 **`FrozenArray` / `ImmutableArray`**：確保 Singleton 內部的伺服器清單不可變且具備最佳讀取效能。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Singleton.ModernNet11;

using System.Collections.Immutable;
using static System.Console;

public static class Program
{
    public static void Main()
    {
        var b1 = LoadBalancer.Instance;
        var b2 = LoadBalancer.Instance;

        if (ReferenceEquals(b1, b2))
        {
            WriteLine("Same instance\n");
        }

        for (int i = 0; i < 5; i++)
        {
            WriteLine($"Dispatch request to: {b1.NextServer.Name} ({b1.NextServer.Ip})");
        }
    }
}

public sealed record Server(string Name, string Ip);

/// <summary>
/// 現代化 Singleton：使用靜態屬性 Instance、ImmutableArray 與執行緒安全的 Random.Shared
/// （註：在 ASP.NET Core / .NET 11 實務中，建議改用 DI 容器的 services.AddSingleton）
/// </summary>
public sealed class LoadBalancer
{
    public static LoadBalancer Instance { get; } = new();

    private readonly ImmutableArray<Server> _servers =
    [
        new("ServerI", "120.14.220.18"),
        new("ServerII", "120.14.220.19"),
        new("ServerIII", "120.14.220.20"),
        new("ServerIV", "120.14.220.21"),
        new("ServerV", "120.14.220.22")
    ];

    private LoadBalancer() { }

    // Random.Shared 為 .NET 內建的執行緒安全隨機數產生器
    public Server NextServer => _servers[Random.Shared.Next(_servers.Length)];
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Prototype](Prototype.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Adapter →](Adapter.md) |

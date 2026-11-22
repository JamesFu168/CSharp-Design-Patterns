# C# Singleton Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：中高 (Medium-High)  
> **原文連結**：[https://www.dofactory.com/net/singleton-design-pattern](https://www.dofactory.com/net/singleton-design-pattern)

---

## 定義與概述 (Definition)

**Singleton** Design Pattern 確保一個類別只有一個實例（Instance），並提供一個存取它的全域存取點（Global Point of Access）。

**Singleton** Design Pattern 的 C# 程式碼範例提供以下 3 種形式：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)

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

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Prototype](Prototype.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Adapter →](Adapter.md) |

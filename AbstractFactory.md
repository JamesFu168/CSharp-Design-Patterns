# C# Abstract Factory Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/abstract-factory-design-pattern](https://www.dofactory.com/net/abstract-factory-design-pattern)

---

## 定義與概述 (Definition)

**Abstract Factory** Design Pattern 提供一個介面（Interface），用於建立相關或相依物件的家族，而無須指定它們的具體類別（Concrete Classes）。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

動物世界裡，非洲有牛羚與獅子，美洲有野牛與狼。如果讓使用這些動物的程式碼自己決定要建立哪一個類別，會寫成這樣：

```csharp
// 示意：呼叫端自己判斷要 new 哪一個類別
IHerbivore herbivore = continent == "Africa" ? new Wildebeest() : new Bison();
ICarnivore carnivore = continent == "Africa" ? new Lion() : new Wolf();
```

這種寫法有三個問題：

- 每個需要建立動物的地方，都要重複一次「是哪一洲」的判斷。
- 新增一洲時，必須找出所有判斷的地方逐一修改。
- 沒有任何機制能防止「獅子配野牛」這種不同家族混用的錯誤。

Abstract Factory 的做法是：把「同一個家族的物件」交給同一個工廠建立，呼叫端只要拿到工廠，就一定拿到整組互相搭配的產品。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Abstract Factory UML Class Diagram](images/abstract.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **AbstractFactory** (`ContinentFactory`)
  - 宣告用於建立抽象產品（Abstract Products）之操作的介面。
- **ConcreteFactory** (`AfricaFactory`, `AmericaFactory`)
  - 實作建立具體產品（Concrete Product）物件的操作。
- **AbstractProduct** (`Herbivore`, `Carnivore`)
  - 為一種類型的產品物件宣告介面。
- **Product** (`Wildebeest`, `Lion`, `Bison`, `Wolf`)
  - 定義將由對應的具體工廠（Concrete Factory）所建立的產品物件。
  - 實作 `AbstractProduct` 介面。
- **Client** (`AnimalWorld`)
  - 僅使用由 `AbstractFactory` 與 `AbstractProduct` 類別所宣告的介面。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/AbstractFactory](samples/AbstractFactory)  
> **執行方式**：`dotnet run --project samples/AbstractFactory`

**角色對照**：AbstractFactory = `IContinentFactory`；ConcreteFactory = `AfricaFactory`、`AmericaFactory`；AbstractProduct = `IHerbivore`、`ICarnivore`；ConcreteProduct = `Wildebeest`、`Bison`、`Lion`、`Wolf`；Client = `AnimalWorld`。

<!-- sample:AbstractFactory/Program.cs -->
```csharp
// Abstract Factory：一次建立「一整組互相搭配的物件」，並保證不會混用不同家族的產品。

// 要用哪一個產品家族，到執行期才決定（實務上常來自設定檔或使用者的選擇）
string[] continents = ["Africa", "America"];

foreach (string continent in continents)
{
    IContinentFactory factory = ContinentFactories.Create(continent);
    var world = new AnimalWorld(factory);
    world.RunFoodChain();
}

// ---- AbstractFactory：宣告「這個家族有哪些產品」 ----
public interface IContinentFactory
{
    IHerbivore CreateHerbivore();
    ICarnivore CreateCarnivore();
}

// ---- ConcreteFactory：每個工廠只生產同一洲的動物 ----
public sealed class AfricaFactory : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Wildebeest();
    public ICarnivore CreateCarnivore() => new Lion();
}

public sealed class AmericaFactory : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Bison();
    public ICarnivore CreateCarnivore() => new Wolf();
}

// ---- AbstractProduct ----
public interface IHerbivore
{
    string Name { get; }
}

public interface ICarnivore
{
    string Name { get; }
    string Eat(IHerbivore prey);
}

// ---- ConcreteProduct ----
public sealed class Wildebeest : IHerbivore
{
    public string Name => "Wildebeest";
}

public sealed class Bison : IHerbivore
{
    public string Name => "Bison";
}

public sealed class Lion : ICarnivore
{
    public string Name => "Lion";
    public string Eat(IHerbivore prey) => $"{Name} eats {prey.Name}";
}

public sealed class Wolf : ICarnivore
{
    public string Name => "Wolf";
    public string Eat(IHerbivore prey) => $"{Name} eats {prey.Name}";
}

// ---- Client：只認得介面，完全不知道自己拿到的是哪一洲的動物 ----
public sealed class AnimalWorld(IContinentFactory factory)
{
    private readonly IHerbivore _herbivore = factory.CreateHerbivore();
    private readonly ICarnivore _carnivore = factory.CreateCarnivore();

    public void RunFoodChain() => Console.WriteLine(_carnivore.Eat(_herbivore));
}

// 「名稱 → 工廠」的對應集中在一個地方，新增家族時只需要改這裡
public static class ContinentFactories
{
    public static IContinentFactory Create(string continent) => continent switch
    {
        "Africa" => new AfricaFactory(),
        "America" => new AmericaFactory(),
        _ => throw new ArgumentException($"Unknown continent: {continent}", nameof(continent)),
    };
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:AbstractFactory -->
```text
Lion eats Wildebeest
Wolf eats Bison
```
<!-- /output -->

### 重點解說

- **Client 只相依介面**：`AnimalWorld` 的程式碼裡沒有出現任何具體的動物或工廠類別，換一個工廠就換掉整組產品。
- **一致性由工廠保證**：`AfricaFactory` 的兩個方法只會回傳非洲的動物，不可能出現混搭。
- **主要建構函式（Primary Constructor）**：`AnimalWorld(IContinentFactory factory)` 把建構函式參數寫在類別名稱後面，參數可以直接拿來初始化欄位。
- **`switch` 運算式集中對應關係**：「名稱 → 工廠」只寫在 `ContinentFactories.Create` 一處；遇到不認識的名稱時丟出例外，而不是回傳 `null` 讓錯誤延後發生。

---

## 與 Dofactory 原版的差異

- 原版 .NET Optimized 使用泛型 `AnimalWorld<T> where T : IContinentFactory, new()`，產品家族在編譯期就固定了。本版改以介面傳入工廠，保留「執行期才決定用哪一個家族」的能力，這才是 Abstract Factory 最常見的用途。
- 原版以 `GetType().Name` 取得動物名稱，類別一旦改名，輸出就跟著改變。本版改用明確的 `Name` 屬性。

---

## .NET 中的對應 (In .NET)

- **`System.Data.Common.DbProviderFactory`**：`CreateConnection()`、`CreateCommand()`、`CreateParameter()` 會建立同一個資料庫廠商、彼此相容的一組 ADO.NET 物件；`SqlClientFactory`、`NpgsqlFactory` 就是 ConcreteFactory。
- **相依性注入（DI）**：把 `IContinentFactory` 註冊進容器，由設定決定實際的實作。需要依名稱同時保留多個實作時，可以使用 Keyed Services（`AddKeyedSingleton` 與 `[FromKeyedServices]`）。

---

## 何時該用、何時不該用

**適合使用**

- 產品是「成組」出現，而且必須互相搭配，例如同一個資料庫廠商的連線與命令、同一個佈景主題的按鈕與視窗。
- 需要依設定或在執行期切換整組實作。

**不適合使用**

- 只有一種產品：用 Factory Method，或直接注入該產品的介面即可。
- 家族只有一個而且看不到第二個：多一層工廠介面只是負擔。
- 產品種類經常增加：每新增一種產品（例如雜食動物），所有工廠都必須跟著修改。

---

## 練習題

1. 新增 `AustraliaFactory`，生產袋鼠（`Kangaroo`）與澳洲野犬（`Dingo`）。確認除了新增類別與 `ContinentFactories.Create` 的一行之外，不需要修改其他程式碼。
2. 在工廠介面加上第三種產品 `IOmnivore`，數一數需要修改幾個類別。這就是這個模式的代價。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Abstract Factory Pattern 如何建立平行階層的物件。物件的建立已被抽象化，因此在用戶端（Client）程式碼中不需要寫死（Hard-coded）類別名稱。

```csharp
using System;

namespace DoFactory.GangOfFour.Abstract.Structural
{
    /// <summary>
    /// MainApp startup class for Structural
    /// Abstract Factory Design Pattern.
    /// </summary>

    class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        public static void Main()
        {
            // Abstract factory #1

            AbstractFactory factory1 = new ConcreteFactory1();
            Client client1 = new Client(factory1);
            client1.Run();

            // Abstract factory #2

            AbstractFactory factory2 = new ConcreteFactory2();
            Client client2 = new Client(factory2);
            client2.Run();

            // Wait for user input

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'AbstractFactory' abstract class
    /// </summary>

    abstract class AbstractFactory
    {
        public abstract AbstractProductA CreateProductA();
        public abstract AbstractProductB CreateProductB();
    }


    /// <summary>
    /// The 'ConcreteFactory1' class
    /// </summary>

    class ConcreteFactory1 : AbstractFactory
    {
        public override AbstractProductA CreateProductA()
        {
            return new ProductA1();
        }
        public override AbstractProductB CreateProductB()
        {
            return new ProductB1();
        }
    }

    /// <summary>
    /// The 'ConcreteFactory2' class
    /// </summary>

    class ConcreteFactory2 : AbstractFactory
    {
        public override AbstractProductA CreateProductA()
        {
            return new ProductA2();
        }
        public override AbstractProductB CreateProductB()
        {
            return new ProductB2();
        }
    }

    /// <summary>
    /// The 'AbstractProductA' abstract class
    /// </summary>

    abstract class AbstractProductA
    {
    }

    /// <summary>
    /// The 'AbstractProductB' abstract class
    /// </summary>

    abstract class AbstractProductB
    {
        public abstract void Interact(AbstractProductA a);
    }


    /// <summary>
    /// The 'ProductA1' class
    /// </summary>

    class ProductA1 : AbstractProductA
    {
    }

    /// <summary>
    /// The 'ProductB1' class
    /// </summary>

    class ProductB1 : AbstractProductB
    {
        public override void Interact(AbstractProductA a)
        {
            Console.WriteLine(this.GetType().Name +
              " interacts with " + a.GetType().Name);
        }
    }

    /// <summary>
    /// The 'ProductA2' class
    /// </summary>

    class ProductA2 : AbstractProductA
    {
    }

    /// <summary>
    /// The 'ProductB2' class
    /// </summary>

    class ProductB2 : AbstractProductB
    {
        public override void Interact(AbstractProductA a)
        {
            Console.WriteLine(this.GetType().Name +
              " interacts with " + a.GetType().Name);
        }
    }

    /// <summary>
    /// The 'Client' class. Interaction environment for the products.
    /// </summary>

    class Client
    {
        private AbstractProductA _abstractProductA;
        private AbstractProductB _abstractProductB;

        // Constructor

        public Client(AbstractFactory factory)
        {
            _abstractProductB = factory.CreateProductB();
            _abstractProductA = factory.CreateProductA();
        }

        public void Run()
        {
            _abstractProductB.Interact(_abstractProductA);
        }
    }
}
```

**輸出結果 (Output)**

```text
ProductB1 interacts with ProductA1
ProductB2 interacts with ProductA2
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了在電腦遊戲中使用不同的工廠來建立不同的動物世界。雖然由各洲（Continent）工廠所建立的動物不同，但動物之間的互動方式依然保持一致。

```csharp
using System;

namespace DoFactory.GangOfFour.Abstract.RealWorld
{
    /// <summary>
    /// MainApp startup class for Real-World
    /// Abstract Factory Design Pattern.
    /// </summary>

    class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        public static void Main()
        {
            // Create and run the African animal world

            ContinentFactory africa = new AfricaFactory();
            AnimalWorld world = new AnimalWorld(africa);
            world.RunFoodChain();

            // Create and run the American animal world

            ContinentFactory america = new AmericaFactory();
            world = new AnimalWorld(america);
            world.RunFoodChain();

            // Wait for user input

            Console.ReadKey();
        }
    }


    /// <summary>
    /// The 'AbstractFactory' abstract class
    /// </summary>

    abstract class ContinentFactory
    {
        public abstract Herbivore CreateHerbivore();
        public abstract Carnivore CreateCarnivore();
    }

    /// <summary>
    /// The 'ConcreteFactory1' class
    /// </summary>

    class AfricaFactory : ContinentFactory
    {
        public override Herbivore CreateHerbivore()
        {
            return new Wildebeest();
        }
        public override Carnivore CreateCarnivore()
        {
            return new Lion();
        }
    }

    /// <summary>
    /// The 'ConcreteFactory2' class
    /// </summary>

    class AmericaFactory : ContinentFactory
    {
        public override Herbivore CreateHerbivore()
        {
            return new Bison();
        }
        public override Carnivore CreateCarnivore()
        {
            return new Wolf();
        }
    }

    /// <summary>
    /// The 'AbstractProductA' abstract class
    /// </summary>

    abstract class Herbivore
    {
    }

    /// <summary>
    /// The 'AbstractProductB' abstract class
    /// </summary>

    abstract class Carnivore
    {
        public abstract void Eat(Herbivore h);
    }

    /// <summary>
    /// The 'ProductA1' class
    /// </summary>

    class Wildebeest : Herbivore
    {
    }

    /// <summary>
    /// The 'ProductB1' class
    /// </summary>

    class Lion : Carnivore
    {
        public override void Eat(Herbivore h)
        {
            // Eat Wildebeest

            Console.WriteLine(this.GetType().Name +
              " eats " + h.GetType().Name);
        }
    }

    /// <summary>
    /// The 'ProductA2' class
    /// </summary>

    class Bison : Herbivore
    {
    }

    /// <summary>
    /// The 'ProductB2' class
    /// </summary>

    class Wolf : Carnivore
    {
        public override void Eat(Herbivore h)
        {
            // Eat Bison

            Console.WriteLine(this.GetType().Name +
              " eats " + h.GetType().Name);
        }
    }

    /// <summary>
    /// The 'Client' class 
    /// </summary>

    class AnimalWorld
    {
        private Herbivore _herbivore;
        private Carnivore _carnivore;

        // Constructor

        public AnimalWorld(ContinentFactory factory)
        {
            _carnivore = factory.CreateCarnivore();
            _herbivore = factory.CreateHerbivore();
        }

        public void RunFoodChain()
        {
            _carnivore.Eat(_herbivore);
        }
    }
}
```

**輸出結果 (Output)**

```text
Lion eats Wildebeest
Wolf eats Bison
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如泛型 `AnimalWorld<T>`、介面與運算式主體成員）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Abstract.NetOptimized;

using static System.Console;

/// <summary>
/// Abstract Factory Design Pattern.
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create and run the African animal world
        var africa = new AnimalWorld<Africa>();
        africa.RunFoodChain();

        // Create and run the American animal world
        var america = new AnimalWorld<America>();
        america.RunFoodChain();

        // Wait for user input
        ReadKey();
    }
}

/// <summary>
/// The 'AbstractFactory' interface. 
/// </summary>
public interface IContinentFactory
{
    IHerbivore CreateHerbivore();
    ICarnivore CreateCarnivore();
}

/// <summary>
/// The 'ConcreteFactory1' class.
/// </summary>
public class Africa : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Wildebeest();

    public ICarnivore CreateCarnivore() => new Lion();
}

/// <summary>
/// The 'ConcreteFactory2' class.
/// </summary>
public class America : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Bison();

    public ICarnivore CreateCarnivore() => new Wolf();
}

/// <summary>
/// The 'AbstractProductA' interface
/// </summary>
public interface IHerbivore
{
}

/// <summary>
/// The 'AbstractProductB' interface
/// </summary>
public interface ICarnivore
{
    void Eat(IHerbivore h);
}

/// <summary>
/// The 'ProductA1' class
/// </summary>
public class Wildebeest : IHerbivore
{
}

/// <summary>
/// The 'ProductB1' class
/// </summary>
public class Lion : ICarnivore
{
    // Eat Wildebeest
    public void Eat(IHerbivore h) =>
        WriteLine($"{GetType().Name} eats {h.GetType().Name}");
}

/// <summary>
/// The 'ProductA2' class
/// </summary>
public class Bison : IHerbivore
{
}

/// <summary>
/// The 'ProductB2' class
/// </summary>
public class Wolf : ICarnivore
{
    // Eat Bison
    public void Eat(IHerbivore h) =>
        WriteLine($"{GetType().Name} eats {h.GetType().Name}");
}

/// <summary>
/// The 'Client' interface
/// </summary>
public interface IAnimalWorld
{
    void RunFoodChain();
}

/// <summary>
/// The 'Client' class
/// </summary>
public class AnimalWorld<T> : IAnimalWorld where T : IContinentFactory, new()
{
    private readonly IHerbivore herbivore;
    private readonly ICarnivore carnivore;

    public AnimalWorld()
    {
        // Create new continent factory
        var factory = new T();

        // Factory creates carnivores and herbivores
        carnivore = factory.CreateCarnivore();
        herbivore = factory.CreateHerbivore();
    }

    /// <summary>
    /// Runs the foodchain: carnivores are eating herbivores.
    /// </summary>
    public void RunFoodChain()
    {
        carnivore.Eat(herbivore);
    }
}
```

**輸出結果 (Output)**

```text
Lion eats Wildebeest
Wolf eats Bison
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 返回 C# Design Patterns 總覽 (README)](README.md) | [🏠 C# Design Patterns 總覽](README.md) | [下一篇：Builder →](Builder.md) |

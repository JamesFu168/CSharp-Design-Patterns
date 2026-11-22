# C# Builder Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：中低 (Medium-Low)  
> **原文連結**：[https://www.dofactory.com/net/builder-design-pattern](https://www.dofactory.com/net/builder-design-pattern)

---

## 定義與概述 (Definition)

**Builder** Design Pattern 將複雜物件的建構過程與其表示方式（Representation）分離，讓相同的建構過程可以建立不同的表示方式。

**Builder** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Builder UML Class Diagram](images/builder.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Builder** (`VehicleBuilder`)
  - 指定一個抽象介面，用於建立 `Product` 物件的各個組件（Parts）。
- **ConcreteBuilder** (`MotorCycleBuilder`, `CarBuilder`, `ScooterBuilder`)
  - 透過實作 `Builder` 介面來建構並組裝產品的各個組件。
  - 定義並追蹤它所建立的表示方式（Representation）。
  - 提供一個用於取得產品的介面。
- **Director** (`Shop`)
  - 使用 `Builder` 介面來建構一個物件。
- **Product** (`Vehicle`)
  - 代表正在被建構的複雜物件。`ConcreteBuilder` 會建立該產品的內部表示方式，並定義其組裝過程。
  - 包含定義組成組件的類別，以及將這些組件組裝為最終結果的介面。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Builder Pattern 如何以循序漸進（Step-by-step）的方式建立複雜物件。建構過程可以建立不同的物件表示方式，並對物件的組裝提供高度的控制。

```csharp
using System;
using System.Collections.Generic;

namespace DoFactory.GangOfFour.Builder.Structural
{
    /// <summary>
    /// MainApp startup class for Structural
    /// Builder Design Pattern.
    /// </summary>

    public class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        public static void Main()
        {
            // Create director and builders

            Director director = new Director();

            Builder b1 = new ConcreteBuilder1();
            Builder b2 = new ConcreteBuilder2();

            // Construct two products

            director.Construct(b1);
            Product p1 = b1.GetResult();
            p1.Show();

            director.Construct(b2);
            Product p2 = b2.GetResult();
            p2.Show();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Director' class
    /// </summary>

    class Director
    {
        // Builder uses a complex series of steps

        public void Construct(Builder builder)
        {
            builder.BuildPartA();
            builder.BuildPartB();
        }
    }

    /// <summary>
    /// The 'Builder' abstract class
    /// </summary>

    abstract class Builder
    {
        public abstract void BuildPartA();
        public abstract void BuildPartB();
        public abstract Product GetResult();
    }

    /// <summary>
    /// The 'ConcreteBuilder1' class
    /// </summary>

    class ConcreteBuilder1 : Builder
    {
        private Product _product = new Product();

        public override void BuildPartA()
        {
            _product.Add("PartA");
        }

        public override void BuildPartB()
        {
            _product.Add("PartB");
        }

        public override Product GetResult()
        {
            return _product;
        }
    }

    /// <summary>
    /// The 'ConcreteBuilder2' class
    /// </summary>

    class ConcreteBuilder2 : Builder
    {
        private Product _product = new Product();

        public override void BuildPartA()
        {
            _product.Add("PartX");
        }

        public override void BuildPartB()
        {
            _product.Add("PartY");
        }

        public override Product GetResult()
        {
            return _product;
        }
    }

    /// <summary>
    /// The 'Product' class
    /// </summary>

    class Product
    {
        private List<string> _parts = new List<string>();

        public void Add(string part)
        {
            _parts.Add(part);
        }

        public void Show()
        {
            Console.WriteLine("\nProduct Parts -------");
            foreach (string part in _parts)
                Console.WriteLine(part);
        }
    }
}
```

### 輸出結果 (Output)

```text
Product Parts -------

  PartA

  PartB

  

  Product Parts -------

  PartX

  PartY
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Builder Pattern 如何以循序漸進的方式組裝不同的交通工具（Vehicles）。`Shop`（車廠）使用 `VehicleBuilder` 透過一系列連續的步驟來建構各種 `Vehicle`。

```csharp
using System;
using System.Collections.Generic;

namespace DoFactory.GangOfFour.Builder.RealWorld
{
    /// <summary>
    /// MainApp startup class for Real-World 
    /// Builder Design Pattern.
    /// </summary>

    public class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        public static void Main()
        {
            VehicleBuilder builder;

            // Create shop with vehicle builders

            Shop shop = new Shop();

            // Construct and display vehicles

            builder = new ScooterBuilder();
            shop.Construct(builder);
            builder.Vehicle.Show();

            builder = new CarBuilder();
            shop.Construct(builder);
            builder.Vehicle.Show();

            builder = new MotorCycleBuilder();
            shop.Construct(builder);
            builder.Vehicle.Show();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Director' class
    /// </summary>

    class Shop
    {
        // Builder uses a complex series of steps

        public void Construct(VehicleBuilder vehicleBuilder)
        {
            vehicleBuilder.BuildFrame();
            vehicleBuilder.BuildEngine();
            vehicleBuilder.BuildWheels();
            vehicleBuilder.BuildDoors();
        }
    }

    /// <summary>
    /// The 'Builder' abstract class
    /// </summary>

    abstract class VehicleBuilder
    {
        protected Vehicle vehicle;

        // Gets vehicle instance

        public Vehicle Vehicle
        {
            get { return vehicle; }
        }

        // Abstract build methods

        public abstract void BuildFrame();
        public abstract void BuildEngine();
        public abstract void BuildWheels();
        public abstract void BuildDoors();
    }

    /// <summary>
    /// The 'ConcreteBuilder1' class
    /// </summary>

    class MotorCycleBuilder : VehicleBuilder
    {
        public MotorCycleBuilder()
        {
            vehicle = new Vehicle("MotorCycle");
        }

        public override void BuildFrame()
        {
            vehicle["frame"] = "MotorCycle Frame";
        }

        public override void BuildEngine()
        {
            vehicle["engine"] = "500 cc";
        }

        public override void BuildWheels()
        {
            vehicle["wheels"] = "2";
        }

        public override void BuildDoors()
        {
            vehicle["doors"] = "0";
        }
    }

    /// <summary>
    /// The 'ConcreteBuilder2' class
    /// </summary>

    class CarBuilder : VehicleBuilder
    {
        public CarBuilder()
        {
            vehicle = new Vehicle("Car");
        }

        public override void BuildFrame()
        {
            vehicle["frame"] = "Car Frame";
        }

        public override void BuildEngine()
        {
            vehicle["engine"] = "2500 cc";
        }

        public override void BuildWheels()
        {
            vehicle["wheels"] = "4";
        }

        public override void BuildDoors()
        {
            vehicle["doors"] = "4";
        }
    }

    /// <summary>
    /// The 'ConcreteBuilder3' class
    /// </summary>

    class ScooterBuilder : VehicleBuilder
    {
        public ScooterBuilder()
        {
            vehicle = new Vehicle("Scooter");
        }

        public override void BuildFrame()
        {
            vehicle["frame"] = "Scooter Frame";
        }

        public override void BuildEngine()
        {
            vehicle["engine"] = "50 cc";
        }

        public override void BuildWheels()
        {
            vehicle["wheels"] = "2";
        }

        public override void BuildDoors()
        {
            vehicle["doors"] = "0";
        }
    }

    /// <summary>
    /// The 'Product' class
    /// </summary>

    class Vehicle
    {
        private string _vehicleType;
        private Dictionary<string, string> _parts =
          new Dictionary<string, string>();

        // Constructor

        public Vehicle(string vehicleType)
        {
            this._vehicleType = vehicleType;
        }

        // Indexer

        public string this[string key]
        {
            get { return _parts[key]; }
            set { _parts[key] = value; }
        }

        public void Show()
        {
            Console.WriteLine("\n---------------------------");
            Console.WriteLine("Vehicle Type: {0}", _vehicleType);
            Console.WriteLine(" Frame : {0}", _parts["frame"]);
            Console.WriteLine(" Engine : {0}", _parts["engine"]);
            Console.WriteLine(" #Wheels: {0}", _parts["wheels"]);
            Console.WriteLine(" #Doors : {0}", _parts["doors"]);
        }
    }
}
```

### 輸出結果 (Output)

```text
---------------------------

  Vehicle Type: Scooter

   Frame   : Scooter Frame

   Engine : none

   #Wheels: 2

   #Doors : 0

  

  ---------------------------

  Vehicle Type: Car

   Frame   : Car Frame

   Engine : 2500 cc

   #Wheels: 4

   #Doors : 4

  

  ---------------------------

  Vehicle Type: MotorCycle

   Frame   : MotorCycle Frame

   Engine : 500 cc

   #Wheels: 2

   #Doors : 0
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors、列舉型別 `PartType` 與索引子）。這是一個優雅的 C# Builder 解決方案。

```csharp
namespace Builder.NetOptimized;

using static System.Console;

/// <summary>
/// Builder Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create shop
        var shop = new Shop();

        // Construct and display vehicles
        shop.Construct(new ScooterBuilder());
        shop.ShowVehicle();

        shop.Construct(new CarBuilder());
        shop.ShowVehicle();

        shop.Construct(new MotorCycleBuilder());
        shop.ShowVehicle();

        // Wait for user
        ReadKey();
    }
}
/// <summary>
/// The 'Director' class
/// </summary>
public class Shop
{
    private VehicleBuilder? builder;

    // Builder uses a complex series of steps
    public void Construct(VehicleBuilder vehicleBuilder)
    {
        builder = vehicleBuilder;

        builder.BuildFrame();
        builder.BuildEngine();
        builder.BuildWheels();
        builder.BuildDoors();
    }

    public void ShowVehicle()
    {
        builder?.Vehicle.Show();
    }
}

/// <summary>
/// The 'Builder' abstract class
/// </summary>
public abstract class VehicleBuilder(VehicleType vehicleType)
{
    public Vehicle Vehicle { get; private set; } = new Vehicle(vehicleType);

    public abstract void BuildFrame();
    public abstract void BuildEngine();
    public abstract void BuildWheels();
    public abstract void BuildDoors();
}

/// <summary>
/// The 'ConcreteBuilder1' class
/// </summary>
public class MotorCycleBuilder : VehicleBuilder
{
    // Invoke base class constructor
    public MotorCycleBuilder() : base(VehicleType.MotorCycle)
    {
    }

    public override void BuildFrame() => Vehicle[PartType.Frame] = "MotorCycle Frame";
    public override void BuildEngine() => Vehicle[PartType.Engine] = "500 cc";
    public override void BuildWheels() => Vehicle[PartType.Wheel] = "2";
    public override void BuildDoors() => Vehicle[PartType.Door] = "0";
}

/// <summary>
/// The 'ConcreteBuilder2' class
/// </summary>
public class CarBuilder : VehicleBuilder
{
    // Invoke base class constructor
    public CarBuilder() : base(VehicleType.Car)
    {
    }

    public override void BuildFrame() => Vehicle[PartType.Frame] = "Car Frame";
    public override void BuildEngine() => Vehicle[PartType.Engine] = "2500 cc";
    public override void BuildWheels() => Vehicle[PartType.Wheel] = "4";
    public override void BuildDoors() => Vehicle[PartType.Door] = "4";
}

/// <summary>
/// The 'ConcreteBuilder3' class
/// </summary>
public class ScooterBuilder : VehicleBuilder
{
    // Invoke base class constructor
    public ScooterBuilder() : base(VehicleType.Scooter)
    {
    }

    public override void BuildFrame() => Vehicle[PartType.Frame] = "Scooter Frame";
    public override void BuildEngine() => Vehicle[PartType.Engine] = "50 cc";
    public override void BuildWheels() => Vehicle[PartType.Wheel] = "2";
    public override void BuildDoors() => Vehicle[PartType.Door] = "0";
}

/// <summary>
/// The 'Product' class
/// </summary>
public class Vehicle(VehicleType vehicleType)
{
    private readonly Dictionary<PartType, string> parts = [];
    private readonly VehicleType vehicleType = vehicleType;

    public string this[PartType key]
    {
        get => parts[key];
        set => parts[key] = value; 
    }

    public void Show()
    {
        WriteLine("\n---------------------------");
        WriteLine($"Vehicle Type: {vehicleType}");
        WriteLine($" Frame  : {this[PartType.Frame]}");
        WriteLine($" Engine : {this[PartType.Engine]}");
        WriteLine($" #Wheels: {this[PartType.Wheel]}");
        WriteLine($" #Doors : {this[PartType.Door]}");
    }
}

/// <summary>
/// Part type enumeration
/// </summary>
public enum PartType
{
    Frame,
    Engine,
    Wheel,
    Door
}

/// <summary>
/// Vehicle type enumeration
/// </summary>
public enum VehicleType
{
    Car,
    Scooter,
    MotorCycle
}
```

### 輸出結果 (Output)

```text
---------------------------

    Vehicle Type: Scooter

     Frame   : Scooter Frame

     Engine : none

     #Wheels: 2

     #Doors : 0

    

    ---------------------------

    Vehicle Type: Car

     Frame   : Car Frame

     Engine : 2500 cc

     #Wheels: 4

     #Doors : 4

    

    ---------------------------

    Vehicle Type: MotorCycle

     Frame   : MotorCycle Frame

     Engine : 500 cc

     #Wheels: 2

     #Doors : 0
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **可變狀態（Mutable State）過多**：Dofactory 原版在 `Vehicle` 內部使用 `Dictionary<PartType, string>`，且要求 `Director`（`Shop`）依序呼叫 `BuildFrame()`、`BuildEngine()`。若漏掉某個步驟，執行期才會拋出 `KeyNotFoundException`。
- **缺乏 Fluent API 與不可變性（Immutability）**：現代 .NET（如 `HostApplicationBuilder`、`WebApplication.CreateBuilder`）普遍採用 **Fluent Builder** 搭配不可變的 `record` 產品物件，並使用 `required` 屬性或集合運算式。

### C# 15 / .NET 11 優化重點

- **Fluent Builder + Immutable `record` 產品**：建構過程採用 Method Chaining（方法鏈），最終 `Build()` 產出不可變的 `Vehicle` `record`。
- **C# 15 Collection Expression Arguments (`with(capacity: ...)`)**：在初始化零件字典或清單時預先指定容量，避免動態擴容開銷。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Builder.ModernNet11;

using System.Collections.Frozen;
using static System.Console;

public static class Program
{
    public static void Main()
    {
        var car = new VehicleBuilder(VehicleType.Car)
            .WithFrame("Car Frame")
            .WithEngine("2500 cc")
            .WithWheels(4)
            .WithDoors(4)
            .Build();

        car.Show();
    }
}

public enum VehicleType { Car, Scooter, MotorCycle }
public enum PartType { Frame, Engine, Wheel, Door }

/// <summary>
/// 不可變的 Product record，使用 .NET FrozenDictionary 提升讀取效能
/// </summary>
public sealed record Vehicle(VehicleType Type, FrozenDictionary<PartType, string> Parts)
{
    public void Show()
    {
        WriteLine($"\n---------------------------");
        WriteLine($"Vehicle Type: {Type}");
        foreach (var (part, spec) in Parts)
        {
            WriteLine($" {part,-6} : {spec}");
        }
    }
}

/// <summary>
/// 現代化 Fluent Builder
/// </summary>
public sealed class VehicleBuilder(VehicleType type)
{
    private readonly Dictionary<PartType, string> _parts = new(capacity: 4);

    public VehicleBuilder WithFrame(string frame) { _parts[PartType.Frame] = frame; return this; }
    public VehicleBuilder WithEngine(string engine) { _parts[PartType.Engine] = engine; return this; }
    public VehicleBuilder WithWheels(int count) { _parts[PartType.Wheel] = $"{count}"; return this; }
    public VehicleBuilder WithDoors(int count) { _parts[PartType.Door] = $"{count}"; return this; }

    public Vehicle Build() => new(type, _parts.ToFrozenDictionary());
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Abstract Factory](AbstractFactory.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Factory Method →](FactoryMethod.md) |

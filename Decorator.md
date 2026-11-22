# C# Decorator Design Pattern

> **分類**：Structural Patterns（結構型模式）  
> **使用頻率 (Frequency of use)**：中 (Medium)  
> **原文連結**：[https://www.dofactory.com/net/decorator-design-pattern](https://www.dofactory.com/net/decorator-design-pattern)

---

## 定義與概述 (Definition)

**Decorator** Design Pattern 可以動態地將額外的職責（Responsibilities）附加到物件上。對於擴充功能而言，此 Pattern 提供了比繼承子類別（Subclassing）更具彈性的替代方案。

**Decorator** Design Pattern 的 C# 程式碼範例除了提供 Dofactory 原版的 3 種形式外，亦額外補充針對 **.NET 11 / C# 15** 的現代化實務優化版本：
- [結構化程式碼 (Structural code)](#structural-code-in-c-結構化程式碼)
- [真實世界程式碼 (Real-world code)](#real-world-code-in-c-真實世界程式碼)
- [.NET 最佳化程式碼 (.NET optimized code)](#net-optimized-code-in-c-net-最佳化程式碼)
- [🚀 .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)](#net-11--c-15-現代化實務優化-modern-net-11--c-15)

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Decorator UML Class Diagram](images/decorator.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

- **Component** (`LibraryItem`)
  - 為可以動態加上職責的物件定義介面。
- **ConcreteComponent** (`Book`, `Video`)
  - 定義一個可以附加額外職責的具體物件。
- **Decorator** (`Decorator`)
  - 維護一個指向 `Component` 物件的參考，並定義一個符合 `Component` 介面的介面。
- **ConcreteDecorator** (`Borrowable`)
  - 向組件（Component）加入額外的職責。

---

## Structural code in C# (結構化程式碼)

這段結構化（Structural）程式碼示範了 Decorator Pattern 如何動態地為既有物件加上額外的功能。

```csharp
using System;

namespace Decorator.Structural
{
    /// <summary>
    /// Decorator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create ConcreteComponent and two Decorators

            ConcreteComponent c = new ConcreteComponent();
            ConcreteDecoratorA d1 = new ConcreteDecoratorA();
            ConcreteDecoratorB d2 = new ConcreteDecoratorB();

            // Link decorators

            d1.SetComponent(c);
            d2.SetComponent(d1);

            d2.Operation();

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Component' abstract class
    /// </summary>

    public abstract class Component
    {
        public abstract void Operation();
    }

    /// <summary>
    /// The 'ConcreteComponent' class
    /// </summary>

    public class ConcreteComponent : Component
    {
        public override void Operation()
        {
            Console.WriteLine("ConcreteComponent.Operation()");
        }
    }

    /// <summary>
    /// The 'Decorator' abstract class
    /// </summary>

    public abstract class Decorator : Component
    {
        protected Component component;

        public void SetComponent(Component component)
        {
            this.component = component;
        }

        public override void Operation()
        {
            if (component != null)
            {
                component.Operation();
            }
        }
    }

    /// <summary>
    /// The 'ConcreteDecoratorA' class
    /// </summary>

    public class ConcreteDecoratorA : Decorator
    {
        public override void Operation()
        {
            base.Operation();
            Console.WriteLine("ConcreteDecoratorA.Operation()");
        }
    }

    /// <summary>
    /// The 'ConcreteDecoratorB' class
    /// </summary>

    public class ConcreteDecoratorB : Decorator
    {
        public override void Operation()
        {
            base.Operation();
            AddedBehavior();
            Console.WriteLine("ConcreteDecoratorB.Operation()");
        }

        void AddedBehavior()
        {
        }
    }
}
```

### 輸出結果 (Output)

```text
ConcreteComponent.Operation()

  ConcreteDecoratorA.Operation()

  ConcreteDecoratorB.Operation()
```

---

## Real-world code in C# (真實世界程式碼)

這段真實世界（Real-world）程式碼示範了 Decorator Pattern 如何將「可借閱」（`Borrowable`）的功能動態加入到既有的圖書館館藏項目（書籍 `Book` 與影片 `Video`）中。

```csharp
using System;
using System.Collections.Generic;

namespace Decorator.RealWorld
{
    /// <summary>
    /// Decorator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create book

            Book book = new Book("Worley", "Inside ASP.NET", 10);
            book.Display();

            // Create video

            Video video = new Video("Spielberg", "Jaws", 23, 92);
            video.Display();

            // Make video borrowable, then borrow and display

            Console.WriteLine("\nMaking video borrowable:");

            Borrowable borrowvideo = new Borrowable(video);
            borrowvideo.BorrowItem("Customer #1");
            borrowvideo.BorrowItem("Customer #2");

            borrowvideo.Display();

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// The 'Component' abstract class
    /// </summary>

    public abstract class LibraryItem
    {
        private int numCopies;

        public int NumCopies
        {
            get { return numCopies; }
            set { numCopies = value; }
        }

        public abstract void Display();
    }

    /// <summary>
    /// The 'ConcreteComponent' class
    /// </summary>

    public class Book : LibraryItem
    {
        private string author;
        private string title;

        // Constructor

        public Book(string author, string title, int numCopies)
        {
            this.author = author;
            this.title = title;
            this.NumCopies = numCopies;
        }

        public override void Display()
        {
            Console.WriteLine("\nBook ------ ");
            Console.WriteLine(" Author: {0}", author);
            Console.WriteLine(" Title: {0}", title);
            Console.WriteLine(" # Copies: {0}", NumCopies);
        }
    }

    /// <summary>
    /// The 'ConcreteComponent' class
    /// </summary>

    public class Video : LibraryItem
    {
        private string director;
        private string title;
        private int playTime;

        // Constructor

        public Video(string director, string title, int numCopies, int playTime)
        {
            this.director = director;
            this.title = title;
            this.NumCopies = numCopies;
            this.playTime = playTime;
        }

        public override void Display()
        {
            Console.WriteLine("\nVideo ----- ");
            Console.WriteLine(" Director: {0}", director);
            Console.WriteLine(" Title: {0}", title);
            Console.WriteLine(" # Copies: {0}", NumCopies);
            Console.WriteLine(" Playtime: {0}\n", playTime);
        }
    }

    /// <summary>
    /// The 'Decorator' abstract class
    /// </summary>

    public abstract class Decorator : LibraryItem
    {
        protected LibraryItem libraryItem;

        // Constructor

        public Decorator(LibraryItem libraryItem)
        {
            this.libraryItem = libraryItem;
        }

        public override void Display()
        {
            libraryItem.Display();
        }
    }

    /// <summary>
    /// The 'ConcreteDecorator' class
    /// </summary>

    public class Borrowable : Decorator
    {
        protected readonly List<string> borrowers = new List<string>();

        // Constructor

        public Borrowable(LibraryItem libraryItem)
            : base(libraryItem)
        {
        }

        public void BorrowItem(string name)
        {
            borrowers.Add(name);
            libraryItem.NumCopies--;
        }

        public void ReturnItem(string name)
        {
            borrowers.Remove(name);
            libraryItem.NumCopies++;
        }

        public override void Display()
        {
            base.Display();

            foreach (string borrower in borrowers)
            {
                Console.WriteLine(" borrower: " + borrower);
            }
        }
    }
}
```

### 輸出結果 (Output)

```text
Book ------

  Author: Worley

  Title: Inside ASP.NET

  # Copies: 10

  

  Video -----

  Director: Spielberg

  Title: Jaws

  # Copies: 23

  Playtime: 92

  

  

  Making video borrowable:

  

  Video -----

  Director: Spielberg

  Title: Jaws

  # Copies: 21

  Playtime: 92

  

  borrower: Customer #1

  borrower: Customer #2
```

---

## .NET Optimized code in C# (.NET 最佳化程式碼)

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors 與泛型 `Decorator<T>` / `Borrowable<T>`）。這是一個優雅的 C# Decorator 解決方案。

```csharp
namespace Decorator.NetOptimized;

using static System.Console;

/// <summary>
/// Decorator Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create book
        var book = new Book("Worley", "Inside ASP.NET", 10);
        book.Display();

        // Create video
        var video = new Video("Spielberg", "Jaws", 23, 92);
        video.Display();

        // Make video borrowable, then borrow and display
        WriteLine("\nMaking video borrowable:");

        var borrow = new Borrowable<Video>(video);
        borrow.BorrowItem("Customer #1");
        borrow.BorrowItem("Customer #2");

        borrow.Display();

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Component' abstract class
/// </summary>
public abstract class LibraryItem<T>
{
    // Each T has its own NumCopies
    public static int NumCopies { get; set; }

    public abstract void Display();
}

/// <summary>
/// The 'ConcreteComponent' class
/// </summary>
public class Book : LibraryItem<Book>
{
    private readonly string author;
    private readonly string title;

    // Constructor
    public Book(string author, string title, int numCopies)
    {
        this.author = author;
        this.title = title;
        NumCopies = numCopies;
    }

    public override void Display()
    {
        WriteLine("\nBook ------ ");
        WriteLine($" Author: {author}");
        WriteLine($" Title: {title}");
        WriteLine($" # Copies: {NumCopies}");
    }
}

/// <summary>
/// The 'ConcreteComponent' class
/// </summary>
public class Video : LibraryItem<Video>
{
    private readonly string director;
    private readonly string title;
    private readonly int playTime;

    // Constructor
    public Video(string director, string title,
        int numCopies, int playTime)
    {
        this.director = director;
        this.title = title;
        NumCopies = numCopies;
        this.playTime = playTime;
    }

    public override void Display()
    {
        WriteLine("\nVideo ----- ");
        WriteLine($" Director: {director}");
        WriteLine($" Title: {title}");
        WriteLine($" # Copies: {NumCopies}");
        WriteLine($" Playtime: {playTime}\n");
    }
}

/// <summary>
/// The 'Decorator' abstract class
/// </summary>
public abstract class Decorator<T>(LibraryItem<T> libraryItem) : LibraryItem<T>
{
    public override void Display() => libraryItem.Display();

}

/// <summary>
/// The 'ConcreteDecorator' class
/// </summary>
public class Borrowable<T> : Decorator<T>
{
    private readonly List<string> borrowers = [];

    // Constructor
    public Borrowable(LibraryItem<T> libraryItem)
        : base(libraryItem)
    {
    }

    public void BorrowItem(string name)
    {
        borrowers.Add(name);
        NumCopies--;
    }

    public void ReturnItem(string name)
    {
        borrowers.Remove(name);
        NumCopies++;
    }

    public override void Display()
    {
        base.Display();
        borrowers.ForEach(b => WriteLine(" borrower: " + b));
    }
}
```

### 輸出結果 (Output)

```text
Book ------

    Author: Worley

    Title: Inside ASP.NET

    # Copies: 10

    

    Video -----

    Director: Spielberg

    Title: Jaws

    # Copies: 23

    Playtime: 92

    

    

    Making video borrowable:

    

    Video -----

    Director: Spielberg

    Title: Jaws

    # Copies: 21

    Playtime: 92

    

    borrower: Customer #1

    borrower: Customer #2
```

---

## .NET 11 / C# 15 現代化實務優化 (Modern .NET 11 & C# 15)

### 原版範例的限制與盲點

- **奇妙的遞迴泛型與靜態狀態錯誤（Critical Bug）**：仔細看 Dofactory 的 `.NET Optimized` 程式碼，它在抽象類別 `LibraryItem<T>` 中宣告了 `public static int NumCopies { get; set; }`！這意味著**所有同型別的書籍或影片會共享同一個 `NumCopies` 靜態變數**，只要借出一本書，全圖書館所有書的庫存都會一起減少！這是原版程式碼中非常嚴重的設計瑕疵。
- **介面組合優於類別繼承**：現代 .NET（如 `Stream`、`IDistributedCache`、ASP.NET Core Middleware）實作 Decorator 時，應該共用一個簡潔的 `ILibraryItem` 介面，並且每個實例各自持有自己的 `NumCopies` 狀態。

### C# 15 / .NET 11 優化重點

- **修正原版的 `static NumCopies` 狀態污染 Bug**：將 `NumCopies` 改為實例屬性（Instance Property）。
- **C# 15 Collection Expression Arguments (`with(capacity: ...)`)**：在 `Borrowable` 裝飾者中高效管理借閱者清單。

### 現代化 C# 15 / .NET 11 程式碼範例

```csharp
namespace Decorator.ModernNet11;

using static System.Console;

public static class Program
{
    public static void Main()
    {
        var book = new Book("Worley", "Inside ASP.NET", NumCopies: 10);
        book.Display();

        var video = new Video("Spielberg", "Jaws", NumCopies: 23, PlayTime: 92);
        video.Display();

        WriteLine("\nMaking video borrowable:");
        var borrowableVideo = new Borrowable(video);
        borrowableVideo.BorrowItem("Customer #1");
        borrowableVideo.BorrowItem("Customer #2");
        borrowableVideo.Display();
    }
}

public interface ILibraryItem
{
    int NumCopies { get; set; }
    void Display();
}

public sealed class Book(string author, string title, int NumCopies) : ILibraryItem
{
    public int NumCopies { get; set; } = NumCopies;

    public void Display()
    {
        WriteLine("\nBook ------ ");
        WriteLine($" Author: {author}");
        WriteLine($" Title: {title}");
        WriteLine($" # Copies: {NumCopies}");
    }
}

public sealed class Video(string director, string title, int NumCopies, int PlayTime) : ILibraryItem
{
    public int NumCopies { get; set; } = NumCopies;

    public void Display()
    {
        WriteLine("\nVideo ----- ");
        WriteLine($" Director: {director}");
        WriteLine($" Title: {title}");
        WriteLine($" # Copies: {NumCopies}");
        WriteLine($" Playtime: {PlayTime}\n");
    }
}

public abstract class LibraryDecorator(ILibraryItem inner) : ILibraryItem
{
    public int NumCopies
    {
        get => inner.NumCopies;
        set => inner.NumCopies = value;
    }

    public virtual void Display() => inner.Display();
}

public sealed class Borrowable(ILibraryItem inner) : LibraryDecorator(inner)
{
    private readonly List<string> _borrowers = [with(capacity: 4)];

    public void BorrowItem(string name)
    {
        _borrowers.Add(name);
        NumCopies--;
    }

    public void ReturnItem(string name)
    {
        _borrowers.Remove(name);
        NumCopies++;
    }

    public override void Display()
    {
        base.Display();
        foreach (var borrower in _borrowers)
        {
            WriteLine($" borrower: {borrower}");
        }
    }
}
```

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Composite](Composite.md) | [🏠 C# Design Patterns 總覽](Index.md) | [下一篇：Facade →](Facade.md) |

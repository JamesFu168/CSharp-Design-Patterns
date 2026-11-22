# C# Iterator Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/iterator-design-pattern](https://www.dofactory.com/net/iterator-design-pattern)

---

## 定義與概述 (Definition)

**Iterator** Design Pattern 提供一種方法，可以循序存取聚合（Aggregate）物件中的各個元素，而無須暴露其底層的表示方式（Underlying Representation）。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

集合內部用什麼方式存放資料（陣列、連結串列、樹狀結構），不應該讓使用它的程式碼知道。如果呼叫端直接存取內部結構：

```csharp
// 示意：呼叫端依賴「內部是一個可以用索引存取的清單」
for (int i = 0; i < collection.Items.Count; i += 2)
{
    Console.WriteLine(collection.Items[i]);
}
```

- 集合的內部結構一旦改變，所有這樣寫的地方都會壞掉。
- 每一種走訪方式（跳著取、反向取）都要呼叫端自己重寫一次。

Iterator 提供一個統一的方式「一次給我下一個元素」，把走訪的邏輯留在集合這一邊。C# 把這個模式直接做進語言：`IEnumerable<T>`、`foreach` 與 `yield return`。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Iterator UML Class Diagram](images/iterator.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Iterator** (`AbstractIterator`)
  - 定義一個用於存取及走訪（Traversing）元素的介面。
- **ConcreteIterator** (`Iterator`)
  - 實作 `Iterator` 介面。
  - 在走訪聚合物件的過程中，追蹤目前所在的位置。
- **Aggregate** (`AbstractCollection`)
  - 定義一個用於建立 `Iterator` 物件的介面。
- **ConcreteAggregate** (`Collection`)
  - 實作建立 `Iterator` 的介面，以回傳適當之 `ConcreteIterator` 的實例。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Iterator](samples/Iterator)  
> **執行方式**：`dotnet run --project samples/Iterator`

**角色對照**：Aggregate = `IEnumerable<T>`；ConcreteAggregate = `ItemCollection<T>`；Iterator = `IEnumerator<T>`；ConcreteIterator = 編譯器依 `yield return` 產生的類別；Client = 最上方的程式碼。

<!-- sample:Iterator/Program.cs -->
```csharp
// Iterator：讓呼叫端可以逐一取得集合的元素，卻不必知道集合內部怎麼存放資料。

ItemCollection<string> items =
[
    "Item 0", "Item 1", "Item 2", "Item 3", "Item 4",
    "Item 5", "Item 6", "Item 7", "Item 8",
];

// 實作 IEnumerable<T> 之後，foreach、LINQ、string.Join 都能直接使用
Console.WriteLine($"All      : {string.Join(", ", items)}");
Console.WriteLine($"Step by 2: {string.Join(", ", items.Step(2))}");
Console.WriteLine($"Reversed : {string.Join(", ", items.Reversed())}");

// yield return 是「延遲執行」：要一個才產生一個。這裡只會實際取出 2 個元素
Console.WriteLine($"First two of step 3: {string.Join(", ", items.Step(3).Take(2))}");

// 資料需要等待（網路、資料庫分頁）才能取得時，改用 IAsyncEnumerable<T> 與 await foreach
await foreach (string[] page in items.ReadPagesAsync(pageSize: 4))
{
    Console.WriteLine($"Page     : {string.Join(", ", page)}");
}

// ---- Aggregate：實作 IEnumerable<T>，C# 的 foreach 就認得它 ----
public sealed class ItemCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = [];

    // 有 Add 方法，才能用集合運算式 [...] 來初始化
    public void Add(T item) => _items.Add(item);

    // ---- Iterator：編譯器會把 yield return 轉成記住「走到哪裡」的列舉器類別 ----
    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in _items)
        {
            yield return item;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    // 同一個集合可以提供好幾種走訪方式
    public IEnumerable<T> Step(int step)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(step, 1);

        for (int i = 0; i < _items.Count; i += step)
        {
            yield return _items[i];
        }
    }

    public IEnumerable<T> Reversed()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            yield return _items[i];
        }
    }

    public async IAsyncEnumerable<T[]> ReadPagesAsync(int pageSize)
    {
        foreach (T[] page in _items.Chunk(pageSize))
        {
            await Task.Delay(10);   // 模擬一次網路或資料庫往返
            yield return page;
        }
    }
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Iterator -->
```text
All      : Item 0, Item 1, Item 2, Item 3, Item 4, Item 5, Item 6, Item 7, Item 8
Step by 2: Item 0, Item 2, Item 4, Item 6, Item 8
Reversed : Item 8, Item 7, Item 6, Item 5, Item 4, Item 3, Item 2, Item 1, Item 0
First two of step 3: Item 0, Item 3
Page     : Item 0, Item 1, Item 2, Item 3
Page     : Item 4, Item 5, Item 6, Item 7
Page     : Item 8
```
<!-- /output -->

### 重點解說

- **實作 `IEnumerable<T>` 就能融入整個生態**：`foreach`、LINQ、`string.Join` 都能直接使用這個集合。
- **`yield return`**：編譯器會把含有 `yield return` 的方法轉成一個列舉器類別，自動記住「目前走到哪裡」。不需要手寫 `MoveNext` 與 `Current`。
- **延遲執行**：`items.Step(3).Take(2)` 只會實際取出兩個元素，不會先走完整個集合。
- **`IAsyncEnumerable<T>` 與 `await foreach`**：下一筆資料需要等待（網路、資料庫）才能取得時使用。
- **集合運算式需要 `Add`**：類別實作 `IEnumerable<T>` 並提供 `Add` 方法，就能用 `[...]` 語法初始化。
- **非泛型的 `GetEnumerator`**：`IEnumerable<T>` 繼承了舊的非泛型介面，所以必須多實作一個，直接轉呼叫泛型版本即可。

---

## 與 Dofactory 原版的差異

- 原版 Structural 與 Real-world 手寫了 `Iterator` 類別（`First()`、`Next()`、`IsDone`、`CurrentItem`）。這是 GoF 書中的寫法，在 C# 中早已由 `IEnumerable<T>` 與 `yield return` 取代。
- 原版 .NET Optimized 已經使用 `yield return`。本版另外補充延遲執行與 `IAsyncEnumerable<T>`。

---

## .NET 中的對應 (In .NET)

- **所有內建集合**：`List<T>`、`Dictionary<TKey, TValue>`、陣列都實作 `IEnumerable<T>`。
- **LINQ**：`Where`、`Select`、`Take` 都是以迭代器實作，並且可以串接。
- **`File.ReadLines(path)`**：一次讀取一行（延遲執行）；對照 `File.ReadAllLines` 會一次把整個檔案讀進記憶體。
- **`IAsyncEnumerable<T>`**：EF Core 的 `AsAsyncEnumerable()`、gRPC 串流、分頁式 API。

---

## 何時該用、何時不該用

**適合使用**

- 自己寫的集合或資料結構，要讓外界走訪。
- 資料量很大或是無限序列，需要「要一個才算一個」。
- 同一份資料要提供多種走訪方式。

**不適合使用**

- 內建集合已經夠用：直接回傳 `IReadOnlyList<T>` 或 `IEnumerable<T>`，不需要自己寫集合類別。

在 C# 中幾乎不需要手寫 `IEnumerator<T>`，使用 `yield return` 即可。

---

## 練習題

1. 寫一個 `IEnumerable<long> Fibonacci()`，它是一個無限序列。用 `Take(10)` 印出前 10 個數字。
2. 呼叫 `items.Step(0)` 但不要走訪它的結果，會丟出例外嗎？為什麼？（提示：延遲執行也包含參數檢查。）
3. 替 `ReadPagesAsync` 加上 `CancellationToken` 參數（提示：`[EnumeratorCancellation]` 屬性）。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Iterator Pattern 如何提供一種方法來走訪（疊代，Iterate）項目集合，而無須暴露該集合的底層結構細節。

```csharp
using System;
using System.Collections.Generic;

namespace Iterator.Structural
{
    /// <summary>
    /// Iterator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            ConcreteAggregate a = new ConcreteAggregate();
            a[0] = "Item A";
            a[1] = "Item B";
            a[2] = "Item C";
            a[3] = "Item D";

            // Create Iterator and provide aggregate

            Iterator i = a.CreateIterator();

            Console.WriteLine("Iterating over collection:");

            object item = i.First();

            while (item != null)
            {
                Console.WriteLine(item);
                item = i.Next();
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Aggregate' abstract class
    /// </summary>

    public abstract class Aggregate
    {
        public abstract Iterator CreateIterator();
    }

    /// <summary>
    /// The 'ConcreteAggregate' class
    /// </summary>

    public class ConcreteAggregate : Aggregate
    {
        List<object> items = new List<object>();

        public override Iterator CreateIterator()
        {
            return new ConcreteIterator(this);
        }

        // Get item count

        public int Count
        {
            get { return items.Count; }
        }

        // Indexer

        public object this[int index]
        {
            get { return items[index]; }
            set { items.Insert(index, value); }
        }
    }

    /// <summary>
    /// The 'Iterator' abstract class
    /// </summary>

    public abstract class Iterator
    {
        public abstract object First();
        public abstract object Next();
        public abstract bool IsDone();
        public abstract object CurrentItem();
    }

    /// <summary>
    /// The 'ConcreteIterator' class
    /// </summary>

    public class ConcreteIterator : Iterator
    {
        ConcreteAggregate aggregate;
        int current = 0;

        // Constructor

        public ConcreteIterator(ConcreteAggregate aggregate)
        {
            this.aggregate = aggregate;
        }

        // Gets first iteration item

        public override object First()
        {
            return aggregate[0];
        }

        // Gets next iteration item

        public override object Next()
        {
            object ret = null;
            if (current < aggregate.Count - 1)
            {
                ret = aggregate[++current];
            }

            return ret;
        }

        // Gets current iteration item

        public override object CurrentItem()
        {
            return aggregate[current];
        }

        // Gets whether iterations are complete

        public override bool IsDone()
        {
            return current >= aggregate.Count;
        }
    }
}
```

**輸出結果 (Output)**

```text
Iterating over collection:
Item A
Item B
Item C
Item D
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了如何使用 Iterator Pattern 來走訪項目集合，並在每次疊代（Iteration）時跳過特定數量的項目。

```csharp
using System;
using System.Collections.Generic;

namespace Iterator.RealWorld
{
    /// <summary>
    /// Iterator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Build a collection

            Collection collection = new Collection();
            collection[0] = new Item("Item 0");
            collection[1] = new Item("Item 1");
            collection[2] = new Item("Item 2");
            collection[3] = new Item("Item 3");
            collection[4] = new Item("Item 4");
            collection[5] = new Item("Item 5");
            collection[6] = new Item("Item 6");
            collection[7] = new Item("Item 7");
            collection[8] = new Item("Item 8");

            // Create iterator

            Iterator iterator = collection.CreateIterator();

            // Skip every other item

            iterator.Step = 2;

            Console.WriteLine("Iterating over collection:");

            for (Item item = iterator.First();
                !iterator.IsDone; item = iterator.Next())
            {
                Console.WriteLine(item.Name);
            }

            // Wait for user

            Console.ReadKey();
        }
    }
    /// <summary>
    /// A collection item
    /// </summary>

    public class Item
    {
        string name;

        // Constructor

        public Item(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }

    /// <summary>
    /// The 'Aggregate' interface
    /// </summary>

    public interface IAbstractCollection
    {
        Iterator CreateIterator();
    }

    /// <summary>
    /// The 'ConcreteAggregate' class
    /// </summary>

    public class Collection : IAbstractCollection
    {
        List<Item> items = new List<Item>();

        public Iterator CreateIterator()
        {
            return new Iterator(this);
        }

        // Gets item count

        public int Count
        {
            get { return items.Count; }
        }

        // Indexer

        public Item this[int index]
        {
            get { return items[index]; }
            set { items.Add(value); }
        }
    }

    /// <summary>
    /// The 'Iterator' interface
    /// </summary>

    public interface IAbstractIterator
    {
        Item First();
        Item Next();
        bool IsDone { get; }
        Item CurrentItem { get; }
    }

    /// <summary>
    /// The 'ConcreteIterator' class
    /// </summary>

    public class Iterator : IAbstractIterator
    {
        Collection collection;
        int current = 0;
        int step = 1;

        // Constructor

        public Iterator(Collection collection)
        {
            this.collection = collection;
        }

        // Gets first item

        public Item First()
        {
            current = 0;
            return collection[current] as Item;
        }

        // Gets next item

        public Item Next()
        {
            current += step;
            if (!IsDone)
                return collection[current] as Item;
            else
                return null;
        }

        // Gets or sets stepsize

        public int Step
        {
            get { return step; }
            set { step = value; }
        }

        // Gets current iterator item

        public Item CurrentItem
        {
            get { return collection[current] as Item; }
        }

        // Gets whether iteration is complete

        public bool IsDone
        {
            get { return current >= collection.Count; }
        }
    }
}
```

**輸出結果 (Output)**

```text
Iterating over collection:
Item 0
Item 2
Item 4
Item 6
Item 8
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors、泛型與 `record` 型別）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Iterator.NetOptimized;

using static System.Console;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Iterator Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create and item collection
        ItemCollection<Item> items = [
                new("Item 0"),
                new("Item 1"),
                new("Item 2"),
                new("Item 3"),
                new("Item 4"),
                new("Item 5"),
                new("Item 6"),
                new("Item 7"),
                new("Item 8")
              ];

        WriteLine("Iterate front to back");
        foreach (var item in items)
        {
            WriteLine(item.Name);
        }

        WriteLine("\nIterate back to front");
        foreach (var item in items.BackToFront)
        {
            WriteLine(item.Name);
        }
        WriteLine();

        // Iterate given range and step over even ones
        WriteLine("\nIterate range (1-7) in steps of 2");
        foreach (var item in items.FromToStep(1, 7, 2))
        {
            WriteLine(item.Name);
        }
        WriteLine();

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'ConcreteAggregate' class
/// </summary>
/// <typeparam name="T">Collection item type</typeparam>
public class ItemCollection<T> : IEnumerable<T>
{
    private readonly List<T> items = [];

    public void Add(T t) => items.Add(t);

    // The 'ConcreteIterator'
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return items[i];
        }
    }

    public IEnumerable<T> FrontToBack { get => this; }

    public IEnumerable<T> BackToFront
    {
        get 
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
    }

    public IEnumerable<T> FromToStep(int from, int to, int step)
    {
        for (int i = from; i <= to; i += step)
        {
            yield return items[i];
        }
    }

    // Gets number of items
    public int Count { get => items.Count; }

    // System.Collections.IEnumerable member implementation
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// The collection item
/// </summary>
internal record Item (string Name);
```

**輸出結果 (Output)**

```text
Iterate front to back
Item 0
Item 1
Item 2
Item 3
Item 4
Item 5
Item 6
Item 7
Item 8

Iterate back to front
Item 8
Item 7
Item 6
Item 5
Item 4
Item 3
Item 2
Item 1
Item 0


Iterate range (1-7) in steps of 2
Item 1
Item 3
Item 5
Item 7
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Interpreter](Interpreter.md) | [🏠 C# Design Patterns 總覽](README.md) | [下一篇：Mediator →](Mediator.md) |

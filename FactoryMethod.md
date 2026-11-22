# C# Factory Method Design Pattern

> **分類**：Creational Patterns（建立型模式）  
> **使用頻率 (Frequency of use)**：高 (High)  
> **原文連結**：[https://www.dofactory.com/net/factory-method-design-pattern](https://www.dofactory.com/net/factory-method-design-pattern)

---

## 定義與概述 (Definition)

**Factory Method** Design Pattern 定義了一個用於建立物件的介面，但讓子類別（Subclasses）決定要實例化（Instantiate）哪一個類別。此 Pattern 讓類別將實例化的動作推遲到子類別中執行。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

履歷與報告都是「文件」，使用方式相同（取得文件、列出每一頁），差別只在於各自有哪些頁面。最直覺的寫法是用一個型別代碼來決定內容：

```csharp
// 示意：用字串決定內容，每新增一種文件就要回來改這個方法
List<Page> CreatePages(string documentType)
{
    if (documentType == "Resume") return [new("Skills"), new("Education"), new("Experience")];
    if (documentType == "Report") return [new("Introduction"), new("Results"), new("Conclusion")];
    throw new ArgumentException("Unknown document type");
}
```

- 每新增一種文件，就要修改這個已經寫好、測試過的方法。
- 字串打錯要到執行期才會發現。

Factory Method 讓基底類別負責共同的流程，並留下一個「建立物件」的抽象方法給子類別實作。新增文件種類時只需要新增一個類別。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Factory Method UML Class Diagram](images/factory.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Product** (`Page`)
  - 定義由 Factory Method 所建立之物件的介面。
- **ConcreteProduct** (`SkillsPage`, `EducationPage`, `ExperiencePage`)
  - 實作 `Product` 介面。
- **Creator** (`Document`)
  - 宣告 Factory Method，該方法會回傳 `Product` 型別的物件。`Creator` 也可以定義 Factory Method 的預設實作，以回傳預設的 `ConcreteProduct` 物件。
  - 可以呼叫 Factory Method 來建立 `Product` 物件。
- **ConcreteCreator** (`Report`, `Resume`)
  - 覆寫（Override）Factory Method，以回傳 `ConcreteProduct` 的實例（Instance）。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/FactoryMethod](samples/FactoryMethod)  
> **執行方式**：`dotnet run --project samples/FactoryMethod`

**角色對照**：Creator = `Document`；ConcreteCreator = `Resume`、`Report`；Product = `Page`；Factory Method = `CreatePages()`。

<!-- sample:FactoryMethod/Program.cs -->
```csharp
// Factory Method：基底類別定義流程，「要建立哪些物件」交給子類別決定。

Document[] documents = [new Resume(), new Report()];

foreach (Document document in documents)
{
    Console.WriteLine($"{document.Title}:");
    foreach (Page page in document.Pages)
    {
        Console.WriteLine($"  {page.Name}");
    }
}

// ---- Product ----
public sealed record Page(string Name);

// ---- Creator ----
public abstract class Document
{
    public abstract string Title { get; }

    // 第一次讀取 Pages 時才呼叫 Factory Method，並用 field 關鍵字把結果存起來。
    // 不在建構函式裡呼叫，是因為那時子類別還沒初始化完成（分析器規則 CA2214）。
    public IReadOnlyList<Page> Pages => field ??= CreatePages();

    // Factory Method：由子類別決定這份文件有哪些頁面
    protected abstract IReadOnlyList<Page> CreatePages();
}

// ---- ConcreteCreator ----
public sealed class Resume : Document
{
    public override string Title => "Resume";

    protected override IReadOnlyList<Page> CreatePages() =>
        [new("Skills"), new("Education"), new("Experience")];
}

public sealed class Report : Document
{
    public override string Title => "Report";

    protected override IReadOnlyList<Page> CreatePages() =>
        [new("Introduction"), new("Results"), new("Conclusion"), new("Summary"), new("Bibliography")];
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:FactoryMethod -->
```text
Resume:
  Skills
  Education
  Experience
Report:
  Introduction
  Results
  Conclusion
  Summary
  Bibliography
```
<!-- /output -->

### 重點解說

- **`protected abstract`**：`CreatePages()` 只給子類別實作，外部呼叫端看不到，也不需要知道它的存在。
- **`field` 關鍵字（C# 14）**：`Pages => field ??= CreatePages();` 的 `field` 代表編譯器替這個屬性產生的欄位，不必另外宣告。`??=` 的意思是「目前是 `null` 才指派」，所以 Factory Method 只會在第一次讀取時被呼叫一次。
- **回傳 `IReadOnlyList<Page>`**：呼叫端可以讀取與列舉，但不能增刪頁面。
- **集合運算式**：`[new("Skills"), new("Education")]` 會依目標型別建立集合；`new("Skills")` 則省略了已經可以推斷的型別名稱 `Page`。

---

## 與 Dofactory 原版的差異

- 原版 Real-world 在 `Document` 的建構函式裡呼叫 `CreatePages()`。基底類別的建構函式執行時，子類別的欄位還沒有初始化，子類別覆寫的方法如果用到自己的欄位就會出錯（分析器規則 CA2214）。本版延後到第一次讀取 `Pages` 才呼叫。
- 原版 .NET Optimized 改由呼叫端自己呼叫 `document.CreatePages()`，忘了呼叫的話 `Pages` 就是 `null`（它被宣告為 `= null!`）。本版的呼叫端不需要記得任何額外步驟。
- 原版的每一種頁面都是一個空類別（`SkillsPage`、`EducationPage` 等共 8 個）。這些頁面只有名稱不同，所以本版只用一個 `record Page(string Name)`。

---

## .NET 中的對應 (In .NET)

- **`IEnumerable<T>.GetEnumerator()`**：每種集合自己決定要建立哪一種列舉器。
- **`DbConnection.CreateCommand()`**：`SqlConnection` 會回傳 `SqlCommand`，呼叫端只需要認得基底型別 `DbCommand`。
- **`ILoggerProvider.CreateLogger(string categoryName)`**：每個記錄提供者建立自己的 `ILogger` 實作。

---

## 何時該用、何時不該用

**適合使用**

- 基底類別有固定的流程，但其中「要建立哪一種物件」必須留給子類別決定。
- 撰寫框架或函式庫，想讓使用者能換掉某個被建立的型別。

**不適合使用**

- 只是想把 `new` 藏起來：一個靜態方法或 DI 就夠了。
- 為了一個 `new` 而多出一整個繼承階層。

---

## 練習題

1. 新增 `Invoice` 文件，頁面為 Header、Line Items、Total。確認不需要修改 `Document` 與最上方的迴圈。
2. 在 `Document` 加上 `PageCount` 屬性。它需要知道任何子類別嗎？
3. 在 `CreatePages()` 裡加一行 `Console.WriteLine`，確認同一份文件讀取 `Pages` 兩次時只會印出一次。

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Factory Method Pattern 在建立不同物件時所提供的極大彈性。抽象類別可以提供預設物件，但每個子類別都能實例化該物件的擴充版本。

```csharp
using System;

namespace DoFactory.GangOfFour.Factory.Structural
{
    /// <summary>
    /// MainApp startup class for Structural 
    /// Factory Method Design Pattern.
    /// </summary>

    class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        static void Main()
        {
            // An array of creators

            Creator[] creators = new Creator[2];

            creators[0] = new ConcreteCreatorA();
            creators[1] = new ConcreteCreatorB();

            // Iterate over creators and create products

            foreach (Creator creator in creators)
            {
                Product product = creator.FactoryMethod();
                Console.WriteLine("Created {0}",
                  product.GetType().Name);
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Product' abstract class
    /// </summary>

    abstract class Product
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class ConcreteProductA : Product
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class ConcreteProductB : Product
    {
    }

    /// <summary>
    /// The 'Creator' abstract class
    /// </summary>

    abstract class Creator
    {
        public abstract Product FactoryMethod();
    }

    /// <summary>
    /// A 'ConcreteCreator' class
    /// </summary>

    class ConcreteCreatorA : Creator
    {
        public override Product FactoryMethod()
        {
            return new ConcreteProductA();
        }
    }

    /// <summary>
    /// A 'ConcreteCreator' class
    /// </summary>

    class ConcreteCreatorB : Creator
    {
        public override Product FactoryMethod()
        {
            return new ConcreteProductB();
        }
    }
}
```

**輸出結果 (Output)**

```text
Created ConcreteProductA
Created ConcreteProductB
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Factory Method Pattern 在建立不同文件（Documents）時所提供的彈性。衍生自 `Document` 的 `Report` 與 `Resume` 類別會實例化 `Document` 類別的擴充頁面版本。在此範例中，Factory Method 是在 `Document` 基底類別（Base Class）的建構函式（Constructor）中被呼叫的。

```csharp
using System;
using System.Collections.Generic;

namespace DoFactory.GangOfFour.Factory.RealWorld
{
    /// <summary>
    /// MainApp startup class for Real-World 
    /// Factory Method Design Pattern.
    /// </summary>

    class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>

        static void Main()
        {
            // Note: constructors call Factory Method

            Document[] documents = new Document[2];

            documents[0] = new Resume();
            documents[1] = new Report();

            // Display document pages

            foreach (Document document in documents)
            {
                Console.WriteLine("\n" + document.GetType().Name + "--");
                foreach (Page page in document.Pages)
                {
                    Console.WriteLine(" " + page.GetType().Name);
                }
            }

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Product' abstract class
    /// </summary>

    abstract class Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class SkillsPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class EducationPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class ExperiencePage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class IntroductionPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class ResultsPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class ConclusionPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class SummaryPage : Page
    {
    }

    /// <summary>
    /// A 'ConcreteProduct' class
    /// </summary>

    class BibliographyPage : Page
    {
    }

    /// <summary>
    /// The 'Creator' abstract class
    /// </summary>

    abstract class Document
    {
        private List<Page> _pages = new List<Page>();

        // Constructor calls abstract Factory method

        public Document()
        {
            this.CreatePages();
        }

        public List<Page> Pages
        {
            get { return _pages; }
        }

        // Factory Method

        public abstract void CreatePages();
    }

    /// <summary>
    /// A 'ConcreteCreator' class
    /// </summary>

    class Resume : Document
    {
        // Factory Method implementation

        public override void CreatePages()
        {
            Pages.Add(new SkillsPage());
            Pages.Add(new EducationPage());
            Pages.Add(new ExperiencePage());
        }
    }

    /// <summary>
    /// A 'ConcreteCreator' class
    /// </summary>

    class Report : Document
    {
        // Factory Method implementation

        public override void CreatePages()
        {
            Pages.Add(new IntroductionPage());
            Pages.Add(new ResultsPage());
            Pages.Add(new ConclusionPage());
            Pages.Add(new SummaryPage());
            Pages.Add(new BibliographyPage());
        }
    }
}
```

**輸出結果 (Output)**

```text
Resume--
 SkillsPage
 EducationPage
 ExperiencePage

Report--
 IntroductionPage
 ResultsPage
 ConclusionPage
 SummaryPage
 BibliographyPage
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如集合運算式 `[]` 與簡潔的屬性語法）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Factory.NetOptimized;

using static System.Console;

/// <summary>
/// Factory Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Document constructors call Factory Method
        List<Document> documents = [new Resume(), new Report()];

        // Display document pages
        foreach (var document in documents)
        {
            document.CreatePages();  // Factory method

            WriteLine($"{document} --");
            foreach (var page in document.Pages) WriteLine($" {page}");
            WriteLine();
        }

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Product' abstract class
/// </summary>
public abstract class Page
{
    // Override. Display class name
    public override string ToString() => GetType().Name;
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class SkillsPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class EducationPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class ExperiencePage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class IntroductionPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>    
public class ResultsPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class ConclusionPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class SummaryPage : Page
{
}

/// <summary>
/// A 'ConcreteProduct' class
/// </summary>
public class BibliographyPage : Page
{
}

/// <summary>
/// The 'Creator' abstract class
/// </summary>
public abstract class Document
{
    // Gets list of document pages
    public List<Page> Pages { get; protected set; } = null!;

    // Factory Method
    public abstract void CreatePages();

    // Override
    public override string ToString() => GetType().Name;
}

/// <summary>
/// A 'ConcreteCreator' class
/// </summary>
public class Resume : Document
{
    // Factory Method implementation
    public override void CreatePages()
    {
        Pages =
        [
            new SkillsPage(),
            new EducationPage(),
            new ExperiencePage()
        ];
    }
}

/// <summary>
/// A 'ConcreteCreator' class
/// </summary>
public class Report : Document
{
    // Factory Method implementation
    public override void CreatePages()
    {
        Pages =
        [
            new IntroductionPage(),
            new ResultsPage(),
            new ConclusionPage(),
            new SummaryPage(),
            new BibliographyPage()
        ];
    }
}
```

**輸出結果 (Output)**

```text
Resume --
 SkillsPage
 EducationPage
 ExperiencePage

Report --
 IntroductionPage
 ResultsPage
 ConclusionPage
 SummaryPage
 BibliographyPage
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Builder](Builder.md) | [🏠 C# Design Patterns 總覽](README.md) | [下一篇：Prototype →](Prototype.md) |

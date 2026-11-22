# C# Design Patterns (設計模式總覽)

> **資料來源**：[Dofactory - C# / .NET Design Patterns](https://www.dofactory.com/net/design-patterns)  
> **說明**：本系列以 Dofactory 網站的 23 種 Gang of Four (GoF) Design Patterns 為基礎。每一篇都以一個**可以直接建置與執行的現代 C# 範例**（.NET 11 / C# 15）為主軸，說明這個模式要解決什麼問題、在 .NET 裡的實際樣貌，以及什麼時候不該使用；Dofactory 原版的三種程式碼則保留在各篇的附錄作為對照。

---

## 什麼是 Design Patterns？ (What are Design Patterns?)

Design Patterns（設計模式）是在真實世界的應用程式開發中，針對一再出現的軟體設計問題所提出的解決方案。Patterns 的核心在於可重複使用的設計以及物件之間的互動方式。

23 種 **Gang of Four (GoF)** Patterns 通常被視為所有其他 Patterns 的基礎。它們被分為三大類別：**Creational（建立型）**、**Structural（結構型）** 與 **Behavioral（行為型）**（完整清單請見下方）。

學習設計模式時，最重要的不是背下類別圖，而是能回答兩個問題：**它解決了什麼問題？** 以及 **什麼時候不需要它？** 每篇文章都是依照這個方向編排的。

---

## 每篇文章的結構

| 章節 | 內容 |
| :--- | :--- |
| **定義與概述** | GoF 對這個模式的定義 |
| **問題情境** | 不使用這個模式時，程式碼會遇到什麼困難 |
| **UML 類別圖、參與者** | 模式的標準結構與每個角色的職責 |
| **現代 C# 實作** | 完整的範例程式碼與實際執行的輸出，並附上「角色對照」與「重點解說」 |
| **與 Dofactory 原版的差異** | 現代版改了什麼、為什麼要改（包含原版的錯誤與過時寫法） |
| **.NET 中的對應** | 這個模式在 .NET 類別庫與常見框架裡的實際例子 |
| **何時該用、何時不該用** | 適用與不適用的情境 |
| **練習題** | 以範例專案為起點的動手練習 |
| **附錄：Dofactory 原版範例** | Structural、Real-world、.NET Optimized 三種原版程式碼（預設收合） |

---

## 建議的閱讀順序

不需要依照 GoF 的分類順序閱讀。剛接觸設計模式時，建議從日常開發最常遇到、而且 C# 已經內建支援的模式開始：

1. **先認識每天都在用的**：[Strategy](Strategy.md) → [Observer](Observer.md) → [Iterator](Iterator.md) → [Singleton](Singleton.md) → [Factory Method](FactoryMethod.md)
2. **再學組合物件的技巧**：[Decorator](Decorator.md) → [Adapter](Adapter.md) → [Facade](Facade.md) → [Composite](Composite.md) → [Proxy](Proxy.md)
3. **接著是流程與狀態**：[Template Method](TemplateMethod.md) → [State](State.md) → [Command](Command.md) → [Chain of Responsibility](ChainOfResponsibility.md) → [Builder](Builder.md)
4. **最後是較少用到的**：[Abstract Factory](AbstractFactory.md) → [Prototype](Prototype.md) → [Bridge](Bridge.md) → [Mediator](Mediator.md) → [Memento](Memento.md) → [Visitor](Visitor.md) → [Flyweight](Flyweight.md) → [Interpreter](Interpreter.md)

---

## 執行範例程式

所有範例都放在 [samples/](samples/) 資料夾，每個模式是一個獨立的主控台專案，只有一個 `Program.cs`。

**需求**：[.NET 11 SDK](https://dotnet.microsoft.com/download)

```bash
# 執行單一範例
dotnet run --project samples/Strategy

# 建置全部範例
dotnet build samples/DesignPatterns.slnx
```

也可以用 Visual Studio 或 VS Code 開啟 `samples/DesignPatterns.slnx`。

### 範例的撰寫慣例

- **一個模式一個檔案**：`Program.cs` 的開頭是示範用的主程式（最上層陳述式），後面是各個型別，並以註解標示它在模式中的角色。
- **程式輸出使用英文、註解使用中文**：避免主控台編碼造成中文輸出變成亂碼。
- **金額一律使用 `decimal`**：`double` 無法精確表示十進位小數。
- **金額以 `${amount:N2}` 格式化**：`{amount:C}` 會依作業系統的語系輸出不同的貨幣符號（在台灣是 `NT$`），不同電腦的輸出會不一致。`N2` 的千分位與小數點符號同樣會隨語系改變，所以同步腳本執行範例時固定使用不變文化特性（Invariant Culture）。
- **啟用可為 Null 的參考型別，並把警告視為錯誤**：設定在 [samples/Directory.Build.props](samples/Directory.Build.props)。

### 文章裡的程式碼從哪裡來

文章中「現代 C# 實作」一節的程式碼與輸出，是由腳本從 `samples/` 同步過來的，不是手動貼上的：

```bash
pwsh tools/sync-samples.ps1          # 建置、執行全部範例，並把程式碼與輸出寫回文章
pwsh tools/sync-samples.ps1 -Check   # 只檢查不修改；有任何不一致就回報錯誤（CI 使用）
```

修改範例的流程是：編輯 `samples/<Pattern>/Program.cs` → 執行腳本 → 一併提交。文章中 `<!-- sample:... -->` 與 `<!-- output:... -->` 標記之間的內容請不要手動修改。

「問題情境」一節裡標示為「示意」的程式碼片段，是用來說明問題的簡化寫法，不屬於範例專案，沒有經過編譯。

---

## Creational Patterns (建立型模式)

建立型模式專注於物件的實例化（Instantiation）機制，將物件的建立與使用分離。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 範例專案 |
| :--- | :--- | :---: | :--- |
| **[Abstract Factory](AbstractFactory.md)** | 建立數個類別家族（Families of classes）的實例 | 高 (High) | [samples/AbstractFactory](samples/AbstractFactory) |
| **[Builder](Builder.md)** | 將複雜物件的建構過程與其表示方式（Representation）分離 | 中低 (Medium-Low) | [samples/Builder](samples/Builder) |
| **[Factory Method](FactoryMethod.md)** | 建立數個衍生類別（Derived classes）的實例 | 高 (High) | [samples/FactoryMethod](samples/FactoryMethod) |
| **[Prototype](Prototype.md)** | 一個已完整初始化的實例，用於被複製（Copied 或 Cloned） | 中 (Medium) | [samples/Prototype](samples/Prototype) |
| **[Singleton](Singleton.md)** | 一個只能存在單一實例（Single instance）的類別 | 中高 (Medium-High) | [samples/Singleton](samples/Singleton) |

---

## Structural Patterns (結構型模式)

結構型模式專注於類別與物件如何組合成更大的結構，同時保持結構的彈性與效率。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 範例專案 |
| :--- | :--- | :---: | :--- |
| **[Adapter](Adapter.md)** | 使不同類別的介面（Interfaces）能夠相容匹配 | 中高 (Medium-High) | [samples/Adapter](samples/Adapter) |
| **[Bridge](Bridge.md)** | 將物件的介面（抽象）與其實作（Implementation）分離 | 中低 (Medium-Low) | [samples/Bridge](samples/Bridge) |
| **[Composite](Composite.md)** | 由簡單物件與組合物件所構成的樹狀結構（Tree structure） | 中高 (Medium-High) | [samples/Composite](samples/Composite) |
| **[Decorator](Decorator.md)** | 動態地為物件加上額外的職責（Responsibilities） | 中 (Medium) | [samples/Decorator](samples/Decorator) |
| **[Facade](Facade.md)** | 代表整個子系統（Subsystem）的單一入口類別 | 高 (High) | [samples/Facade](samples/Facade) |
| **[Flyweight](Flyweight.md)** | 用於高效率共享的細粒度（Fine-grained）實例 | 低 (Low) | [samples/Flyweight](samples/Flyweight) |
| **[Proxy](Proxy.md)** | 代表另一個物件的代理物件，用於控制存取 | 中高 (Medium-High) | [samples/Proxy](samples/Proxy) |

---

## Behavioral Patterns (行為型模式)

行為型模式專注於物件之間的通訊、職責分配與演算法的封裝。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 範例專案 |
| :--- | :--- | :---: | :--- |
| **[Chain of Responsibility](ChainOfResponsibility.md)** | 在一條物件鏈（Chain of objects）之間傳遞請求的方法 | 中低 (Medium-Low) | [samples/ChainOfResponsibility](samples/ChainOfResponsibility) |
| **[Command](Command.md)** | 將命令請求封裝為一個物件 | 中高 (Medium-High) | [samples/Command](samples/Command) |
| **[Interpreter](Interpreter.md)** | 在程式中納入語言文法元素並進行直譯的方法 | 低 (Low) | [samples/Interpreter](samples/Interpreter) |
| **[Iterator](Iterator.md)** | 循序存取集合（Collection）中的各個元素 | 高 (High) | [samples/Iterator](samples/Iterator) |
| **[Mediator](Mediator.md)** | 定義類別之間簡化的集中式通訊方式 | 中低 (Medium-Low) | [samples/Mediator](samples/Mediator) |
| **[Memento](Memento.md)** | 捕捉並還原物件的內部狀態（Internal state） | 低 (Low) | [samples/Memento](samples/Memento) |
| **[Observer](Observer.md)** | 將狀態變更通知給多個相依類別的方法 | 高 (High) | [samples/Observer](samples/Observer) |
| **[State](State.md)** | 當物件的內部狀態改變時，改變該物件的行為 | 中 (Medium) | [samples/State](samples/State) |
| **[Strategy](Strategy.md)** | 將演算法封裝在類別內部，使其可互相替換 | 高 (High) | [samples/Strategy](samples/Strategy) |
| **[Template Method](TemplateMethod.md)** | 將演算法的確切步驟推遲（Defer）到子類別中實作 | 中 (Medium) | [samples/TemplateMethod](samples/TemplateMethod) |
| **[Visitor](Visitor.md)** | 在不修改類別的情況下，為類別定義一個新的操作 | 低 (Low) | [samples/Visitor](samples/Visitor) |

---

## 範例中用到的 C# 與 .NET 功能

現代版範例的原則是「由模式決定要用哪些語言功能」，而不是為了展示新語法而使用。下表列出範例實際用到的功能，以及可以在哪一篇看到它。

| 功能 | 加入的版本 | 可以在哪裡看到 | 在範例中的用途 |
| :--- | :---: | :--- | :--- |
| **`closed` 類別階層、`union` 型別** | C# 15 | [Visitor](Visitor.md) | 讓編譯器檢查 `switch` 是否涵蓋所有型別，取代傳統 Visitor 的 `Accept` / `Visit` |
| **`field` 關鍵字** | C# 14 | [Observer](Observer.md)、[Factory Method](FactoryMethod.md) | 在屬性存取子裡使用編譯器產生的欄位，不必另外宣告 |
| **`System.Threading.Lock`** | .NET 9 | [Singleton](Singleton.md) | 保護 Singleton 的可變狀態 |
| **主要建構函式（Primary Constructor）** | C# 12 | [Adapter](Adapter.md)、[Decorator](Decorator.md)、[Facade](Facade.md) 等多數範例 | 以建構函式接收相依物件 |
| **集合運算式 `[...]`** | C# 12 | [Iterator](Iterator.md)、[Composite](Composite.md)、[Prototype](Prototype.md) 等 | 初始化陣列、清單與自訂集合 |
| **`record` 與 `with` 運算式** | C# 9 | [Prototype](Prototype.md)、[Visitor](Visitor.md)、[Command](Command.md)、[Memento](Memento.md) | 不可變的資料物件與複製 |
| **`switch` 運算式與模式比對** | C# 8 / 9 | [State](State.md)、[Visitor](Visitor.md)、[Abstract Factory](AbstractFactory.md) | 集中表達對應規則 |
| **`IAsyncEnumerable<T>` 與 `await foreach`** | C# 8 | [Iterator](Iterator.md) | 走訪需要等待才能取得的資料 |
| **`yield return`** | C# 2 | [Iterator](Iterator.md) | 由編譯器產生列舉器 |
| **`event` 與 `EventHandler<T>`** | C# 1 / 2 | [Observer](Observer.md) | 語言內建的 Observer |
| **委派 `Func<>`** | C# 3 | [Strategy](Strategy.md) | 以 lambda 作為策略 |
| **`Lazy<T>`** | .NET Framework 4 | [Singleton](Singleton.md)、[Proxy](Proxy.md) | 執行緒安全的延後建立 |
| **相依性注入（`AddSingleton`）** | Microsoft.Extensions.DependencyInjection | [Singleton](Singleton.md) | 由容器管理物件的生命週期 |

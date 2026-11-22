# C# Design Patterns (設計模式總覽)

> **資料來源**：[Dofactory - C# / .NET Design Patterns](https://www.dofactory.com/net/design-patterns)  
> **說明**：本系列文件完整收錄並翻譯 Dofactory 網站上的 23 種 Gang of Four (GoF) C# Design Patterns，包含 UML 類別圖、參與者角色說明，以及 **Structural（結構化）**、**Real-world（真實世界）** 與 **.NET Optimized（.NET 最佳化）** 三種完整 C# 原始碼與執行輸出結果，並額外針對 **.NET 11 與 C# 15** 補充第 4 種「**現代化實務優化版本**」與架構檢討。

---

## 什麼是 Design Patterns？ (What are Design Patterns?)

Design Patterns（設計模式）是在真實世界的應用程式開發中，針對一再出現的軟體設計問題所提出的解決方案。Patterns 的核心在於可重複使用的設計以及物件之間的互動方式。

23 種 **Gang of Four (GoF)** Patterns 通常被視為所有其他 Patterns 的基礎。它們被分為三大類別：**Creational（建立型）**、**Structural（結構型）** 與 **Behavioral（行為型）**（完整清單請見下方）。本參考指南為 23 種 GoF Patterns 的每一種都提供了完整的 C# 原始碼。

---

## C# Design Patterns 的四種程式碼形式

為了讓你能夠快速上手並掌握最新技術，每一種 Pattern 的 C# 原始碼都提供以下 4 種形式：**Structural（結構化）**、**Real-world（真實世界）**、**.NET Optimized（Dofactory 原版最佳化）** 以及 **.NET 11 / C# 15 現代化實務優化**。

- **Structural（結構化程式碼）**：使用在 Pattern 定義與 UML 類別圖中所定義的標準型別名稱，清晰呈現該 Pattern 的核心架構與運作機制。
- **Real-world（真實世界程式碼）**：提供真實世界的程式設計情境，展示你在實務開發中可能會如何應用這些 Patterns。
- **.NET Optimized（Dofactory 原版 .NET 最佳化程式碼）**：保留 Dofactory 網站原版提供的 C# 現代化程式碼（約對應 C# 10~12 語法，如 Primary Constructors、基本 Records 與泛型）。
- **🚀 .NET 11 / C# 15 現代化實務優化（Modern .NET 11 & C# 15）**：深入檢討原版程式碼的效能盲點或潛在 Bug（如使用 JSON 序列化做 Clone、`static` 欄位狀態污染、基底建構函式呼叫虛擬方法等），並提供採用 **C# 15 / .NET 11** 最新特性的實戰程式碼！

---

## Creational Patterns (建立型模式)

建立型模式專注於物件的實例化（Instantiation）機制，將物件的建立與使用分離。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 檔案連結 |
| :--- | :--- | :---: | :--- |
| **[Abstract Factory](AbstractFactory.md)** | 建立數個類別家族（Families of classes）的實例 | 高 (High) | [AbstractFactory.md](AbstractFactory.md) |
| **[Builder](Builder.md)** | 將複雜物件的建構過程與其表示方式（Representation）分離 | 中低 (Medium-Low) | [Builder.md](Builder.md) |
| **[Factory Method](FactoryMethod.md)** | 建立數個衍生類別（Derived classes）的實例 | 高 (High) | [FactoryMethod.md](FactoryMethod.md) |
| **[Prototype](Prototype.md)** | 一個已完整初始化的實例，用於被複製（Copied 或 Cloned） | 中 (Medium) | [Prototype.md](Prototype.md) |
| **[Singleton](Singleton.md)** | 一個只能存在單一實例（Single instance）的類別 | 中高 (Medium-High) | [Singleton.md](Singleton.md) |

---

## Structural Patterns (結構型模式)

結構型模式專注於類別與物件如何組合成更大的結構，同時保持結構的彈性與效率。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 檔案連結 |
| :--- | :--- | :---: | :--- |
| **[Adapter](Adapter.md)** | 使不同類別的介面（Interfaces）能夠相容匹配 | 中高 (Medium-High) | [Adapter.md](Adapter.md) |
| **[Bridge](Bridge.md)** | 將物件的介面（抽象）與其實作（Implementation）分離 | 中低 (Medium-Low) | [Bridge.md](Bridge.md) |
| **[Composite](Composite.md)** | 由簡單物件與組合物件所構成的樹狀結構（Tree structure） | 中高 (Medium-High) | [Composite.md](Composite.md) |
| **[Decorator](Decorator.md)** | 動態地為物件加上額外的職責（Responsibilities） | 中 (Medium) | [Decorator.md](Decorator.md) |
| **[Facade](Facade.md)** | 代表整個子系統（Subsystem）的單一入口類別 | 高 (High) | [Facade.md](Facade.md) |
| **[Flyweight](Flyweight.md)** | 用於高效率共享的細粒度（Fine-grained）實例 | 低 (Low) | [Flyweight.md](Flyweight.md) |
| **[Proxy](Proxy.md)** | 代表另一個物件的代理物件，用於控制存取 | 中高 (Medium-High) | [Proxy.md](Proxy.md) |

---

## Behavioral Patterns (行為型模式)

行為型模式專注於物件之間的通訊、職責分配與演算法的封裝。

| Pattern 名稱 | 說明 (Description) | 使用頻率 (Frequency) | 檔案連結 |
| :--- | :--- | :---: | :--- |
| **[Chain of Responsibility](ChainOfResponsibility.md)** | 在一條物件鏈（Chain of objects）之間傳遞請求的方法 | 中低 (Medium-Low) | [ChainOfResponsibility.md](ChainOfResponsibility.md) |
| **[Command](Command.md)** | 將命令請求封裝為一個物件 | 中高 (Medium-High) | [Command.md](Command.md) |
| **[Interpreter](Interpreter.md)** | 在程式中納入語言文法元素並進行直譯的方法 | 低 (Low) | [Interpreter.md](Interpreter.md) |
| **[Iterator](Iterator.md)** | 循序存取集合（Collection）中的各個元素 | 高 (High) | [Iterator.md](Iterator.md) |
| **[Mediator](Mediator.md)** | 定義類別之間簡化的集中式通訊方式 | 中低 (Medium-Low) | [Mediator.md](Mediator.md) |
| **[Memento](Memento.md)** | 捕捉並還原物件的內部狀態（Internal state） | 低 (Low) | [Memento.md](Memento.md) |
| **[Observer](Observer.md)** | 將狀態變更通知給多個相依類別的方法 | 高 (High) | [Observer.md](Observer.md) |
| **[State](State.md)** | 當物件的內部狀態改變時，改變該物件的行為 | 中 (Medium) | [State.md](State.md) |
| **[Strategy](Strategy.md)** | 將演算法封裝在類別內部，使其可互相替換 | 高 (High) | [Strategy.md](Strategy.md) |
| **[Template Method](TemplateMethod.md)** | 將演算法的確切步驟推遲（Defer）到子類別中實作 | 中 (Medium) | [TemplateMethod.md](TemplateMethod.md) |
| **[Visitor](Visitor.md)** | 在不修改類別的情況下，為類別定義一個新的操作 | 低 (Low) | [Visitor.md](Visitor.md) |

---

## 🚀 .NET 11 與 C# 15 現代化實務優化總整理

Dofactory 原版的 `.NET Optimized` 範例大多停留在早期 C# 語法糖，部分寫法在現代 .NET 11 高效能與雲端原生開發中已不再推薦。本知識庫在每一篇 Pattern 的最後一節，皆針對 **.NET 11 與 C# 15** 提供了深度檢討與改寫範例，核心優化重點如下：

| C# 15 / .NET 11 語言與框架特性 | 應用的 Design Patterns | 解決的原版痛點與優化效益 |
| :--- | :--- | :--- |
| **C# 15 `union`（聯合型別）與 `closed`（封閉繼承階層）** | [Visitor](Visitor.md)、[State](State.md)、[Factory Method](FactoryMethod.md)、[Flyweight](Flyweight.md) | 讓編譯器在編譯期強制進行**窮舉式模式比對（Exhaustive Pattern Matching）**，徹底免除傳統 Visitor 笨重的 `Accept(IVisitor)` 雙重分派樣板與 State 散落各處的 `if-else`。 |
| **C# 15 集合運算式參數 `[with(capacity: ...)]`** | [Builder](Builder.md)、[Bridge](Bridge.md)、[Decorator](Decorator.md)、[Template Method](TemplateMethod.md) | 在集合運算式開頭直接傳入預留容量或比較器（如 `[with(capacity: 8), ..items]`），兼具簡潔語法與零動態擴容開銷。 |
| **C# 14/15 `field` 關鍵字（Field-backed Properties）** | [Observer](Observer.md)、[Memento](Memento.md) | 在屬性 `set` 存取子中直接存取編譯器合成的 `field`，省去所有手動宣告的 `private` 背端欄位（Backing Fields）。 |
| **Static Abstract Interface Members（靜態抽象介面成員）** | [Abstract Factory](AbstractFactory.md) | 取代 `where T : new()` 的執行期反射實例化開銷，達成編譯期靜態分派與零工廠實例配置（Zero-allocation）。 |
| **`record` / `readonly record struct` 與 `with` 運算式** | [Prototype](Prototype.md)、[Command](Command.md)、[Observer](Observer.md)、[Facade](Facade.md) | 修正原版 Prototype 與 Memento 濫用 `JsonSerializer` 進行複製的嚴重效能陷阱，改用語言原生的非破壞性複製（Non-destructive Mutation）與堆疊配置（Stack Allocation）。 |
| **`params ReadOnlySpan<T>` 與 `ReadOnlySpan<char>` 切片** | [Composite](Composite.md)、[Interpreter](Interpreter.md)、[Mediator](Mediator.md) | 批次加入樹狀節點或剖析羅馬數字文法時，達成**零陣列與零子字串記憶體配置（Zero Heap Allocation）**。 |
| **原版潛在 Bug 修正與執行緒安全強化** | [Decorator](Decorator.md)、[Flyweight](Flyweight.md)、[Factory Method](FactoryMethod.md)、[Singleton](Singleton.md) | 修正原版 Decorator 誤將 `NumCopies` 設為 `static` 的嚴重 Bug、Flyweight 將外部狀態存入共享物件的 Race Condition、Factory Method 在基底建構函式呼叫虛擬方法（CA2214），以及 Singleton 改用 `Random.Shared` 與 `ImmutableArray`。 |


# C# Mediator Design Pattern

> **分類**：Behavioral Patterns（行為型模式）  
> **使用頻率 (Frequency of use)**：中低 (Medium-Low)  
> **原文連結**：[https://www.dofactory.com/net/mediator-design-pattern](https://www.dofactory.com/net/mediator-design-pattern)

---

## 定義與概述 (Definition)

**Mediator** Design Pattern 定義了一個物件，用來封裝一組物件之間的互動方式。Mediator 藉由避免物件之間明確地互相參考（Referring），促進了鬆散耦合（Loose Coupling），並讓你可以獨立地改變它們之間的互動。

本篇分成兩個部分：

- **[現代 C# 實作](#現代-c-實作-net-11--c-15)**：可以直接建置與執行的範例專案，建議由此開始閱讀。
- **[附錄：Dofactory 原版範例](#附錄dofactory-原版範例)**：Structural、Real-world、.NET Optimized 三種原版程式碼，作為對照。

---

## 問題情境 (Problem)

聊天室裡，如果每個參與者都直接持有其他人的參考並互相呼叫，5 個人就有 20 條單向的關係；每加入一個人，所有既有的參與者都要認識他。而且「誰可以傳訊息給誰」這類規則會散落在每個參與者的程式碼裡。

Mediator 引入一個中介者：所有參與者只認識中介者，訊息都交給它轉送。參與者之間不再互相參考，互動的規則也集中在中介者一處。

---

## UML 類別圖 (UML Class Diagram)

以下是參與此 Pattern 的類別與物件之視覺化關係圖：

![Mediator UML Class Diagram](images/mediator.png)

---

## 參與者 (Participants)

參與此 Pattern 的類別與物件包括：

> 括號內是 Dofactory 原版範例（見附錄）使用的類別名稱。現代 C# 實作中各角色對應的型別，請見下一節開頭的「角色對照」。

- **Mediator** (`IChatroom`)
  - 定義一個用於與各個 `Colleague`（同仁）物件進行通訊的介面。
- **ConcreteMediator** (`Chatroom`)
  - 透過協調各個 `Colleague` 物件來實作協同合作的行為。
  - 認識並維護其所有的 `Colleague` 物件。
- **Colleague classes** (`Participant`)
  - 每一個 `Colleague` 類別都知道它的 `Mediator` 物件。
  - 每當一個 `Colleague` 需要與另一個 `Colleague` 通訊時，都會改為透過其 `Mediator` 進行通訊。

---

## 現代 C# 實作 (.NET 11 / C# 15)

> **範例專案**：[samples/Mediator](samples/Mediator)  
> **執行方式**：`dotnet run --project samples/Mediator`

**角色對照**：Mediator = `IChatroom`；ConcreteMediator = `Chatroom`；Colleague = `Participant`；ConcreteColleague = `Beatle`、`NonBeatle`。

<!-- sample:Mediator/Program.cs -->
```csharp
// Mediator：物件之間不直接互相呼叫，而是都透過一個「中介者」溝通，彼此就不必互相認識。

var chatroom = new Chatroom();

Participant george = new Beatle("George");
Participant paul = new Beatle("Paul");
Participant ringo = new Beatle("Ringo");
Participant john = new Beatle("John");
Participant yoko = new NonBeatle("Yoko");

chatroom.Join(george, paul, ringo, john, yoko);

yoko.Send("John", "Hi John!");
paul.Send("Ringo", "All you need is love");
ringo.Send("George", "My sweet Lord");
john.Send("Yoko", "My sweet love");

// 收件人不在聊天室：由 Mediator 統一決定怎麼處理，而不是每個參與者各寫一次
paul.Send("Pete", "Are you there?");

// ---- Mediator ----
public interface IChatroom
{
    void Join(params Participant[] participants);
    void Send(string from, string to, string message);
}

// ---- ConcreteMediator：所有「誰可以傳給誰、怎麼傳」的規則都集中在這裡 ----
public sealed class Chatroom : IChatroom
{
    private readonly Dictionary<string, Participant> _participants = new(StringComparer.OrdinalIgnoreCase);

    public void Join(params Participant[] participants)
    {
        foreach (Participant participant in participants)
        {
            _participants[participant.Name] = participant;
            participant.Chatroom = this;
        }
    }

    public void Send(string from, string to, string message)
    {
        // 用 TryGetValue 查詢：索引子 _participants[to] 在找不到時會直接丟出 KeyNotFoundException
        if (_participants.TryGetValue(to, out Participant? recipient))
        {
            recipient.Receive(from, message);
        }
        else
        {
            Console.WriteLine($"[Chatroom] '{to}' is not in the room; message from {from} was not delivered.");
        }
    }
}

// ---- Colleague：只認得 Mediator，不認得其他參與者 ----
public abstract class Participant(string name)
{
    public string Name => name;

    // 由 Chatroom.Join 設定
    public IChatroom? Chatroom { get; set; }

    public void Send(string to, string message)
    {
        if (Chatroom is null)
        {
            throw new InvalidOperationException($"{Name} has not joined a chatroom.");
        }

        Chatroom.Send(Name, to, message);
    }

    public virtual void Receive(string from, string message) =>
        Console.WriteLine($"{from} to {Name}: '{message}'");
}

// ---- ConcreteColleague ----
public sealed class Beatle(string name) : Participant(name)
{
    public override void Receive(string from, string message)
    {
        Console.Write("To a Beatle: ");
        base.Receive(from, message);
    }
}

public sealed class NonBeatle(string name) : Participant(name)
{
    public override void Receive(string from, string message)
    {
        Console.Write("To a non-Beatle: ");
        base.Receive(from, message);
    }
}
```
<!-- /sample -->

### 輸出結果 (Output)

<!-- output:Mediator -->
```text
To a Beatle: Yoko to John: 'Hi John!'
To a Beatle: Paul to Ringo: 'All you need is love'
To a Beatle: Ringo to George: 'My sweet Lord'
To a non-Beatle: John to Yoko: 'My sweet love'
[Chatroom] 'Pete' is not in the room; message from Paul was not delivered.
```
<!-- /output -->

### 重點解說

- **參與者只認得 `IChatroom`**：`Participant` 的程式碼裡沒有任何其他參與者的參考，傳訊息時只提供對方的名字。
- **互動規則集中**：「收件人不在聊天室」的處理只寫在 `Chatroom.Send` 一處（輸出最後一行）。
- **`TryGetValue`**：一次完成「檢查是否存在」與「取出」。直接用索引子 `_participants[to]`，在找不到時會丟出 `KeyNotFoundException`。
- **`StringComparer.OrdinalIgnoreCase`**：建立字典時指定比較方式，查詢名字時不分大小寫。
- **`params`**：`Join(george, paul, ringo)` 可以一次傳入任意數量的參與者。

---

## 與 Dofactory 原版的差異

- 原版的 `Send` 以 `participants[to]` 取出收件人，再檢查是否為 `null`。但 `Dictionary` 的索引子在找不到鍵時會丟出 `KeyNotFoundException`，不會回傳 `null`，所以那個檢查永遠不會生效，傳訊息給不存在的人會讓程式直接中止。本版改用 `TryGetValue`。
- 原版的參與者還沒加入聊天室就呼叫 `Send`，會發生 `NullReferenceException`。本版丟出說明原因的 `InvalidOperationException`。

---

## .NET 中的對應 (In .NET)

- **MVVM 訊息匯流排**：CommunityToolkit.Mvvm 的 `IMessenger` 與 `WeakReferenceMessenger`，讓 ViewModel 之間不必互相參考。
- **行程內的訊息派送**：MediatR、Wolverine 等套件，傳送端只送出一個請求物件，不需要知道由哪個處理常式負責（採用前請留意各套件的授權條款）。
- **SignalR Hub**：用戶端之間不直接連線，所有訊息都經由 Hub 轉送。

---

## 何時該用、何時不該用

**適合使用**

- 多個物件之間有複雜的多對多互動。
- 想把互動規則集中管理，而不是散落在每個物件裡。

**不適合使用**

- 只有兩三個物件互動：直接呼叫更清楚。
- 中介者累積了太多規則，變成什麼都管的巨大類別。
- 單純的一對多通知：那是 Observer。

---

## 練習題

1. 加上 `Broadcast(string from, string message)`，把訊息傳給自己以外的所有人。
2. 加上封鎖功能：Yoko 封鎖 Paul 之後，就收不到 Paul 的訊息。這條規則應該寫在 `Participant` 還是 `Chatroom`？為什麼？
3. 新增一種參與者 `Moderator`，它會收到聊天室裡每一則訊息的副本。需要修改 `Beatle` 與 `NonBeatle` 嗎？

---

## 附錄：Dofactory 原版範例

> 以下保留 Dofactory 網站的三種原版程式碼作為對照，點選標題即可展開。  
> 輸出結果是實際編譯並執行這些程式碼所得（執行時移除了會等待按鍵的 `Console.ReadKey()`，文化特性為 en-US）。  
> 原版使用較舊的 C# 寫法，撰寫新程式時請以上方的「現代 C# 實作」為準。

<details>
<summary><b>Structural code（結構化程式碼）</b></summary>

這段結構化（Structural）程式碼示範了 Mediator Pattern 如何促進不同物件與不同物件型別之間的鬆散耦合通訊。Mediator（中介者）是一個中央樞紐（Central Hub），所有的互動都必須透過它來進行。

```csharp
using System;

namespace Mediator.Structural
{
    /// <summary>
    /// Mediator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            ConcreteMediator m = new ConcreteMediator();

            ConcreteColleague1 c1 = new ConcreteColleague1(m);
            ConcreteColleague2 c2 = new ConcreteColleague2(m);

            m.Colleague1 = c1;
            m.Colleague2 = c2;

            c1.Send("How are you?");
            c2.Send("Fine, thanks");

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Mediator' abstract class
    /// </summary>

    public abstract class Mediator
    {
        public abstract void Send(string message,
            Colleague colleague);
    }

    /// <summary>
    /// The 'ConcreteMediator' class
    /// </summary>

    public class ConcreteMediator : Mediator
    {
        ConcreteColleague1 colleague1;
        ConcreteColleague2 colleague2;

        public ConcreteColleague1 Colleague1
        {
            set { colleague1 = value; }
        }

        public ConcreteColleague2 Colleague2
        {
            set { colleague2 = value; }
        }

        public override void Send(string message, Colleague colleague)
        {
            if (colleague == colleague1)
            {
                colleague2.Notify(message);
            }
            else
            {
                colleague1.Notify(message);
            }
        }
    }

    /// <summary>
    /// The 'Colleague' abstract class
    /// </summary>

    public abstract class Colleague
    {
        protected Mediator mediator;

        // Constructor

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }
    }

    /// <summary>
    /// A 'ConcreteColleague' class
    /// </summary>

    public class ConcreteColleague1 : Colleague
    {
        // Constructor

        public ConcreteColleague1(Mediator mediator)
            : base(mediator)
        {
        }

        public void Send(string message)
        {
            mediator.Send(message, this);
        }

        public void Notify(string message)
        {
            Console.WriteLine("Colleague1 gets message: "
                + message);
        }
    }

    /// <summary>
    /// A 'ConcreteColleague' class
    /// </summary>

    public class ConcreteColleague2 : Colleague
    {
        // Constructor

        public ConcreteColleague2(Mediator mediator)
            : base(mediator)
        {
        }

        public void Send(string message)
        {
            mediator.Send(message, this);
        }

        public void Notify(string message)
        {
            Console.WriteLine("Colleague2 gets message: "
                + message);
        }
    }
}
```

**輸出結果 (Output)**

```text
Colleague2 gets message: How are you?
Colleague1 gets message: Fine, thanks
```

</details>

<details>
<summary><b>Real-world code（真實世界程式碼）</b></summary>

這段真實世界（Real-world）程式碼示範了 Mediator Pattern 如何促進註冊到同一個聊天室（`Chatroom`）的不同參與者（`Participant`）之間的鬆散耦合通訊。`Chatroom` 是所有通訊發生的中央樞紐。目前在此範例中僅實作了一對一（One-to-one）通訊，但若要改為一對多（One-to-many）通訊也是非常容易的。

```csharp
using System;
using System.Collections.Generic;

namespace Mediator.RealWorld
{
    /// <summary>
    /// Mediator Design Pattern
    /// </summary>

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create chatroom

            Chatroom chatroom = new Chatroom();

            // Create participants and register them

            Participant George = new Beatle("George");
            Participant Paul = new Beatle("Paul");
            Participant Ringo = new Beatle("Ringo");
            Participant John = new Beatle("John");
            Participant Yoko = new NonBeatle("Yoko");

            chatroom.Register(George);
            chatroom.Register(Paul);
            chatroom.Register(Ringo);
            chatroom.Register(John);
            chatroom.Register(Yoko);

            // Chatting participants

            Yoko.Send("John", "Hi John!");
            Paul.Send("Ringo", "All you need is love");
            Ringo.Send("George", "My sweet Lord");
            Paul.Send("John", "Can't buy me love");
            John.Send("Yoko", "My sweet love");

            // Wait for user

            Console.ReadKey();
        }
    }

    /// <summary>
    /// The 'Mediator' abstract class
    /// </summary>

    public abstract class AbstractChatroom
    {
        public abstract void Register(Participant participant);
        public abstract void Send(
            string from, string to, string message);
    }

    /// <summary>
    /// The 'ConcreteMediator' class
    /// </summary>

    public class Chatroom : AbstractChatroom
    {
        private Dictionary<string, Participant> participants = new Dictionary<string, Participant>();

        public override void Register(Participant participant)
        {
            if (!participants.ContainsValue(participant))
            {
                participants[participant.Name] = participant;
            }

            participant.Chatroom = this;
        }

        public override void Send(string from, string to, string message)
        {
            Participant participant = participants[to];

            if (participant != null)
            {
                participant.Receive(from, message);
            }
        }
    }

    /// <summary>
    /// The 'AbstractColleague' class
    /// </summary>

    public class Participant
    {
        Chatroom chatroom;
        string name;

        // Constructor

        public Participant(string name)
        {
            this.name = name;
        }

        // Gets participant name

        public string Name
        {
            get { return name; }
        }

        // Gets chatroom

        public Chatroom Chatroom
        {
            set { chatroom = value; }
            get { return chatroom; }
        }

        // Sends message to given participant

        public void Send(string to, string message)
        {
            chatroom.Send(name, to, message);
        }

        // Receives message from given participant

        public virtual void Receive(
            string from, string message)
        {
            Console.WriteLine("{0} to {1}: '{2}'",
                from, Name, message);
        }
    }

    /// <summary>
    /// A 'ConcreteColleague' class
    /// </summary>

    public class Beatle : Participant
    {
        // Constructor

        public Beatle(string name)
            : base(name)
        {
        }

        public override void Receive(string from, string message)
        {
            Console.Write("To a Beatle: ");
            base.Receive(from, message);
        }
    }

    /// <summary>
    /// A 'ConcreteColleague' class
    /// </summary>

    public class NonBeatle : Participant
    {
        // Constructor
        public NonBeatle(string name)
            : base(name)
        {
        }

        public override void Receive(string from, string message)
        {
            Console.Write("To a non-Beatle: ");
            base.Receive(from, message);
        }
    }
}
```

**輸出結果 (Output)**

```text
To a Beatle: Yoko to John: 'Hi John!'
To a Beatle: Paul to Ringo: 'All you need is love'
To a Beatle: Ringo to George: 'My sweet Lord'
To a Beatle: Paul to John: 'Can't buy me love'
To a non-Beatle: John to Yoko: 'My sweet love'
```

</details>

<details>
<summary><b>.NET Optimized code（.NET 最佳化程式碼）</b></summary>

這段 .NET 最佳化（.NET Optimized）程式碼示範了與上述相同的邏輯，但採用了更現代化的 C# 與 .NET 功能（例如 Primary Constructors 與字典索引）。此版本的實作細節與限制，請見上方的〈與 Dofactory 原版的差異〉。

```csharp
namespace Mediator.NetOptimized;

using static System.Console;
using System.Collections.Generic;

/// <summary>
/// Mediator Design Pattern
/// </summary>
public class Program
{
    public static void Main()
    {
        // Create chatroom participants
        var George = new Beatle { Name = "George" };
        var Paul = new Beatle { Name = "Paul" };
        var Ringo = new Beatle { Name = "Ringo" };
        var John = new Beatle { Name = "John" };
        var Yoko = new NonBeatle { Name = "Yoko" };

        // Create chatroom and register participants
        var chatroom = new Chatroom();
        chatroom.Register(George);
        chatroom.Register(Paul);
        chatroom.Register(Ringo);
        chatroom.Register(John);
        chatroom.Register(Yoko);

        // Chatting participants
        Yoko.Send("John", "Hi John!");
        Paul.Send("Ringo", "All you need is love");
        Ringo.Send("George", "My sweet Lord");
        Paul.Send("John", "Can't buy me love");
        John.Send("Yoko", "My sweet love");

        // Wait for user
        ReadKey();
    }
}

/// <summary>
/// The 'Mediator' interface
/// </summary>
public interface IChatroom
{
    void Register(Participant participant);
    void Send(string from, string to, string message);
}

/// <summary>
/// The 'ConcreteMediator' class
/// </summary>
public class Chatroom : IChatroom
{
    private readonly Dictionary<string, Participant> participants = [];

    public void Register(Participant participant)
    {
        participants.TryAdd(participant.Name, participant);
        participant.Chatroom = this;
    }

    public void Send(string from, string to, string message)
    {
        var participant = participants[to];
        if (participant != null)
        {
            participant.Receive(from, message);
        }
    }
}

/// <summary>
/// The 'AbstractColleague' class
/// </summary>
public class Participant
{
    // Gets or sets participant name
    public string Name { get; set; } = null!;

    // Gets or sets chatroom
    public Chatroom Chatroom { get; set; } = null!;

    // Send a message to given participant
    public void Send(string to, string message)
    {
        Chatroom.Send(Name, to, message);
    }

    // Receive message from participant
    public virtual void Receive(string from, string message)
    {
        WriteLine($"{from} to {Name}: '{message}'");
    }
}

/// <summary>
/// A 'ConcreteColleague' class
/// </summary>
public class Beatle : Participant
{
    public override void Receive(string from, string message)
    {
        Write("To a Beatle: ");
        base.Receive(from, message);
    }
}

/// <summary>
/// A 'ConcreteColleague' class
/// </summary>
public class NonBeatle : Participant
{
    public override void Receive(string from, string message)
    {
        Write("To a non-Beatle: ");
        base.Receive(from, message);
    }
}
```

**輸出結果 (Output)**

```text
To a Beatle: Yoko to John: 'Hi John!'
To a Beatle: Paul to Ringo: 'All you need is love'
To a Beatle: Ringo to George: 'My sweet Lord'
To a Beatle: Paul to John: 'Can't buy me love'
To a non-Beatle: John to Yoko: 'My sweet love'
```

</details>

---

## 導覽 (Navigation)

| 上一篇 | 總覽 | 下一篇 |
| :--- | :---: | ---: |
| [← 上一篇：Iterator](Iterator.md) | [🏠 C# Design Patterns 總覽](README.md) | [下一篇：Memento →](Memento.md) |

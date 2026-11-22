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

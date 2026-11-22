// Memento：把物件的狀態存成快照，之後可以還原；快照的內容只有物件自己看得到。

var prospect = new SalesProspect("Noel van Halen", "(412) 256-0990", 25_000m);
var history = new ProspectHistory();
Console.WriteLine($"Initial : {prospect}");

// 修改之前先存檔
history.Save(prospect);
prospect.Name = "Leo Welch";
prospect.Phone = "(310) 209-7111";
prospect.Budget = 1_000_000m;
Console.WriteLine($"Changed : {prospect}");

// 還原到上一次存檔
history.Undo(prospect);
Console.WriteLine($"Restored: {prospect}");

Console.WriteLine($"Undo again: {history.Undo(prospect)}");

// ---- Memento：對外只是一個沒有任何成員的介面，外界可以保管它，但看不到也改不了內容 ----
public interface IMemento
{
}

// ---- Originator：只有它知道快照裡有什麼，也只有它能建立與讀取快照 ----
public sealed class SalesProspect(string name, string phone, decimal budget)
{
    public string Name { get; set; } = name;
    public string Phone { get; set; } = phone;
    public decimal Budget { get; set; } = budget;

    public IMemento Save() => new Snapshot(Name, Phone, Budget);

    public void Restore(IMemento memento)
    {
        if (memento is not Snapshot snapshot)
        {
            throw new ArgumentException("This memento was not created by a SalesProspect.", nameof(memento));
        }

        // record 可以直接解構成多個變數或屬性
        (Name, Phone, Budget) = snapshot;
    }

    public override string ToString() => $"{Name}, {Phone}, budget ${Budget:N2}";

    // 巢狀的 private 型別：SalesProspect 以外的程式碼完全無法存取它的成員
    private sealed record Snapshot(string Name, string Phone, decimal Budget) : IMemento;
}

// ---- Caretaker：負責保管快照與決定何時還原，但從不查看快照內容 ----
public sealed class ProspectHistory
{
    private readonly Stack<IMemento> _snapshots = new();

    public void Save(SalesProspect prospect) => _snapshots.Push(prospect.Save());

    public bool Undo(SalesProspect prospect)
    {
        if (!_snapshots.TryPop(out IMemento? snapshot))
        {
            return false;   // 沒有可以還原的存檔
        }

        prospect.Restore(snapshot);
        return true;
    }
}

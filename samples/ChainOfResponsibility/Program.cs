// Chain of Responsibility：把處理者串成一條鏈，請求沿著鏈往下傳，直到有人處理為止。

// 組裝責任鏈：Director -> Vice President -> President -> 高層會議
PurchaseHandler chain = new Approver("Director", limit: 10_000m);
chain.SetNext(new Approver("Vice President", limit: 25_000m))
     .SetNext(new Approver("President", limit: 100_000m))
     .SetNext(new ExecutiveMeeting());

Purchase[] purchases =
[
    new(2034, 350.00m, "Supplies"),
    new(2035, 32_590.10m, "Project X"),
    new(2036, 122_100.00m, "Project Y"),
];

// 發出請求的人只認得鏈的第一個人，不必知道最後是誰核准
foreach (Purchase purchase in purchases)
{
    chain.Handle(purchase);
}

public sealed record Purchase(int Number, decimal Amount, string Purpose);

// ---- Handler：「處理不了就往下傳」的邏輯只寫在這裡一次 ----
public abstract class PurchaseHandler
{
    private PurchaseHandler? _next;

    // 回傳 next，組裝時才能一路 .SetNext(...).SetNext(...) 串下去
    public PurchaseHandler SetNext(PurchaseHandler next)
    {
        _next = next;
        return next;
    }

    public void Handle(Purchase purchase)
    {
        if (TryHandle(purchase))
        {
            return;
        }

        if (_next is null)
        {
            Console.WriteLine($"Request #{purchase.Number} could not be handled by anyone.");
            return;
        }

        _next.Handle(purchase);
    }

    // 子類別只需要回答：這個請求我能不能處理？
    protected abstract bool TryHandle(Purchase purchase);
}

// ---- ConcreteHandler：各級主管只差在職稱與核准上限，用建構函式參數表達即可 ----
public sealed class Approver(string title, decimal limit) : PurchaseHandler
{
    protected override bool TryHandle(Purchase purchase)
    {
        if (purchase.Amount >= limit)
        {
            return false;
        }

        Console.WriteLine($"{title} approved request #{purchase.Number} ({purchase.Purpose}, ${purchase.Amount:N2})");
        return true;
    }
}

// ---- ConcreteHandler：鏈的最後一關，行為和主管完全不同 ----
public sealed class ExecutiveMeeting : PurchaseHandler
{
    protected override bool TryHandle(Purchase purchase)
    {
        Console.WriteLine($"Request #{purchase.Number} (${purchase.Amount:N2}) requires an executive meeting!");
        return true;
    }
}

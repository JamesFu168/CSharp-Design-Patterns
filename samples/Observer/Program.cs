// Observer：一個物件的狀態改變時，自動通知所有「訂閱」它的物件。C# 用 event 內建支援這個模式。

var ibm = new Stock("IBM", 120.00m);
var sorros = new Investor("Sorros");
var berkshire = new Investor("Berkshire");

// 訂閱：+=
ibm.PriceChanged += sorros.OnPriceChanged;
ibm.PriceChanged += berkshire.OnPriceChanged;

ibm.Price = 120.10m;
ibm.Price = 121.00m;
ibm.Price = 121.00m;   // 價格沒有改變，不會通知

// 取消訂閱：-=。不再需要通知時一定要取消，否則 Stock 會一直持有 Investor，造成記憶體洩漏
ibm.PriceChanged -= berkshire.OnPriceChanged;
Console.WriteLine("Berkshire unsubscribed.");

ibm.Price = 120.50m;

// 事件資料：依 .NET 慣例繼承 EventArgs，並設計成唯讀
public sealed class PriceChangedEventArgs(string symbol, decimal oldPrice, decimal newPrice) : EventArgs
{
    public string Symbol { get; } = symbol;
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
}

// ---- Subject：用 event 宣告「可以被訂閱的通知」 ----
public sealed class Stock(string symbol, decimal price)
{
    // event 只允許外部 += 與 -=，外部不能直接觸發，也不能清掉別人的訂閱
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public string Symbol => symbol;

    public decimal Price
    {
        get;
        set
        {
            // field 代表編譯器替這個屬性產生的欄位，不必另外宣告 _price
            if (field == value)
            {
                return;
            }

            decimal oldPrice = field;
            field = value;

            // ?. 的意思是「有人訂閱才呼叫」
            PriceChanged?.Invoke(this, new PriceChangedEventArgs(symbol, oldPrice, value));
        }
    } = price;
}

// ---- Observer：方法簽章符合 EventHandler<PriceChangedEventArgs> 就能訂閱 ----
public sealed class Investor(string name)
{
    public void OnPriceChanged(object? sender, PriceChangedEventArgs e) =>
        Console.WriteLine($"Notified {name}: {e.Symbol} changed from ${e.OldPrice:N2} to ${e.NewPrice:N2}");
}

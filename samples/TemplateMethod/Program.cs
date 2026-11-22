// Template Method：基底類別固定「流程的順序」，子類別只負責填入其中幾個步驟。

new CategoryReport().Run(top: 5);
new ProductReport().Run(top: 3);

// ---- AbstractClass ----
public abstract class DataReport<T>
{
    // Template Method：流程固定。C# 的方法預設不是 virtual，所以子類別無法改變步驟順序
    public void Run(int top)
    {
        Connect();
        IReadOnlyList<T> rows = Select();

        Console.WriteLine($"{Title} ----");
        foreach (T row in rows.Take(top))
        {
            Console.WriteLine($"  {Format(row)}");
        }

        Disconnect();
        Console.WriteLine();
    }

    // 必要步驟：abstract，子類別一定要實作
    protected abstract string Title { get; }
    protected abstract IReadOnlyList<T> Select();

    // 選擇性步驟（Hook）：virtual，有預設行為，子類別需要時才覆寫
    protected virtual string Format(T row) => row?.ToString() ?? "";
    protected virtual void Connect() => Console.WriteLine("[connect]");
    protected virtual void Disconnect() => Console.WriteLine("[disconnect]");
}

// ---- ConcreteClass ----
public sealed class CategoryReport : DataReport<string>
{
    protected override string Title => "Categories";

    protected override IReadOnlyList<string> Select() =>
        ["Red", "Green", "Blue", "Yellow", "Purple", "White", "Black"];
}

public sealed record Product(string Name, decimal Price);

public sealed class ProductReport : DataReport<Product>
{
    protected override string Title => "Products";

    protected override IReadOnlyList<Product> Select() =>
    [
        new("Car", 25_000m),
        new("Bike", 480m),
        new("Boat", 12_500m),
        new("Truck", 41_000m),
        new("Moped", 1_900m),
    ];

    // 覆寫 Hook：自訂每一列的顯示方式
    protected override string Format(Product row) => $"{row.Name,-6} ${row.Price,10:N2}";
}

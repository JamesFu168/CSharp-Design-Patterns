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

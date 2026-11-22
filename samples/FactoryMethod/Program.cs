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

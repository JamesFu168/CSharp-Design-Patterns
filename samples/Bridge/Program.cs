// Bridge：把「要做什麼」（抽象）和「怎麼做」（實作）拆成兩個各自獨立發展的階層。

ICustomerSource chicago = new InMemoryCustomerSource("Chicago", ["Jim Jones", "Samual Jackson", "Allen Good"]);
ICustomerSource taipei = new CsvCustomerSource("Taipei", "Ann Stills;Lisa Giolani");

// 2 種報表 × 2 種資料來源 = 4 種組合，但只需要 2 + 2 個類別
new CustomerReport(chicago).Print();
new NumberedCustomerReport(chicago).Print();
new CustomerReport(taipei).Print();
new NumberedCustomerReport(taipei).Print();

// ---- Abstraction：報表「要呈現什麼」，透過介面使用資料來源 ----
public class CustomerReport(ICustomerSource source)
{
    public void Print()
    {
        Console.WriteLine($"Customer Group: {source.Group}");

        int index = 0;
        foreach (string customer in source.GetCustomers())
        {
            Console.WriteLine(FormatLine(++index, customer));
        }

        Console.WriteLine();
    }

    protected virtual string FormatLine(int index, string customer) => $"  {customer}";
}

// ---- RefinedAbstraction：改變呈現方式，完全不必知道資料從哪裡來 ----
public sealed class NumberedCustomerReport(ICustomerSource source) : CustomerReport(source)
{
    protected override string FormatLine(int index, string customer) => $"  {index}. {customer}";
}

// ---- Implementor：資料「怎麼取得」 ----
public interface ICustomerSource
{
    string Group { get; }
    IEnumerable<string> GetCustomers();
}

// ---- ConcreteImplementor ----
public sealed class InMemoryCustomerSource(string group, IReadOnlyList<string> customers) : ICustomerSource
{
    public string Group => group;
    public IEnumerable<string> GetCustomers() => customers;
}

public sealed class CsvCustomerSource(string group, string csv) : ICustomerSource
{
    public string Group => group;
    public IEnumerable<string> GetCustomers() => csv.Split(';', StringSplitOptions.TrimEntries);
}

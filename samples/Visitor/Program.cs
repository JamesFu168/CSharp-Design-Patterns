// Visitor：在不修改既有型別的前提下，替「一組固定的型別」新增操作。
// C# 15 可以用 closed 階層（或 union）加上 switch，取代傳統的 Accept / Visit 雙重分派。

Employee[] employees =
[
    new Clerk("Kevin", 25_000m, VacationDays: 14),
    new Director("Elly", 35_000m, VacationDays: 16),
    new President("Eric", 45_000m, VacationDays: 21),
];

// 兩個「操作」都寫在 Employee 階層之外，新增第三個操作也不必修改任何 Employee 類別
foreach (Employee employee in employees)
{
    Employee raised = Payroll.RaiseIncome(employee);
    Console.WriteLine($"{employee.GetType().Name} {employee.Name}: income ${employee.Income:N2} -> ${raised.Income:N2}");
}

Console.WriteLine();

foreach (Employee employee in employees)
{
    Employee rested = Payroll.AddVacation(employee);
    Console.WriteLine($"{employee.GetType().Name} {employee.Name}: vacation {employee.VacationDays} -> {rested.VacationDays} days");
}

Console.WriteLine();

// union：型別之間沒有共同的基底類別時，用 union 宣告「一定是這幾種之一」
Bonus[] bonuses = [new CashBonus(1_500m), new StockBonus("MSFT", 10)];
foreach (Bonus bonus in bonuses)
{
    Console.WriteLine($"Bonus: {Payroll.Describe(bonus)}");
}

// ---- Element：closed 表示「直接子類別只能宣告在同一個組件」，
// 所以編譯器知道 Employee 只可能是下面這三種。closed 類別本身一律是抽象的。
public closed record Employee(string Name, decimal Income, int VacationDays);

// ---- ConcreteElement ----
public sealed record Clerk(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);
public sealed record Director(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);
public sealed record President(string Name, decimal Income, int VacationDays) : Employee(Name, Income, VacationDays);

public sealed record CashBonus(decimal Amount);
public sealed record StockBonus(string Symbol, int Shares);
public union Bonus(CashBonus, StockBonus);

// ---- Visitor：每個操作是一個方法，用 switch 依實際型別決定行為 ----
public static class Payroll
{
    public static Employee RaiseIncome(Employee employee)
    {
        // 三種型別都列出來了，所以不需要 _ => 分支。
        // 日後新增第四種 Employee 而這裡忘了處理，編譯器會提出警告。
        decimal rate = employee switch
        {
            Clerk => 0.10m,
            Director => 0.12m,
            President => 0.15m,
        };

        // with：複製一份並修改指定屬性，原本的物件不會被改動
        return employee with { Income = employee.Income * (1 + rate) };
    }

    public static Employee AddVacation(Employee employee)
    {
        int extraDays = employee switch
        {
            Clerk => 3,
            Director => 5,
            President => 7,
        };

        return employee with { VacationDays = employee.VacationDays + extraDays };
    }

    public static string Describe(Bonus bonus) => bonus switch
    {
        CashBonus cash => $"cash ${cash.Amount:N2}",
        StockBonus stock => $"{stock.Shares} shares of {stock.Symbol}",
    };
}

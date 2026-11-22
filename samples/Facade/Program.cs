// Facade：替一組複雜的子系統提供一個簡單的入口，呼叫端只需要認識這個入口。

// 子系統由外部傳入（建構函式注入），測試時可以換成假的實作
var mortgage = new MortgageService(new Bank(), new CreditBureau(), new LoanRegistry());

Customer ann = new("Ann McKinsey", Savings: 40_000m, CreditScore: 720, HasBadLoans: false);
Customer bob = new("Bob Miller", Savings: 5_000m, CreditScore: 700, HasBadLoans: false);

// 呼叫端只做一件事：申請。三個子系統的檢查細節都藏在 Facade 後面
Console.WriteLine(mortgage.Apply(ann, amount: 125_000m));
Console.WriteLine();
Console.WriteLine(mortgage.Apply(bob, amount: 125_000m));

public sealed record Customer(string Name, decimal Savings, int CreditScore, bool HasBadLoans);

public sealed record MortgageDecision(string CustomerName, bool Approved, string Reason)
{
    public override string ToString() =>
        $"{CustomerName} has been {(Approved ? "Approved" : "Rejected")}: {Reason}";
}

// ---- Facade ----
public sealed class MortgageService(Bank bank, CreditBureau creditBureau, LoanRegistry loanRegistry)
{
    public MortgageDecision Apply(Customer customer, decimal amount)
    {
        Console.WriteLine($"{customer.Name} applies for a ${amount:N2} loan");

        if (!bank.HasSufficientSavings(customer, amount))
        {
            return new(customer.Name, Approved: false, "insufficient savings");
        }

        if (!creditBureau.HasGoodCredit(customer))
        {
            return new(customer.Name, Approved: false, "credit score too low");
        }

        if (loanRegistry.HasBadLoans(customer))
        {
            return new(customer.Name, Approved: false, "existing bad loans");
        }

        return new(customer.Name, Approved: true, "all checks passed");
    }
}

// ---- Subsystem：各自獨立，完全不知道 Facade 的存在 ----
public sealed class Bank
{
    // 自備款至少要有貸款金額的 20%
    public bool HasSufficientSavings(Customer customer, decimal amount)
    {
        Console.WriteLine($"  Check bank savings for {customer.Name}");
        return customer.Savings >= amount * 0.20m;
    }
}

public sealed class CreditBureau
{
    public bool HasGoodCredit(Customer customer)
    {
        Console.WriteLine($"  Check credit score for {customer.Name}");
        return customer.CreditScore >= 650;
    }
}

public sealed class LoanRegistry
{
    public bool HasBadLoans(Customer customer)
    {
        Console.WriteLine($"  Check loan history for {customer.Name}");
        return customer.HasBadLoans;
    }
}

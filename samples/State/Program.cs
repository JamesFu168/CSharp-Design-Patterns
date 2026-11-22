// State：把「每個狀態下該怎麼做」各自寫成一個類別，物件把工作轉交給目前的狀態物件。

var account = new Account("Jim Johnson");

account.Deposit(500m);
account.Deposit(300m);
account.Deposit(550m);
account.PayInterest();
account.Withdraw(2_000m);
account.Withdraw(1_100m);

// ---- State：定義每個狀態都要回答的問題，並提供預設答案 ----
public abstract class AccountState
{
    public static readonly AccountState Red = new RedState();
    public static readonly AccountState Silver = new SilverState();
    public static readonly AccountState Gold = new GoldState();

    public abstract string Name { get; }

    public virtual decimal CalculateInterest(decimal balance) => 0m;

    public virtual bool CanWithdraw => true;

    // 狀態轉換的規則集中在一個地方，而不是散落在每個狀態類別裡
    public static AccountState For(decimal balance) => balance switch
    {
        < 0m => Red,
        < 1_000m => Silver,
        _ => Gold,
    };
}

// ---- ConcreteState：只覆寫和預設不同的行為 ----
public sealed class RedState : AccountState
{
    public override string Name => "Red";

    // 透支中：不能再提款
    public override bool CanWithdraw => false;
}

public sealed class SilverState : AccountState
{
    public override string Name => "Silver";
}

public sealed class GoldState : AccountState
{
    public override string Name => "Gold";

    // 只有 Gold 帳戶有利息
    public override decimal CalculateInterest(decimal balance) => balance * 0.05m;
}

// ---- Context：沒有任何「if 現在是某某狀態」的判斷，只問目前的狀態物件 ----
public sealed class Account(string owner)
{
    private AccountState _state = AccountState.Silver;

    public decimal Balance { get; private set; }

    public void Deposit(decimal amount)
    {
        ChangeBalance(Balance + amount);
        Report($"Deposited ${amount:N2}");
    }

    public void Withdraw(decimal amount)
    {
        if (!_state.CanWithdraw)
        {
            Report($"Withdrawal of ${amount:N2} refused");
            return;
        }

        ChangeBalance(Balance - amount);
        Report($"Withdrew ${amount:N2}");
    }

    public void PayInterest()
    {
        ChangeBalance(Balance + _state.CalculateInterest(Balance));
        Report("Interest paid");
    }

    private void ChangeBalance(decimal newBalance)
    {
        Balance = newBalance;
        _state = AccountState.For(Balance);
    }

    private void Report(string action) =>
        Console.WriteLine($"{owner}: {action,-32} Balance = {Balance,10:N2}  Status = {_state.Name}");
}
